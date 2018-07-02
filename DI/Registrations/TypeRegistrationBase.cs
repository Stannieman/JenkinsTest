using System;

namespace Stannieman.DI.Registrations
{
    internal abstract class TypeRegistrationBase : RegistrationBase
    {
        public Type ImplementationType { get; }

        public TypeRegistrationBase(Type registrationType, string key, Type implementationType)
            : base(registrationType, key)
        {
            ImplementationType = implementationType;
        }
    }
}
