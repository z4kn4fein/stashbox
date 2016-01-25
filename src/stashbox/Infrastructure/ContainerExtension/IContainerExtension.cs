namespace Stashbox.Infrastructure.ContainerExtension
{
    /// <summary>
    /// Represents a container extension.
    /// </summary>
    public interface IContainerExtension
    {
        /// <summary>
        /// Initializes the container extension.
        /// </summary>
        /// <param name="containerContext">The <see cref="IContainerContext"/> of the <see cref="StashboxContainer"/></param>
        void Initialize(IContainerContext containerContext);

        /// <summary>
        /// Cleans up the container extension.
        /// </summary>
        void CleanUp();

        /// <summary>
        /// Creates a copy of the extension.
        /// </summary>
        /// <returns>The actual copy.</returns>
        IContainerExtension CreateCopy();
    }
}
