using System.Collections.Generic;

namespace Stannieman.DI.UnitTests.TestTypes
{
    public class TypeWithCollectionsByKey : ITypeWithCollectionsByKey
    {
        [DependencyKey("Key2")]
        public IEnumerable<ICollectionType1> CollectionType2s { get; set; }

        public IEnumerable<ICollectionType1> CollectionType1s { get; }

        public TypeWithCollectionsByKey([DependencyKey("Key1")] IEnumerable<ICollectionType1> collectionType1s)
        {
            CollectionType1s = collectionType1s;
        }
    }
}
