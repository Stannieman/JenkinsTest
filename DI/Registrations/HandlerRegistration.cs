using System;

namespace Stannieman.DI.Registrations
{
    internal class HandlerRegistration : RegistrationBase
    {
        public Func<HandlerParameter, object> Handler { get; }

        public HandlerRegistration(Type registrationType, string key, Func<HandlerParameter, object> handler)
            : base(registrationType, key)
        {
            Handler = handler;
        }
    }
}
