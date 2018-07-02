namespace Stannieman.DI.UnitTests.TestTypes
{
    public interface IRegisterByBaseTypeInterface { }

    public class RegisterByBaseTypeType1 : IRegisterByBaseTypeInterface { }

    public class RegisterByBaseType2 : RegisterByBaseTypeType1 { }

    public abstract class RegisterByBaseType3 : RegisterByBaseTypeType1 { }

    public class RegisterByBaseType4 : RegisterByBaseType3 { }
}
