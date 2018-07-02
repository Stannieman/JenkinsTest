namespace Stannieman.DI.UnitTests.TestTypes
{
    public interface IType3 { }

    public class Type3 : IType3
    {
        [DependencyKey("Key2")]
        public IType3_2 Type3_2 { get; set; }

        public IType3_1 Type3_1 { get; }

        public Type3([DependencyKey("Key1")] IType3_1 type3_1)
        {
            Type3_1 = type3_1;
        }
    }


    public interface IType3_1 { }

    public class Type3_1 : IType3_1 { }


    public interface IType3_2 { }

    public class Type3_2Impl1 : IType3_2 { }

    public class Type3_2Impl2 : IType3_2 { }
}
