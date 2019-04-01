using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;

namespace Stashbox.BuildUp.Expressions
{
    internal interface IConstructorSelector
    {
        ResolutionConstructor CreateResolutionConstructor(IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            ConstructorInformation constructor);

        ResolutionConstructor SelectConstructor(
            Type implementationType,
            IContainerContext containerContext,
            ResolutionContext resolutionContext,
            ConstructorInformation[] constructors,
            IEnumerable<InjectionParameter> injectionParameters);
    }
}
