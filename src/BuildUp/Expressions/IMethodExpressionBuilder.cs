using Stashbox.Registration;
using Stashbox.Resolution;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions
{
    internal interface IMethodExpressionBuilder
    {
        IEnumerable<Expression> CreateParameterExpressionsForMethod(
            IContainerContext containerContext,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            MethodBase method);

        ConstructorInfo SelectConstructor(
            IContainerContext containerContext,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            ConstructorInfo[] constructors,
            out Expression[] parameterExpressions);

        IEnumerable<Expression> CreateMethodExpressions(
            IContainerContext containerContext,
            IEnumerable<MethodInfo> methods,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Expression instance);
    }
}
