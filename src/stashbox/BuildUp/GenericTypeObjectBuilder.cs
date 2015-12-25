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

        public object BuildInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            if (containerContext.RegistrationRepository.ConstainsTypeKeyWithConditionsWithoutGenericDefinitionExtraction(resolveType))
                return containerContext.Container.Resolve(resolveType.Type);
            lock (this.syncObject)
            {
                if (containerContext.RegistrationRepository.ConstainsTypeKeyWithConditionsWithoutGenericDefinitionExtraction(resolveType))
                    return containerContext.Container.Resolve(resolveType.Type);
                var genericType = this.metaInfoProvider.TypeTo.MakeGenericType(resolveType.Type.GenericTypeArguments);
                this.containerContext.Container.RegisterType(resolveType.Type, genericType);
            }

            return containerContext.Container.Resolve(resolveType.Type);
        }

        public Expression GetExpression(Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            var callExpression = Expression.Call(Expression.Constant(this), "BuildInstance", null, resolutionInfoExpression, Expression.Constant(resolveType));
            return Expression.Convert(callExpression, resolveType.Type);
        }

        public void CleanUp()
        {
        }
    }
}
