namespace Stashbox.Infrastructure.ContainerExtension
{
    public interface IContainerExtension
    {
        void Initialize(IContainerContext containerContext);
        void CleanUp();
        IContainerExtension CreateCopy();
    }
}
