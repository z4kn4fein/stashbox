using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class GenericTypeObjectBuilder : IObjectBuilder
    {
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IContainerContext containerContext;
        private readonly object syncObject = new object();

        public GenericTypeObjectBuilder(IContainerContext containerContext, IMetaInfoProvider metaInfoProvider)
        {
            this.metaInfoProvider = metaInfoProvider;
            this.containerContext = containerContext;
        }

        public object BuildInstance(ResolutionInfo resolutionInfo)
        {
            if (!containerContext.RegistrationRepository.ConstainsTypeKeyWithConditionsWithoutGenericDefinitionExtraction(resolutionInfo.ResolveType))
            {
                lock (this.syncObject)
                {
                    if (!containerContext.RegistrationRepository.ConstainsTypeKeyWithConditionsWithoutGenericDefinitionExtraction(resolutionInfo.ResolveType))
                    {
                        var genericType = this.metaInfoProvider.TypeTo.MakeGenericType(resolutionInfo.ResolveType.Type.GenericTypeArguments);
                        this.containerContext.Container.RegisterType(resolutionInfo.ResolveType.Type, genericType);
                    }
                }
            }

            return containerContext.Container.Resolve(resolutionInfo.ResolveType.Type);
        }

        public void CleanUp()
        {
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            return Expression.Constant(this.BuildInstance(resolutionInfo));
        }
    }
}
