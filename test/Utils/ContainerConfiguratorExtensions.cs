using FastExpressionCompiler;
using Stashbox.Tests.Utils;

namespace Stashbox.Configuration
{
    public static class ContainerConfiguratorExtensions
    {
        public static ContainerConfigurator WithCompiler(this ContainerConfigurator configurator, CompilerType compilerType)
        {
            switch (compilerType)
            {
                case CompilerType.Microsoft:
                    return configurator.WithExpressionCompiler(Rules.ExpressionCompilers.MicrosoftExpressionCompiler);
                case CompilerType.Stashbox:
                    return configurator.WithExpressionCompiler(Rules.ExpressionCompilers.StashboxExpressionCompiler);
                case CompilerType.FastExpressionCompiler:
                    return configurator.WithExpressionCompiler(lambda => lambda.CompileFast());
            }

            return configurator;
        }
    }
}
