using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Expressions
{
    internal interface IExpressionBuilder
    {
        Expression CreateFillExpression(
            IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            Expression instance,
            Type serviceType);

        Expression CreateBasicFillExpression(
            IContainerContext containerContext,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Expression instance,
            Type serviceType);

        Expression CreateExpression(
            IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            Type serviceType);

        Expression CreateBasicExpression(
            IContainerContext containerContext,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Type serviceType);
    }
}
