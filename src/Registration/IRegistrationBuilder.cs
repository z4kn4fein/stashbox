namespace Stashbox.Registration
{
    internal interface IRegistrationBuilder
    {
        IServiceRegistration BuildServiceRegistration(IRegistrationContext registrationContext, bool isDecorator);
    }
}
