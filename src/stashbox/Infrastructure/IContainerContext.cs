namespace Stashbox.Infrastructure
{
    public interface IContainerContext
    {
        IRegistrationRepository RegistrationRepository { get; }
        IStashboxContainer Container { get; }
        IResolutionStrategy ResolutionStrategy { get; }
    }
}
