using System;
using System.Linq.Expressions;
using Stashbox.Entity;

namespace Stashbox.BuildUp.Expressions
{
    internal class ResolutionInfoParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression parameter;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node.Type == typeof(ResolutionInfo) ? base.VisitParameter(parameter) : node;
        }

        internal ResolutionInfoParameterVisitor(ParameterExpression parameter)
        {
            this.parameter = parameter;
        }
    }
}
