using System;

namespace Stannieman.DI.Registrations
{
    internal class PerRequestTypeRegistration : TypeRegistrationBase
    {
        public PerRequestTypeRegistration(Type registrationType, string key, Type implementationType)
            : base(registrationType, key, implementationType) { }
    }
}
