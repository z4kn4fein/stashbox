using Stashbox.Entity;
using Stashbox.Resolution;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class ParentContainerResolver : IMultiServiceResolver
    {
        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.Container.ParentContainer != null && containerContext.Container.ParentContainer.CanResolve(typeInfo.Type, typeInfo.DependencyName);

        public Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var resolution = resolutionContext.ChildContext == null
                ? resolutionContext.Clone(containerContext)
                : resolutionContext.Clone(resolutionContext.ChildContext);

            var result = containerContext.Container.ParentContainer.ContainerContext.ResolutionStrategy
                .BuildResolutionExpression(containerContext.Container.ParentContainer.ContainerContext, resolution, typeInfo, null);

            foreach (var definedVariable in resolution.DefinedVariables.Repository)
                resolutionContext.AddDefinedVariable(definedVariable.Key, definedVariable.Value);

            foreach (var instruction in resolution.SingleInstructions)
                resolutionContext.AddInstruction(instruction);

            return result;
        }

        public Expression[] GetExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var resolution = resolutionContext.ChildContext == null
                ? resolutionContext.Clone(containerContext)
                : resolutionContext.Clone(resolutionContext.ChildContext);

            var result = containerContext.Container.ParentContainer.ContainerContext.ResolutionStrategy
                .BuildResolutionExpressions(containerContext.Container.ParentContainer.ContainerContext, resolution, typeInfo);

            foreach (var definedVariable in resolution.DefinedVariables.Repository)
                resolutionContext.AddDefinedVariable(definedVariable.Key, definedVariable.Value);

            foreach (var instruction in resolution.SingleInstructions)
                resolutionContext.AddInstruction(instruction);

            return result;
        }
    }
}
