using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Expressions
{
    internal interface IExpressionBuilder
    {
        Expression CreateFillExpression(Expression instance, ResolutionInfo resolutionInfo, Type serviceType, 
            InjectionParameter[] parameters, ResolutionMember[] members, ResolutionMethod[] methods);

        Expression CreateExpression(ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo,
            Type serviceType, InjectionParameter[] parameters, ResolutionMember[] members, ResolutionMethod[] methods);
    }
}
