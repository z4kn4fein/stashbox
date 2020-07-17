using Stashbox.Tests.Utils;

namespace Stashbox.Configuration
{
    public static class ContainerConfiguratorExtensions
    {
        public static void WithCompiler(this ContainerConfigurator configurator, CompilerType compilerType)
        {
            switch (compilerType)
            {
                case CompilerType.ForcedMicrosoft:
                    configurator.WithExpressionCompiler(Rules.ExpressionCompilers.MicrosoftExpressionCompiler);
                    break;
                case CompilerType.ForcedBuiltIn:
                    configurator.WithExpressionCompiler(Rules.ExpressionCompilers.StashboxExpressionCompiler);
                    break;
            }
        }
    }
}
