namespace Stannieman.DI
{
    public enum ContainerErrorCodes
    {
        TypeAlreadyRegisteredAsSingleton = 1,
        TypeAlreadyRegisteredAsPerRequest = 2,
        NoEligibleConstructorFound = 3,
        MultipleImplementationTypesRegistered = 4
    }
}
