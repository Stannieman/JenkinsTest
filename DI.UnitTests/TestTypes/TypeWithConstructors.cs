namespace Stannieman.DI.UnitTests.TestTypes
{
    public interface ITypeWithConstructors { }

    public class TypeWithConstructors : ITypeWithConstructors
    {
        public bool CorrectConstructorCalled { get; }

        public TypeWithConstructors() { }

        public TypeWithConstructors(ITypeWithConstructors_1 typeWithConstructors_1) { }

        public TypeWithConstructors(ITypeWithConstructors_1 typeWithConstructors_1, ITypeWithConstructors_2 typeWithConstructors_2, ITypeWithConstructors_4 typeWithConstructors_4)
        {
            CorrectConstructorCalled = true;
        }

        public TypeWithConstructors(ITypeWithConstructors_1 typeWithConstructors_1, ITypeWithConstructors_2 typeWithConstructors_2, ITypeWithConstructors_3 typeWithConstructors_3, ITypeWithConstructors_4 typeWithConstructors_4) { }
    }

    public interface ITypeWithConstructors_1 { }

    public class TypeWithConstructors_1 : ITypeWithConstructors_1 { }


    public interface ITypeWithConstructors_2 { }

    public class TypeWithConstructors_2 : ITypeWithConstructors_2 { }


    public interface ITypeWithConstructors_3 { }

    public class TypeWithConstructors_3 : ITypeWithConstructors_3 { }


    public interface ITypeWithConstructors_4 { }

    public class TypeWithConstructors_4 : ITypeWithConstructors_4 { }
}
