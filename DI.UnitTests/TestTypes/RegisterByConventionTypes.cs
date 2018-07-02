namespace Stannieman.DI.UnitTests.TestTypes
{
    public interface IRegisterByConventionInterface1 { }

    public interface IRegisterByConventionInterface2 { }

    public class RegisterByConventionType1ByConvention { }

    public abstract class RegisterByConventionType2ByConvention { }

    public class RegisterByConventionType3ByConvention : IRegisterByConventionInterface1 { }

    public class RegisterByConventionType4ByConvention : RegisterByConventionType3ByConvention, IRegisterByConventionInterface2 { }

    public class RegisterByConventionType5ByConvention : RegisterByConventionType2ByConvention { }
}
