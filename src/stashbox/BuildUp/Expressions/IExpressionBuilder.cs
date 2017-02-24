using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Expressions
{
    internal interface IExpressionBuilder
    {
        Func<object> CompileExpression(Expression expression);

        Expression CreateFillExpression(IContainerExtensionManager extensionManager, IContainerContext containerContext, Expression instance,
            ResolutionInfo resolutionInfo, TypeInformation typeInfo, InjectionParameter[] parameters, ResolutionMember[] members, ResolutionMethod[] methods);

        Expression CreateExpression(IContainerExtensionManager extensionManager, IContainerContext containerContext,
            ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo,
            TypeInformation typeInfo, InjectionParameter[] parameters, ResolutionMember[] members, ResolutionMethod[] methods);
    }
}
