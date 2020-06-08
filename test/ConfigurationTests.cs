using Stashbox.Configuration;
using Xunit;

namespace Stashbox.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void Ensure_Unknown_Type_Configurations_Working()
        {
            using var container = new StashboxContainer(c => c.WithUnknownTypeResolution());
            Assert.True(container.ContainerContext.ContainerConfiguration.UnknownTypeResolutionEnabled);
            Assert.Null(container.ContainerContext.ContainerConfiguration.UnknownTypeConfigurator);

            container.Configure(c => c.WithUnknownTypeResolution(co => { }));
            Assert.True(container.ContainerContext.ContainerConfiguration.UnknownTypeResolutionEnabled);
            Assert.NotNull(container.ContainerContext.ContainerConfiguration.UnknownTypeConfigurator);

            container.Configure(c => c.WithUnknownTypeResolution(enabled: false));
            Assert.False(container.ContainerContext.ContainerConfiguration.UnknownTypeResolutionEnabled);
        }

        [Fact]
        public void Ensure_Auto_Member_Injection_Configurations_Working()
        {
            using var container = new StashboxContainer(c => c.WithAutoMemberInjection());
            Assert.True(container.ContainerContext.ContainerConfiguration.AutoMemberInjectionEnabled);
            Assert.Equal(Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter, container.ContainerContext.ContainerConfiguration.AutoMemberInjectionRule);
            Assert.Null(container.ContainerContext.ContainerConfiguration.AutoMemberInjectionFilter);

            container.Configure(c => c.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields));
            Assert.True(container.ContainerContext.ContainerConfiguration.AutoMemberInjectionEnabled);
            Assert.Equal(Rules.AutoMemberInjectionRules.PrivateFields, container.ContainerContext.ContainerConfiguration.AutoMemberInjectionRule);
            Assert.Null(container.ContainerContext.ContainerConfiguration.AutoMemberInjectionFilter);

            container.Configure(c => c.WithAutoMemberInjection(filter: m => true));
            Assert.True(container.ContainerContext.ContainerConfiguration.AutoMemberInjectionEnabled);
            Assert.Equal(Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter, container.ContainerContext.ContainerConfiguration.AutoMemberInjectionRule);
            Assert.NotNull(container.ContainerContext.ContainerConfiguration.AutoMemberInjectionFilter);

            container.Configure(c => c.WithAutoMemberInjection(enabled: false));
            Assert.False(container.ContainerContext.ContainerConfiguration.AutoMemberInjectionEnabled);
        }

        [Fact]
        public void Ensure_Feature_Configurations_Working()
        {
            using var container = new StashboxContainer(c => c.WithCircularDependencyWithLazy());
            Assert.True(container.ContainerContext.ContainerConfiguration.CircularDependenciesWithLazyEnabled);

            container.Configure(c => c.WithCircularDependencyWithLazy(false));
            Assert.False(container.ContainerContext.ContainerConfiguration.CircularDependenciesWithLazyEnabled);


            container.Configure(c => c.WithDefaultValueInjection());
            Assert.True(container.ContainerContext.ContainerConfiguration.DefaultValueInjectionEnabled);

            container.Configure(c => c.WithDefaultValueInjection(false));
            Assert.False(container.ContainerContext.ContainerConfiguration.DefaultValueInjectionEnabled);


            container.Configure(c => c.WithDisposableTransientTracking());
            Assert.True(container.ContainerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled);

            container.Configure(c => c.WithDisposableTransientTracking(false));
            Assert.False(container.ContainerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled);


            container.Configure(c => c.WithLifetimeValidation());
            Assert.True(container.ContainerContext.ContainerConfiguration.LifetimeValidationEnabled);

            container.Configure(c => c.WithLifetimeValidation(false));
            Assert.False(container.ContainerContext.ContainerConfiguration.LifetimeValidationEnabled);


            container.Configure(c => c.WithExpressionCompiler(Rules.ExpressionCompilers.MicrosoftExpressionCompiler));
            Assert.NotNull(container.ContainerContext.ContainerConfiguration.ExternalExpressionCompiler);

            container.Configure(c => c.WithExpressionCompiler(null));
            Assert.Null(container.ContainerContext.ContainerConfiguration.ExternalExpressionCompiler);


            container.Configure(c => c.WithNamedDependencyResolutionForUnNamedRequests());
            Assert.True(container.ContainerContext.ContainerConfiguration.NamedDependencyResolutionForUnNamedRequestsEnabled);

            container.Configure(c => c.WithNamedDependencyResolutionForUnNamedRequests(false));
            Assert.False(container.ContainerContext.ContainerConfiguration.NamedDependencyResolutionForUnNamedRequestsEnabled);


            container.Configure(c => c.TreatParameterAndMemberNameAsDependencyName());
            Assert.True(container.ContainerContext.ContainerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled);

            container.Configure(c => c.TreatParameterAndMemberNameAsDependencyName(false));
            Assert.False(container.ContainerContext.ContainerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled);
        }

        [Fact]
        public void Ensure_Configuration_Change_Does_Not_Affect_Parent()
        {
            using var container = new StashboxContainer(c => c.WithLifetimeValidation());
            Assert.True(container.ContainerContext.ContainerConfiguration.LifetimeValidationEnabled);

            using var child = container.CreateChildContainer();
            child.Configure(c => c.WithLifetimeValidation(false));

            Assert.True(container.ContainerContext.ContainerConfiguration.LifetimeValidationEnabled);
            Assert.False(child.ContainerContext.ContainerConfiguration.LifetimeValidationEnabled);
        }
    }
}
