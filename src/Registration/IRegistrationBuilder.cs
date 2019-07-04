using Stashbox.Registration.Fluent;

namespace Stashbox.Registration
{
    internal interface IRegistrationBuilder
    {
        IServiceRegistration BuildServiceRegistration(IRegistrationConfiguration registrationConfiguration, bool isDecorator);
    }
}
