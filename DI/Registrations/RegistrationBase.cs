using System;

namespace Stannieman.DI.Registrations
{
    internal abstract class RegistrationBase
    {
        public Type RegistrationType { get; }
        public string Key { get; }

        public RegistrationBase(Type registrationType, string key)
        {
            RegistrationType = registrationType;
            Key = key;
        }
    }
}
