using Stashbox.Registration.Fluent;

namespace Stashbox.Registration
{
    internal interface IRegistrationBuilder
    {
        IServiceRegistration BuildServiceRegistration(RegistrationContext registrationContext, bool isDecorator);
    }
}
