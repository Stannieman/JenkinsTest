namespace Stannieman.DI
{
    public class HandlerParameter
    {
        public Container ParentContainer { get; }
        public object Target { get; }

        public HandlerParameter(Container parentContainer, object target)
        {
            ParentContainer = parentContainer;
            Target = target;
        }
    }
}
