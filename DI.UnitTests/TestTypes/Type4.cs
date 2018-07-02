namespace Stannieman.DI.UnitTests.TestTypes
{
    public interface IType4 { }

    public class Type4 : IType4
    {
        public IType4_1 Type4_1_1 { get; set; }

        protected IType4_1 Type4_1_2 { get; set; }

        /// <summary>
        /// Static so use only in 1 test method of parallel test running is on!
        /// </summary>
        public static IType4_1 Type4_1_3 { get; set; }

        public IType4_1 Type4_1_4;

        public IType4_1 Type4_1_5 { set; protected get; }

        public IType4_1 Type4_1_6 { protected set; get; }

        public Type4()
        {
            Type4_1_1 = new Type4_1();
        }

        public IType4_1 GetType4_1_2() => Type4_1_2;

        public IType4_1 GetType4_1_5() => Type4_1_5;
    }


    public interface IType4_1 { }

    public class Type4_1 : IType4_1 { }
}
