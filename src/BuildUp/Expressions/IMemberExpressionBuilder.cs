using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions
{
    internal interface IMemberExpressionBuilder
    {
        IEnumerable<Expression> GetMemberExpressions(
            IContainerContext containerContext,
            MemberInfo[] injectionMembers,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Expression instance);

        IEnumerable<MemberBinding> GetMemberBindings(
            IContainerContext containerContext,
            MemberInfo[] injectionMembers,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Type implementationType);
    }
}
