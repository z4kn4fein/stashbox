using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions
{
    internal class MemberExpressionBuilder : IMemberExpressionBuilder
    {
        private readonly IResolutionStrategy resolutionStrategy;

        public MemberExpressionBuilder(IResolutionStrategy resolutionStrategy)
        {
            this.resolutionStrategy = resolutionStrategy;
        }

        public IEnumerable<Expression> GetMemberExpressions(
            IContainerContext containerContext,
            IEnumerable<MemberInfo> members,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Expression instance) =>
            from member in members
            let expression = this.GetMemberExpression(member, instance.Type, registrationContext, containerContext, resolutionContext)
            where expression != null
            select instance.Member(member).AssignTo(expression);

        public IEnumerable<MemberBinding> GetMemberBindings(
            IContainerContext containerContext,
            IEnumerable<MemberInfo> members,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Type implementationType) =>
            from member in members
            let expression = this.GetMemberExpression(member, implementationType, registrationContext, containerContext, resolutionContext)
            where expression != null
            select member.AssignTo(expression);


        private Expression GetMemberExpression(
            MemberInfo member,
            Type implementationType,
            RegistrationContext registrationContext,
            IContainerContext containerContext,
            ResolutionContext resolutionContext)
        {
            var memberTypeInfo = member.AsTypeInformation(implementationType, registrationContext, containerContext.ContainerConfiguration);
            var memberExpression = this.resolutionStrategy
                .BuildResolutionExpression(containerContext, resolutionContext,
                    memberTypeInfo, registrationContext.InjectionParameters);

            switch (memberExpression)
            {
                case null when !resolutionContext.NullResultAllowed:
                    var memberType = member is PropertyInfo ? "property" : "field";
                    throw new ResolutionFailedException(memberTypeInfo.ParentType,
                        $"Unresolvable {memberType}: ({memberTypeInfo.Type.FullName}){memberTypeInfo.ParameterOrMemberName}.");
                case ConstantExpression constant when constant.Value == null:
                    return null;
                default:
                    return memberExpression;
            }
        }
    }
}
