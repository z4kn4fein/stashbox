using Stashbox.Registration.Fluent;

namespace Stashbox.Registration
{
    internal interface IRegistrationBuilder
    {
        IServiceRegistration BuildServiceRegistration(RegistrationConfigurator registrationContext, bool isDecorator);
    }
}
