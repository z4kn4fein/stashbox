namespace Stashbox;

/// <summary>
/// Represents a composition root used by the <see cref="CollectionRegistratorExtensions.ComposeBy{TCompositionRoot}"/>, <see cref="IDependencyCollectionRegistrator.ComposeBy(System.Type, object[])"/>, <see cref="CollectionRegistratorExtensions.ComposeAssembly"/>, and <see cref="CollectionRegistratorExtensions.ComposeAssemblies"/> methods.
/// </summary>
public interface ICompositionRoot
{
    /// <summary>
    /// Composes services through the <see cref="CollectionRegistratorExtensions.ComposeBy{TCompositionRoot}"/>, <see cref="IDependencyCollectionRegistrator.ComposeBy(System.Type, object[])"/>, <see cref="CollectionRegistratorExtensions.ComposeAssembly"/>, and <see cref="CollectionRegistratorExtensions.ComposeAssemblies"/> methods.
    /// </summary>
    /// <param name="container">The <see cref="IStashboxContainer"/>.</param>
    void Compose(IStashboxContainer container);
}