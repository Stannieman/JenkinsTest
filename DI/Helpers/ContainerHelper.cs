using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stannieman.DI.Helpers
{
    internal class ContainerHelper
    {
        public object GetDefault(Type type)
        {
            return type.GetTypeInfo().IsValueType
                ? Activator.CreateInstance(type)
                : null;
        }

        public bool IsGenericEnumerable(Type type)
        {
            return type.IsGenericType
                && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(IEnumerable<>));
        }

        public string GetKeyFromInjectable(PropertyInfo property)
        {
            var keyAttribute = property.GetCustomAttribute<DependencyKey>(false);
            if (keyAttribute == null)
            {
                return null;
            }

            return keyAttribute.Key;
        }

        public string GetKeyFromInjectable(ParameterInfo parameter)
        {
            var keyAttribute = parameter.GetCustomAttribute<DependencyKey>(false);
            if (keyAttribute == null)
            {
                return null;
            }

            return keyAttribute.Key;
        }
    }
}
