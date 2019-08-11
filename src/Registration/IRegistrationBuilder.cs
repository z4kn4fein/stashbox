using Stashbox.Registration.Fluent;

namespace Stashbox.Registration
{
    internal interface IRegistrationBuilder
    {
        IServiceRegistration BuildServiceRegistration(RegistrationConfiguration registrationConfiguration, bool isDecorator);
    }
}
