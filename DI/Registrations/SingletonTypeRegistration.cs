using System;

namespace Stannieman.DI.Registrations
{
    internal class SingletonTypeRegistration : TypeRegistrationBase
    {
        public object Instance { get; set; }

        public SingletonTypeRegistration(Type registrationType, string key, Type implementationType)
            : base(registrationType, key, implementationType) { }
    }
}
