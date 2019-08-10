using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class ParentContainerResolver : IMultiServiceResolver
    {
        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.Container.ParentContainer != null &&
            containerContext.Container.ParentContainer.CanResolve(typeInfo.Type, typeInfo.DependencyName);

        public Expression GetExpression(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var resolution = resolutionContext.ChildContext == null
                ? resolutionContext.Clone(containerContext)
                : resolutionContext.Clone(resolutionContext.ChildContext);

            var result = resolutionStrategy
                .BuildResolutionExpression(containerContext.Container.ParentContainer.ContainerContext, resolution, typeInfo);

            foreach (var definedVariable in resolution.DefinedVariables.Repository)
                resolutionContext.AddDefinedVariable(definedVariable.Key, definedVariable.Value);

            foreach (var instruction in resolution.SingleInstructions)
                resolutionContext.AddInstruction(instruction);

            return result;
        }

        public Expression[] GetAllExpressions(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var resolution = resolutionContext.ChildContext == null
                ? resolutionContext.Clone(containerContext)
                : resolutionContext.Clone(resolutionContext.ChildContext);

            var result = resolutionStrategy
                .BuildAllResolutionExpressions(containerContext.Container.ParentContainer.ContainerContext, resolution, typeInfo);

            foreach (var definedVariable in resolution.DefinedVariables.Repository)
                resolutionContext.AddDefinedVariable(definedVariable.Key, definedVariable.Value);

            foreach (var instruction in resolution.SingleInstructions)
                resolutionContext.AddInstruction(instruction);

            return result;
        }
    }
}
