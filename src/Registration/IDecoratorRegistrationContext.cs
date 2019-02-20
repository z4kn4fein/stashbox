namespace Stashbox.Registration
{
    internal interface IDecoratorRegistrationContext : IFluentDecoratorRegistrator
    {
        IRegistrationContext RegistrationContext { get; }
    }
}
