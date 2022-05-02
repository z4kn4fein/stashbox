﻿using Stashbox.Exceptions;
using Stashbox.Registration.ServiceRegistrations;
using Stashbox.Resolution;
using Stashbox.Resolution.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Expressions
{
    internal static partial class ExpressionFactory
    {
        private static IEnumerable<Expression> GetMemberExpressions(
            IEnumerable<MemberInfo> members,
            ServiceRegistration? serviceRegistration,
            ResolutionContext resolutionContext,
            Expression instance) =>
            from member in members
            let expression = GetMemberExpression(member, serviceRegistration, resolutionContext)
            where expression != null
            select instance.Member(member).AssignTo(expression);

        private static IEnumerable<MemberBinding> GetMemberBindings(
            IEnumerable<MemberInfo> members,
            ServiceRegistration? serviceRegistration,
            ResolutionContext resolutionContext) =>
            from member in members
            let expression = GetMemberExpression(member, serviceRegistration, resolutionContext)
            where expression != null
            select member.AssignTo(expression);


        private static Expression GetMemberExpression(
            MemberInfo member,
            ServiceRegistration? serviceRegistration,
            ResolutionContext resolutionContext)
        {
            var memberTypeInfo = member.AsTypeInformation(serviceRegistration,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            var injectionParameter = (serviceRegistration as ComplexRegistration)?.InjectionParameters?.SelectInjectionParameterOrDefault(memberTypeInfo);
            if (injectionParameter != null) return injectionParameter;

            var serviceContext = resolutionContext.CurrentContainerContext
                .ResolutionStrategy.BuildExpressionForType(resolutionContext, memberTypeInfo);

            if (!serviceContext.IsEmpty() || resolutionContext.NullResultAllowed) return serviceContext.ServiceExpression;

            var memberType = member is PropertyInfo ? "property" : "field";
            throw new ResolutionFailedException(memberTypeInfo.ParentType, memberTypeInfo.DependencyName,
                $"Unresolvable {memberType}: ({memberTypeInfo.Type.FullName}){memberTypeInfo.ParameterOrMemberName}.");

        }
    }
}
