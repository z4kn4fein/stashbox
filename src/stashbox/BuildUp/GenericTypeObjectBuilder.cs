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

        public object BuildInstance(IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        {
            if (!builderContext.RegistrationRepository.ConstainsTypeKeyWithoutGenericDefinitionExtraction(resolutionInfo.ResolveType.Type))
            {
                lock (this.syncObject)
                {
                    if (!builderContext.RegistrationRepository.ConstainsTypeKeyWithoutGenericDefinitionExtraction(resolutionInfo.ResolveType.Type))
                    {
                        var genericType = this.metaInfoProvider.TypeTo.MakeGenericType(resolutionInfo.ResolveType.Type.GenericTypeArguments);
                        builderContext.Container.RegisterType(genericType, resolutionInfo.ResolveType.Type);
                    }
                }
            }

            return builderContext.Container.Resolve(resolutionInfo.ResolveType.Type);
        }

        public void CleanUp()
        {
        }
    }
}
