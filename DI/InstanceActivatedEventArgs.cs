using System;

namespace Stannieman.DI
{
    public class InstanceActivatedEventArgs : EventArgs
    {
        public object Instance { get; }

        public InstanceActivatedEventArgs(object instance)
        {
            Instance = instance;
        }
    }
}
