using System;

namespace Stannieman.DI
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DependencyKey : Attribute
    {
        public string Key { get; set; }

        public DependencyKey(string key)
        {
            Key = key;
        }
    }
}
