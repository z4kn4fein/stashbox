using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Entity;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class UnableToUseNullableTypesWithInjectionParameters
    {
        [TestMethod]
        public void Unable_to_use_nullable_types_with_injection_parameters()
        {
            var container = new StashboxContainer(config => config
                .WithOptionalAndDefaultValueInjection()
                .WithCircularDependencyTracking());

            var someInt = 123;

            var inst = container.Register<IExample, ExampleClass>(
                config => config.WithInjectionParameters(
                    new InjectionParameter
                    {
                        Name = "exampleValue",
                        Value = someInt
                    })).Resolve<IExample>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(ExampleClass));
            Assert.AreEqual(someInt, inst.ExampleProperty.Value);
        }

        [TestMethod]
        public void Unable_to_use_nullable_types_with_injection_parameters_member()
        {
            var container = new StashboxContainer(config => config
                .WithOptionalAndDefaultValueInjection()
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

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(ExampleClass2));
            Assert.AreEqual(someInt, inst.ExampleProperty.Value);
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
