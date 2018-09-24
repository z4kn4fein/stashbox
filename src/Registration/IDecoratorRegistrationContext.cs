namespace Stashbox.Registration
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

        /// <summary>
        /// Replaces an already registered service.
        /// </summary>
        /// <returns>The container.</returns>
        IStashboxContainer ReMap();
    }
}
