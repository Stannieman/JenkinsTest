using System;
using System.Collections.Generic;

namespace Stannieman.DI
{
    public interface IContainer : IDisposable
    {
        event EventHandler<InstanceActivatedEventArgs> Activated;

        IEnumerable<object> GetAllInstances(Type registrationType, string key = null);
        object GetSingleInstance(Type registrationType, string key = null);
        bool IsRegistered(Type type, string key = null);
        bool IsSingleRegistered(Type type, string key = null);
        void RegisterHandler(Type registrationType, Func<HandlerParameter, object> handler, string key = null);
        void RegisterPerRequest(Type registrationType, Type implementationType, string key = null);
        void RegisterSingleton(Type registrationType, Type implementationType, string key = null);
    }
}