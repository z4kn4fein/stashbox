using System;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;

namespace Stashbox.BuildUp.Resolution
{
    internal class UnknownTypeResolver : Resolver
    {
        private readonly MethodInfo resolverMethodInfo;

        public UnknownTypeResolver(IContainerContext containerContext, TypeInformation typeInfo) 
            :base(containerContext, typeInfo)
        {
        }

        public override Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            base.BuilderContext.Container.RegisterType(base.TypeInfo.Type, base.TypeInfo.Type, base.TypeInfo.DependencyName);
            var reg = base.BuilderContext.RegistrationRepository.GetRegistrationOrDefault(base.TypeInfo);
            if(reg == null)
                throw new ResolutionFailedException(base.TypeInfo.Type.FullName);

            return reg.GetExpression(resolutionInfo, base.TypeInfo);
        }
    }
}
