using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
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
            MemberInfo[] injectionMembers,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Expression instance)
        {
            var length = injectionMembers.Length;
            for (var i = 0; i < length; i++)
            {
                var member = injectionMembers[i];
                var expression = this.GetMemberExpression(member, instance.Type, registrationContext,
                    containerContext, resolutionContext);

                if (expression == null) continue;

                yield return instance.Member(member).AssignTo(expression);
            }
        }

        public IEnumerable<MemberBinding> GetMemberBindings(
            IContainerContext containerContext,
            MemberInfo[] injectionMembers,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Type implementationType)
        {
            var length = injectionMembers.Length;
            for (var i = 0; i < length; i++)
            {
                var info = injectionMembers[i];
                var expression = this.GetMemberExpression(info, implementationType, registrationContext,
                    containerContext, resolutionContext);

                if (expression == null) continue;

                yield return info.AssignTo(expression);
            }
        }

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

            if (memberExpression == null && !resolutionContext.NullResultAllowed)
                throw new ResolutionFailedException(memberTypeInfo.ParentType,
                    $"Unresolvable member: ({memberTypeInfo.Type.FullName}){memberTypeInfo.ParameterOrMemberName}.");


            if (memberExpression is ConstantExpression constant && constant.Value == null)
                return null;

            return memberExpression;
        }
    }
}
