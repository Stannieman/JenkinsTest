using System.Collections.Generic;

namespace Stannieman.DI.UnitTests.TestTypes
{
    public class TypeWithCollections : ITypeWithCollections
    {
        public IEnumerable<ICollectionType2> CollectionType2s { get; set; }

        public IEnumerable<ICollectionType1> CollectionType1s { get; }

        public TypeWithCollections(IEnumerable<ICollectionType1> collectionType1s)
        {
            CollectionType1s = collectionType1s;
        }
    }
}
