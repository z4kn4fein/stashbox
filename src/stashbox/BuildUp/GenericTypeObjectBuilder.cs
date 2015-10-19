using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.BuildUp
{
    internal class GenericTypeObjectBuilder : IObjectBuilder
    {
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly object syncObject = new object();

        public GenericTypeObjectBuilder(IMetaInfoProvider metaInfoProvider)
        {
            this.metaInfoProvider = metaInfoProvider;
        }

        public object BuildInstance(IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
            if (!containerContext.RegistrationRepository.ConstainsTypeKeyWithConditionsWithoutGenericDefinitionExtraction(resolutionInfo.ResolveType))
            {
                lock (this.syncObject)
                {
                    if (!containerContext.RegistrationRepository.ConstainsTypeKeyWithConditionsWithoutGenericDefinitionExtraction(resolutionInfo.ResolveType))
                    {
                        var genericType = this.metaInfoProvider.TypeTo.MakeGenericType(resolutionInfo.ResolveType.Type.GenericTypeArguments);
                        containerContext.Container.RegisterType(resolutionInfo.ResolveType.Type, genericType);
                    }
                }
            }

            return containerContext.Container.Resolve(resolutionInfo.ResolveType.Type);
        }

        public void CleanUp()
        {
        }
    }
}
