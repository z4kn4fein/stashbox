using Stashbox.Entity;
using System;
using System.Linq.Expressions;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.BuildUp.Expressions
{
    internal interface IExpressionBuilder
    {
        Expression CreateFillExpression(IServiceRegistration serviceRegistration, Expression instance, ResolutionInfo resolutionInfo, Type serviceType);

        Expression CreateExpression(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type serviceType);
    }
}
