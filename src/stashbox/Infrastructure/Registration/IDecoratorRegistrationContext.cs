namespace Stashbox.Infrastructure.Registration
{
    /// <summary>
    /// Represents a decorator registration context.
    /// </summary>
    public interface IDecoratorRegistrationContext : IFluentDecoratorRegistrator
    {
        /// <summary>
        /// Registers the registration into the container.
        /// </summary>
        /// <returns>The container.</returns>
        IStashboxContainer Register();
    }
}
