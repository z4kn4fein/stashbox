namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a composition root used by the <see cref="IDependencyCollectionRegistrator.ComposeBy{TCompositionRoot}"/>, <see cref="IDependencyCollectionRegistrator.ComposeBy"/>, <see cref="IDependencyCollectionRegistrator.ComposeAssembly"/>, <see cref="IDependencyCollectionRegistrator.ComposeAssemblies"/>.
    /// </summary>
    public interface ICompositionRoot
    {
        /// <summary>
        /// Composes services during the call of the <see cref="IDependencyCollectionRegistrator.ComposeBy{TCompositionRoot}"/>, <see cref="IDependencyCollectionRegistrator.ComposeBy"/>, <see cref="IDependencyCollectionRegistrator.ComposeAssembly"/>, <see cref="IDependencyCollectionRegistrator.ComposeAssemblies"/>.
        /// </summary>
        /// <param name="container">The <see cref="IStashboxContainer"/>.</param>
        void Compose(IStashboxContainer container);
    }
}
