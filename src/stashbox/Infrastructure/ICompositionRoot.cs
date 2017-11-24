namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a composition root used by the <see cref="CollectionRegistratorExtensions.ComposeBy{TCompositionRoot}"/>, <see cref="IDependencyCollectionRegistrator.ComposeBy"/>, <see cref="CollectionRegistratorExtensions.ComposeAssembly"/>, <see cref="CollectionRegistratorExtensions.ComposeAssemblies"/>.
    /// </summary>
    public interface ICompositionRoot
    {
        /// <summary>
        /// Composes services during the call of the <see cref="CollectionRegistratorExtensions.ComposeBy{TCompositionRoot}"/>, <see cref="IDependencyCollectionRegistrator.ComposeBy"/>, <see cref="CollectionRegistratorExtensions.ComposeAssembly"/>, <see cref="CollectionRegistratorExtensions.ComposeAssemblies"/>.
        /// </summary>
        /// <param name="container">The <see cref="IStashboxContainer"/>.</param>
        void Compose(IStashboxContainer container);
    }
}
