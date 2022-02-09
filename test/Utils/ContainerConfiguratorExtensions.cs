using FastExpressionCompiler;
using Stashbox.Tests.Utils;

namespace Stashbox.Configuration
{
    public static class ContainerConfiguratorExtensions
    {
        public static void WithCompiler(this ContainerConfigurator configurator, CompilerType compilerType)
        {
            switch (compilerType)
            {
                case CompilerType.Microsoft:
                    configurator.WithExpressionCompiler(Rules.ExpressionCompilers.MicrosoftExpressionCompiler);
                    break;
                case CompilerType.Stashbox:
                    configurator.WithExpressionCompiler(Rules.ExpressionCompilers.StashboxExpressionCompiler);
                    break;
                case CompilerType.FastExpressionCompiler:
                    configurator.WithExpressionCompiler(lambda => lambda.CompileFast());
                    break;
            }
        }
    }
}
