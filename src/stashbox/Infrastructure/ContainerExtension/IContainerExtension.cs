namespace Stashbox.Infrastructure.ContainerExtension
{
    public interface IContainerExtension
    {
        void Initialize(IContainerContext containerContext);
        IContainerExtension CreateCopy();
    }
}
