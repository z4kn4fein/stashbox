using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using System;
using System.Linq.Expressions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.BuildUp.Expressions
{
    internal interface IExpressionBuilder
    {
        Expression CreateFillExpression(IMetaInfoProvider metaInfoProvider, Expression instance, ResolutionInfo resolutionInfo, Type serviceType);

        Expression CreateFillExpression(IServiceRegistration serviceRegistration, Expression instance, ResolutionInfo resolutionInfo, Type serviceType);

        Expression CreateExpression(IServiceRegistration serviceRegistration, ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo, Type serviceType);
    }
}
