using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Expressions
{
    internal interface IExpressionBuilder
    {
        Expression CreateFillExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, Expression instance, ResolutionInfo resolutionInfo, Type serviceType);

        Expression CreateExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type serviceType);
    }
}
