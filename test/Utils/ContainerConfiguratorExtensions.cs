using FastExpressionCompiler;
using Stashbox.Tests.Utils;

namespace Stashbox.Configuration;

public static class ContainerConfiguratorExtensions
{
    public static ContainerConfigurator WithCompiler(this ContainerConfigurator configurator, CompilerType compilerType)
    {
        return compilerType switch
        {
            CompilerType.Microsoft => configurator.WithExpressionCompiler(Rules.ExpressionCompilers.MicrosoftExpressionCompiler),
            CompilerType.Stashbox => configurator.WithExpressionCompiler(Rules.ExpressionCompilers.StashboxExpressionCompiler),
            CompilerType.FastExpressionCompiler => configurator.WithExpressionCompiler(lambda => lambda.CompileFast()),
            _ => configurator,
        };
    }
}