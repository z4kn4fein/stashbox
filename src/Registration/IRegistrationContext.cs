namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a registration context. Allows a fluent registration configuration.
    /// </summary>
    public interface IRegistrationContext<TService> : IRegistrationContext, IFluentServiceRegistrator<TService>
    { }

    /// <summary>
    /// Represents a registration context. Allows a fluent registration configuration.
    /// </summary>
    public interface IRegistrationContext : IFluentServiceRegistrator
    {
        /// <summary>
        /// Creates an <see cref="IServiceRegistration"/>.
        /// </summary>
        /// <param name="isDecorator">True if the requested registration is a decorator.</param>
        /// <returns>The created <see cref="IServiceRegistration"/>.</returns>
        IServiceRegistration CreateServiceRegistration(bool isDecorator);

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
