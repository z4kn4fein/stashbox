using Stashbox.Entity;
using Xunit;

namespace Stashbox.Tests.IssueTests
{

    public class UnableToUseNullableTypesWithInjectionParameters
    {
        [Fact]
        public void Unable_to_use_nullable_types_with_injection_parameters()
        {
            var container = new StashboxContainer(config => config
                .WithDefaultValueInjection()
                .WithCircularDependencyTracking());

            var someInt = 123;

            var inst = container.Register<IExample, ExampleClass>(
                config => config.WithInjectionParameters(
                    new InjectionParameter
                    {
                        Name = "exampleValue",
                        Value = someInt
                    })).Resolve<IExample>();

            Assert.NotNull(inst);
            Assert.IsType<ExampleClass>(inst);
            Assert.Equal(someInt, inst.ExampleProperty.Value);
        }

        [Fact]
        public void Unable_to_use_nullable_types_with_injection_parameters_member()
        {
            var container = new StashboxContainer(config => config
                .WithDefaultValueInjection()
                .WithMemberInjectionWithoutAnnotation()
                .WithCircularDependencyTracking());

            var someInt = 123;

            var inst = container.Register<IExample, ExampleClass2>(
                config => config.WithInjectionParameters(
                    new InjectionParameter
                    {
                        Name = "ExampleProperty",
                        Value = someInt
                    })).Resolve<IExample>();

            Assert.NotNull(inst);
            Assert.IsType<ExampleClass2>(inst);
            Assert.Equal(someInt, inst.ExampleProperty.Value);
        }

        interface IExample
        {
            int? ExampleProperty { get; }
        }

        class ExampleClass : IExample
        {
            public ExampleClass(int? exampleValue = null)
            {
                ExampleProperty = exampleValue;
            }

            public int? ExampleProperty { get; private set; }
        }

        class ExampleClass2 : IExample
        {
            public int? ExampleProperty { get; set; }
        }
    }
}
