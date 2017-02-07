using System.Linq.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;

namespace Stashbox.BuildUp.Resolution
{
    internal class ParameterExpressionResolver : Resolver
    {
        private readonly ParameterExpression parameterExpression;

        public ParameterExpressionResolver(IContainerContext containerContext, TypeInformation typeInfo, ParameterExpression parameterExpression) 
            : base(containerContext, typeInfo)
        {
            this.parameterExpression = parameterExpression;
        }

        public override Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            return parameterExpression;
        }
    }
}
