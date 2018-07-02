using Stannieman.DI.Helpers;
using Stannieman.DI.Registrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Stannieman.DI
{
    public class Container : IContainer
    {
        private readonly List<RegistrationBase> _registrations = new List<RegistrationBase>();
        private readonly object _lock = new object();
        private readonly ContainerConfiguration _configuration;
        private readonly List<KeyValuePair<object, object>> _trackedDependencies = new List<KeyValuePair<object, object>>();

        private readonly ContainerHelper _containerHelper = new ContainerHelper();

        public event EventHandler<InstanceActivatedEventArgs> Activated;

        public Container(ContainerConfiguration configuration)
        {
            _configuration = configuration.Clone();
        }

        public void RegisterPerRequest(Type registrationType, Type implementationType, string key = null)
        {
            Register(registrationType, implementationType, false, key);
        }

        public void RegisterSingleton(Type registrationType, Type implementationType, string key = null)
        {
            Register(registrationType, implementationType, true, key);
        }

        private void Register(Type registrationType, Type implementationType, bool registerSingleton, string key)
        {
            lock (_lock)
            {
                var registrationsForKey = _registrations.Where(x => x.Key == key);

                var perRequestRegistrationsForKeyAndImplementationType = registrationsForKey.OfType<PerRequestTypeRegistration>()
                                                                            .Where(x => x.ImplementationType == implementationType);

                var singletonRegistrationsForKeyAndImplementationType = registrationsForKey.OfType<SingletonTypeRegistration>()
                                                                            .Where(x => x.ImplementationType == implementationType);
                

                if (!registerSingleton
                    && singletonRegistrationsForKeyAndImplementationType.Any())
                {
                    throw new ContainerException(ContainerErrorCodes.TypeAlreadyRegisteredAsSingleton,
                        $"The implementation type {implementationType.FullName} was already registered as PerRequest for the key {key}.");
                }

                if (registerSingleton
                    && perRequestRegistrationsForKeyAndImplementationType.Any())
                {
                    throw new ContainerException(ContainerErrorCodes.TypeAlreadyRegisteredAsPerRequest,
                        $"The implementation type {implementationType.FullName} was already registered as Singleton for the key {key}.");
                }

                if (!registerSingleton
                    && !perRequestRegistrationsForKeyAndImplementationType.Any(x => x.RegistrationType == registrationType))
                {
                    _registrations.Add(new PerRequestTypeRegistration(registrationType, key, implementationType));
                    return;
                }

                if (registerSingleton
                    && !singletonRegistrationsForKeyAndImplementationType.Any())
                {
                    _registrations.Add(new SingletonTypeRegistration(registrationType, key, implementationType));
                    return;
                }
            }
        }

        public void RegisterHandler(Type registrationType, Func<HandlerParameter, object> handler, string key = null)
        {
            lock (_lock)
            {
                _registrations.Add(new HandlerRegistration(registrationType, key, handler));
            }
        }

        public object GetSingleInstance(Type registrationType, string key = null)
        {
            return GetSingleInstance(registrationType, null, key);
        }

        public IEnumerable<object> GetAllInstances(Type registrationType, string key = null)
        {
            return GetAllInstances(registrationType, null, key);
        }

        private object GetSingleInstance(Type registrationType, object target, string key)
        {
            if (registrationType == null)
            {
                return null;
            }

            lock (_lock)
            {
                var registrations = _registrations.Where(x => x.RegistrationType == registrationType && x.Key == key);

                var registrationsCount = registrations.Count();
                if (registrationsCount > 1)
                {
                    // More than 1 implementation type registerd for this registration type.
                    // No idea which one to pick.
                    throw new ContainerException(ContainerErrorCodes.MultipleImplementationTypesRegistered,
                        $"Cannot get a single instance for registration type {registrationType.FullName} because multiple implementation types have been registered for this registration type.");
                }

                if (registrationsCount == 1)
                {
                    // There is exactly 1 implementation type registered for this registration type.
                    // This is ok, so construct it.
                    var registration = registrations.First();
                    if (registration is TypeRegistrationBase typeRegistration)
                    {
                        return GetImplementationInstance(typeRegistration, target);
                    }

                    if (registration is HandlerRegistration handlerRegistration)
                    {
                        return handlerRegistration.Handler(new HandlerParameter(this, target));
                    }
                }

                return _containerHelper.GetDefault(registrationType);
            }
        }

        private IEnumerable<object> GetAllInstances(Type registrationType, object target, string key)
        {
            if (registrationType == null)
            {
                return (IEnumerable<object>)Array.CreateInstance(registrationType, 0);
            }

            lock (_lock)
            {
                var registrations = _registrations.Where(x => x.RegistrationType == registrationType && x.Key == key);
                if (!registrations.Any())
                {
                    return (IEnumerable<object>)Array.CreateInstance(registrationType, 0);
                }

                var array = Array.CreateInstance(registrationType, registrations.Count());

                var i = 0;
                foreach (var typeRegistration in registrations.OfType<TypeRegistrationBase>())
                {
                    array.SetValue(GetImplementationInstance(typeRegistration, target), i);
                    i++;
                }

                var handlerParameter = new HandlerParameter(this, target);
                foreach (var handlerRegistration in registrations.OfType<HandlerRegistration>())
                {
                    array.SetValue(handlerRegistration.Handler(handlerParameter), i);
                    i++;
                }

                return (IEnumerable<object>)array;
            }
        }

        private object GetImplementationInstance(TypeRegistrationBase registration, object target)
        {
            if (registration is SingletonTypeRegistration singletonRegistration)
            {
                return singletonRegistration.Instance =
                    singletonRegistration.Instance ?? ConstructInstance(registration.ImplementationType, target, true);
            }

            return ConstructInstance(registration.ImplementationType, target, false);
        }

        private object ConstructInstance(Type implementationType, object target, bool singleton)
        {
            var constructor = FindEligibleConstructor(implementationType);
            if (constructor == null)
            {
                throw new ContainerException(ContainerErrorCodes.NoEligibleConstructorFound,
                    $"No eligible constructor found for type {implementationType.FullName}.");
            }

            var instance = FormatterServices.GetUninitializedObject(implementationType);

            _trackedDependencies.Add(new KeyValuePair<object, object>(singleton ? null : target, instance));

            var constructorArguments = GetConstructorArguments(constructor, instance);

            constructor.Invoke(instance, constructorArguments);

            InjectPropertiesIfNeeded(instance);

            Activated?.Invoke(this, new InstanceActivatedEventArgs(instance));

            return instance;
        }

        private void InjectPropertiesIfNeeded(object instance)
        {
            if (_configuration.EnablePropertyInjection)
            {
                var injectables = instance.GetType().GetProperties()
                    .Where(x => (x.GetGetMethod()?.IsPublic ?? false)
                        && (x.GetSetMethod()?.IsPublic ?? false)
                        && !(x.GetGetMethod()?.IsStatic ?? false)
                        && x.GetValue(instance) == null);

                foreach (var injectable in injectables)
                {
                    var key = _containerHelper.GetKeyFromInjectable(injectable);

                    var injectInstance = _containerHelper.IsGenericEnumerable(injectable.PropertyType)
                        ? GetAllInstances(injectable.PropertyType.GenericTypeArguments.First(), instance, key)
                        : GetSingleInstance(injectable.PropertyType, instance, key);

                    injectable.SetValue(instance, injectInstance);
                }
            }
        }

        private object[] GetConstructorArguments(ConstructorInfo constructor, object target)
        {
            var args = new List<object>();

            args.AddRange(constructor.GetParameters().Select(x =>
            {
                var key = _containerHelper.GetKeyFromInjectable(x);

                if (_containerHelper.IsGenericEnumerable(x.ParameterType))
                {
                    return GetAllInstances(x.ParameterType.GenericTypeArguments.First(), target, key);
                }

                return GetSingleInstance(x.ParameterType, target, key);
            }));

            return args.ToArray();
        }

        private ConstructorInfo FindEligibleConstructor(Type type)
        {
            var publicConstructors = type.GetTypeInfo().DeclaredConstructors.Where(x => x.IsPublic);

            ConstructorInfo candidate = null;
            int bestParametersCount = -1;

            foreach (var constructor in publicConstructors)
            {
                var parameters = constructor.GetParameters();
                var currentParametersCount = parameters.Length;
                var resolvableParameters = 0;

                if (currentParametersCount <= bestParametersCount)
                {
                    continue;
                }

                foreach (var parameter in parameters)
                {
                    var key = _containerHelper.GetKeyFromInjectable(parameter);
                    if (_containerHelper.IsGenericEnumerable(parameter.ParameterType))
                    {
                        resolvableParameters++;
                    }
                    else
                    {
                        if (IsSingleRegistered(parameter.ParameterType, key))
                        {
                            resolvableParameters++;
                        }
                    }
                }

                if (resolvableParameters == currentParametersCount
                    && currentParametersCount > bestParametersCount)
                {
                    candidate = constructor;
                    bestParametersCount = currentParametersCount;
                }
            }

            return candidate;
        }

        public bool IsRegistered(Type type, string key = null)
        {
            return IsRegistered(type, false, key);
        }

        public bool IsSingleRegistered(Type type, string key = null)
        {
            return IsRegistered(type, true, key);
        }

        private bool IsRegistered(Type type, bool single, string key = null)
        {
            var count = _registrations.Count(x => x.RegistrationType == type && x.Key == key);

            return single
                ? count == 1
                : count > 0;
        }

        public void ReleaseInstance(object instance)
        {
            if (instance == null)
            {
                return;
            }

            lock (_lock)
            {
                var dependenciesToRelease = _trackedDependencies.Where(x => x.Key == null && x.Value == instance
                    && !_registrations.OfType<SingletonTypeRegistration>().Any(y => y.Instance == x.Value)).ToList();
                dependenciesToRelease.ForEach(x => InternalReleaseInstance(x));
            }
        }

        private void InternalReleaseInstance(KeyValuePair<object, object> trackedDependency)
        {
            if (trackedDependency.Value is IDisposable disposableInstance)
            {
                try
                {
                    disposableInstance.Dispose();
                }
                catch { }
            }

            _trackedDependencies.Remove(trackedDependency);

            var dependenciesToRelease = _trackedDependencies.Where(x => x.Key == trackedDependency.Value
                && !_registrations.OfType<SingletonTypeRegistration>().Any(y => y.Instance == x.Value)).ToList();
            dependenciesToRelease.ForEach(x => InternalReleaseInstance(x));            
        }

        public void Dispose()
        {
            lock (_lock)
            {
                var dependenciesToRelease = _trackedDependencies.Where(x => x.Key == null
                    && !_registrations.OfType<SingletonTypeRegistration>().Any(y => y.Instance == x.Value)).ToList();
                dependenciesToRelease.ForEach(x => InternalReleaseInstance(x));

                // By now only singletons and their dependencies are left.
                _trackedDependencies.ToList().ForEach(x => InternalReleaseInstance(x));
                _registrations.OfType<SingletonTypeRegistration>().ForEach(x => x.Instance = null);
            }
        }
    }
}
