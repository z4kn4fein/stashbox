using Stashbox.Attributes;
using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Lifetime;
using Stashbox.Registration.ServiceRegistrations;
using Stashbox.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stashbox.Tests
{

    public class DecoratorTests
    {
        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Simple(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator1>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Simple2(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1>(typeof(TestDecorator1));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator1>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Simple3(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator(typeof(ITest1), typeof(TestDecorator1));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator1>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Simple_Lazy(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            var test = container.Resolve<Lazy<ITest1>>();

            Assert.NotNull(test.Value);
            Assert.IsType<TestDecorator1>(test.Value);

            Assert.NotNull(test.Value.Test);
            Assert.IsType<Test1>(test.Value.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Simple_Func(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            var test = container.Resolve<Func<ITest1>>();

            Assert.NotNull(test());
            Assert.IsType<TestDecorator1>(test());

            Assert.NotNull(test().Test);
            Assert.IsType<Test1>(test().Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Simple_Enumerable(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.Register<ITest1, Test11>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            var test = container.Resolve<IEnumerable<ITest1>>();

            Assert.NotNull(test);
            Assert.IsAssignableFrom<IEnumerable<ITest1>>(test);

            var arr = test.ToArray();

            Assert.NotNull(arr[0]);
            Assert.IsType<TestDecorator1>(arr[0]);

            Assert.NotNull(arr[1]);
            Assert.IsType<TestDecorator1>(arr[1]);

            Assert.NotNull(arr[0].Test);
            Assert.IsType<Test1>(arr[0].Test);

            Assert.NotNull(arr[1].Test);
            Assert.IsType<Test11>(arr[1].Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Decorator_Holds_Lazy(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator6>();
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator6>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Decorator_Holds_Func(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator7>();
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator7>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Decorator_Holds_Enumerable(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator8>();
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator8>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<TestDecorator1>(test.Test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Dependency(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator3>();
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator3>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Inject_Member_With_Config(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator3Attributeless>(config => config.WithDependencyBinding("Test"));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator3Attributeless>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Inject_Member_With_Config_Non_Generic_Implementation(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1>(typeof(TestDecorator3Attributeless), config => config.WithDependencyBinding("Test"));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator3Attributeless>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_AutoMemberInjection_Throw_When_Member_Unresolvable(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator4>(context => context.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess));
            Assert.Throws<ResolutionFailedException>(() => container.Resolve<ITest1>());
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_AutoMemberInjection_InjectionParameter(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator4>(context => context
                .WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess)
                .WithInjectionParameter("Name", "test"));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator4>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
            Assert.Equal("test", ((TestDecorator4)test).Name);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_AutoMemberInjection_InjectionParameter_Fluent(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator4>(context => context
                .WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess)
                .WithInjectionParameter("Name", "test"));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator4>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
            Assert.Equal("test", ((TestDecorator4)test).Name);
        }

        [Fact]
        public void DecoratorTests_ConstructorSelection_LeastParameters()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator5>(context => context.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator5>(test);

            Assert.Null(test.Test);
        }

        [Fact]
        public void DecoratorTests_ConstructorSelection_MostParameters()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator5>(context => context.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferMostParameters));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator5>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Multiple(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>();
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator2>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<TestDecorator1>(test.Test);

            Assert.NotNull(test.Test.Test);
            Assert.IsType<Test1>(test.Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Multiple_Scoped(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.RegisterScoped<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>();

            using var child = container.BeginScope();
            var test = child.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator2>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<TestDecorator1>(test.Test);

            Assert.NotNull(test.Test.Test);
            Assert.IsType<Test1>(test.Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_OpenGeneric(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register(typeof(ITest1<>), typeof(Test1<>));
            container.RegisterDecorator(typeof(ITest1<>), typeof(TestDecorator1<>));
            var test = container.Resolve<ITest1<int>>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator1<int>>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1<int>>(test.Test);
        }

        [Fact]
        public void DecoratorTests_DecoratorDependency_Null()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator9>();

            var inst = container.ResolveOrDefault<ITest1>();

            Assert.Null(inst);
        }

        [Fact]
        public void DecoratorTests_DecoreteeDependency_Null()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test12>();
            container.RegisterDecorator<ITest1, TestDecorator9>();

            var inst = container.ResolveOrDefault<ITest1>();

            Assert.Null(inst);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Disposed(CompilerType compilerType)
        {
            IDisp test;
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking().WithCompiler(compilerType)))
            {
                container.Register<IDisp, TestDisp>();
                container.RegisterDecorator<IDisp, TestDispDecorator>();

                test = container.Resolve<IDisp>();

                Assert.NotNull(test);
                Assert.NotNull(test.Test);
            }

            Assert.True(test.Disposed);
            Assert.True(test.Test.Disposed);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Disposed_OnlyDecoreteeDisposal(CompilerType compilerType)
        {
            IDisp test;
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking().WithCompiler(compilerType)))
            {
                container.Register<IDisp, TestDisp>(context => context.WithoutDisposalTracking());
                container.RegisterDecorator<IDisp, TestDispDecorator>();

                test = container.Resolve<IDisp>();

                Assert.NotNull(test);
                Assert.NotNull(test.Test);
            }

            Assert.True(test.Disposed);
            Assert.False(test.Test.Disposed);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Disposed_BothDisposal(CompilerType compilerType)
        {
            IDisp test;
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking().WithCompiler(compilerType)))
            {
                container.Register<IDisp, TestDisp>(context => context.WithoutDisposalTracking());
                container.RegisterDecorator<IDisp, TestDispDecorator>(context => context.WithoutDisposalTracking());

                test = container.Resolve<IDisp>();

                Assert.NotNull(test);
                Assert.NotNull(test.Test);
            }

            Assert.False(test.Disposed);
            Assert.False(test.Test.Disposed);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_ReplaceDecorator(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();

            var test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator1>(test);
            Assert.IsType<Test1>(test.Test);

            container.RegisterDecorator<ITest1, TestDecorator1>(context => context.ReplaceExisting());

            test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator1>(test);
            Assert.IsType<Test1>(test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_RemapDecorator(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>();

            var test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator2>(test);
            Assert.IsType<TestDecorator1>(test.Test);
            Assert.IsType<Test1>(test.Test.Test);

            container.ReMapDecorator<ITest1, TestDecorator3>();

            test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator3>(test);
            Assert.IsType<Test1>(test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_RemapDecorator_V2(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>();

            var test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator2>(test);
            Assert.IsType<TestDecorator1>(test.Test);
            Assert.IsType<Test1>(test.Test.Test);

            container.ReMapDecorator<ITest1>(typeof(TestDecorator3));

            test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator3>(test);
            Assert.IsType<Test1>(test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_RemapDecorator_WithConfig(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>();

            var test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator2>(test);
            Assert.IsType<TestDecorator1>(test.Test);
            Assert.IsType<Test1>(test.Test.Test);

            container.ReMapDecorator(typeof(ITest1), typeof(TestDecorator3), context => context.WithoutDisposalTracking());

            test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator3>(test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_Service_ImplementationType()
        {
            using var container = new StashboxContainer();
            container.RegisterDecorator<ITest1, TestDecorator1>(context =>
            {
                Assert.Equal(typeof(ITest1), context.ServiceType);
                Assert.Equal(typeof(TestDecorator1), context.ImplementationType);
            });
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Conditional(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>("t1");
            container.Register<ITest1, Test11>("t2");
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>(c => c.WhenDecoratedServiceIs<Test11>());

            var t1 = container.Resolve<ITest1>("t1");
            var t2 = container.Resolve<ITest1>("t2");

            Assert.IsType<TestDecorator1>(t1);
            Assert.IsType<Test1>(t1.Test);

            Assert.IsType<TestDecorator2>(t2);
            Assert.IsType<TestDecorator1>(t2.Test);
            Assert.IsType<Test11>(t2.Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Enumerable(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.Register<ITest1, Test11>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>(c => c.WhenDecoratedServiceIs<Test11>());

            var t = container.Resolve<ITest1[]>();

            Assert.IsType<TestDecorator1>(t[0]);
            Assert.IsType<Test1>(t[0].Test);

            Assert.IsType<TestDecorator2>(t[1]);
            Assert.IsType<TestDecorator1>(t[1].Test);
            Assert.IsType<Test11>(t[1].Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Conditional_Named(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>("t1");
            container.Register<ITest1, Test11>("t2");
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>(c => c.When(t => t.DependencyName.Equals("t2")));

            var t1 = container.Resolve<ITest1>("t1");
            var t2 = container.Resolve<ITest1>("t2");

            Assert.IsType<TestDecorator1>(t1);
            Assert.IsType<Test1>(t1.Test);

            Assert.IsType<TestDecorator2>(t2);
            Assert.IsType<TestDecorator1>(t2.Test);
            Assert.IsType<Test11>(t2.Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Conditional_Named_Short(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>("t1");
            container.Register<ITest1, Test11>("t2");
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>(c => c.WhenDecoratedServiceIs("t2"));

            var t1 = container.Resolve<ITest1>("t1");
            var t2 = container.Resolve<ITest1>("t2");

            Assert.IsType<TestDecorator1>(t1);
            Assert.IsType<Test1>(t1.Test);

            Assert.IsType<TestDecorator2>(t2);
            Assert.IsType<TestDecorator1>(t2.Test);
            Assert.IsType<Test11>(t2.Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Conditional_Parent(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithAutoMemberInjection().WithUnknownTypeResolution().WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>(c => c.When(t => t.ParentType == typeof(TestHolder2)));

            var t1 = container.Resolve<TestHolder1>();
            var t2 = container.Resolve<TestHolder2>();

            Assert.IsType<TestDecorator1>(t1.Test1);
            Assert.IsType<Test1>(t1.Test1.Test);

            Assert.IsType<TestDecorator2>(t2.Test1);
            Assert.IsType<TestDecorator1>(t2.Test1.Test);
            Assert.IsType<Test1>(t2.Test1.Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Conditional_Attribute(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithAutoMemberInjection().WithUnknownTypeResolution().WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>(c => c.WhenHas<Decorator1Attribute>());
            container.RegisterDecorator<ITest1, TestDecorator2>(c => c.WhenHas<Decorator2Attribute>());

            var t1 = container.Resolve<TestHolder1>();
            var t2 = container.Resolve<TestHolder2>();

            Assert.IsType<TestDecorator2>(t1.Test1);
            Assert.IsType<TestDecorator1>(t1.Test1.Test);
            Assert.IsType<Test1>(t1.Test1.Test.Test);

            Assert.IsType<TestDecorator2>(t2.Test1);
            Assert.IsType<Test1>(t2.Test1.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Conditional_Attribute_Multiple(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithAutoMemberInjection().WithUnknownTypeResolution().WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>(c => c.WhenHas<Decorator1Attribute>());
            container.RegisterDecorator<ITest1, TestDecorator2>(c => c.WhenHas<Decorator2Attribute>());
            container.RegisterDecorator<ITest1, TestDecorator3>(c => c.WhenHas<Decorator1Attribute>().WhenHas<Decorator3Attribute>());

            var t = container.Resolve<TestHolder3>();

            Assert.IsType<TestDecorator3>(t.Test1);
            Assert.IsType<TestDecorator2>(t.Test1.Test);
            Assert.IsType<Test1>(t.Test1.Test.Test);

            Assert.IsType<TestDecorator3>(t.Test11);
            Assert.IsType<TestDecorator1>(t.Test11.Test);
            Assert.IsType<Test1>(t.Test11.Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Conditional_Attribute_Multiple_Scoped(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithAutoMemberInjection().WithUnknownTypeResolution().WithCompiler(compilerType));
            container.RegisterScoped<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>(c => c.WhenHas<Decorator1Attribute>().WithTransientLifetime());
            container.RegisterDecorator<ITest1, TestDecorator2>(c => c.WhenHas<Decorator2Attribute>().WithTransientLifetime());
            container.RegisterDecorator<ITest1, TestDecorator3>(c => c.WhenHas<Decorator1Attribute>().WhenHas<Decorator3Attribute>().WithTransientLifetime());

            var t = container.Resolve<TestHolder3>();

            Assert.IsType<TestDecorator3>(t.Test1);
            Assert.IsType<TestDecorator2>(t.Test1.Test);
            Assert.IsType<Test1>(t.Test1.Test.Test);

            Assert.IsType<TestDecorator3>(t.Test11);
            Assert.IsType<TestDecorator1>(t.Test11.Test);
            Assert.IsType<Test1>(t.Test11.Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Different_Lifetime(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.RegisterSingleton<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>(c => c.WithScopedLifetime());

            using var scope1 = container.BeginScope();
            var t1 = scope1.Resolve<ITest1>();

            using var scope2 = scope1.BeginScope();
            var t2 = scope2.Resolve<ITest1>();

            Assert.NotSame(t1, t2);
            Assert.Same(t1.Test, t2.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Inheriting_Lifetime(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.RegisterSingleton<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();

            var t1 = container.Resolve<ITest1>();
            var t2 = container.Resolve<ITest1>();

            Assert.Same(t1, t2);
            Assert.Same(t1.Test, t2.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Different_Decoretees(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.Register<ITest2, T2>();
            container.RegisterDecorator<ITest2, TestDecorator10>();
            container.RegisterDecorator<ITest1, TestDecorator1>();

            var t = container.Resolve<ITest2>();

            Assert.IsType<TestDecorator10>(t);

            Assert.IsType<TestDecorator1>(((TestDecorator10)t).Test1);
            Assert.IsType<T2>(t.Test2);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Different_Decoretees_Indirect(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.Register<ITest2, T3>();
            container.RegisterDecorator<ITest2, TestDecorator11>();
            container.RegisterDecorator<ITest1, TestDecorator1>();

            var t = container.Resolve<ITest2>();

            Assert.IsType<TestDecorator11>(t);
            Assert.IsType<T3>(t.Test2);
            Assert.IsType<TestDecorator1>(((T3)t.Test2).Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Different_Decoretees_Indirect2(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest2, T2>();
            container.Register<ITest1, T4>();
            container.RegisterDecorator<ITest2, TestDecorator12>();
            container.RegisterDecorator<ITest2, TestDecorator11>();
            container.RegisterDecorator<ITest1, TestDecorator1>();

            var t = container.Resolve<ITest2>();

            Assert.IsType<TestDecorator11>(t);
            Assert.IsType<TestDecorator12>(t.Test2);
            Assert.IsType<T2>(t.Test2.Test2);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_NamedScope(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>(c => c.InNamedScope("A"));

            var t = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator1>(t);
            Assert.IsType<Test1>(t.Test);

            using var scope = container.BeginScope("A");
            t = scope.Resolve<ITest1>();

            Assert.IsType<TestDecorator2>(t);
            Assert.IsType<TestDecorator1>(t.Test);
            Assert.IsType<Test1>(t.Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_NamedScope_Different_Decoretee(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.Register<ITest1, Test11>(c => c.InNamedScope("A"));
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>(c => c.InNamedScope("A"));

            var t = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator1>(t);
            Assert.IsType<Test1>(t.Test);

            using var scope = container.BeginScope("A");
            t = scope.Resolve<ITest1>();

            Assert.IsType<TestDecorator2>(t);
            Assert.IsType<TestDecorator1>(t.Test);
            Assert.IsType<Test11>(t.Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Factory(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator(typeof(ITest1), typeof(TestDecorator3), c => c.WithFactory(() => new TestDecorator3()));
            container.RegisterDecorator(typeof(ITest1), typeof(TestDecorator13), c => c.WithFactory(r => new TestDecorator13 { Name = "T4" }));

            var t = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator13>(t);
            Assert.IsType<TestDecorator3>(t.Test);
            Assert.IsType<Test1>(t.Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Factory_Generic(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator3>(c => c.WithFactory(() => new TestDecorator3()));
            container.RegisterDecorator<ITest1, TestDecorator13>(c => c.WithFactory(r => new TestDecorator13 { Name = "T4" }));

            var t = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator13>(t);
            Assert.IsType<TestDecorator3>(t.Test);
            Assert.IsType<Test1>(t.Test.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Target_Attribute(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, TestService>("t1");
            container.Register<ITest1, Test1>("t2");
            container.RegisterDecorator<ITest1, TestDecorator1>(c => c.WhenDecoratedServiceHas<Decorator1Attribute>());
            container.RegisterDecorator<ITest1, TestDecorator2>();

            var t = container.Resolve<ITest1>("t1");

            Assert.IsType<TestDecorator2>(t);
            Assert.IsType<TestDecorator1>(t.Test);
            Assert.IsType<TestService>(t.Test.Test);

            t = container.Resolve<ITest1>("t2");

            Assert.IsType<TestDecorator2>(t);
            Assert.IsType<Test1>(t.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_InjectMember(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator14>();

            var t = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator14>(t);
            Assert.IsType<Test1>(t.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_InjectMember_AttributeLess(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator3Attributeless>(c => c.WithDependencyBinding(d => d.Test));

            var t = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator3Attributeless>(t);
            Assert.IsType<Test1>(t.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_WithFinalizer(CompilerType compilerType)
        {
            var finalized = false;
            {
                using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
                container.Register<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator1>(c => c.WithFinalizer(d => { finalized = true; }));
                container.Resolve<ITest1>();
            }

            Assert.True(finalized);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Factory_Param1(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>(c => c.WithFactory<ITest1>(t1 =>
            {
                Assert.IsType<Test1>(t1);
                return new TestDecorator1(t1);
            }));
            container.Resolve<ITest1>();
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Factory_Param_NextDecorator(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator2>();
            container.RegisterDecorator<ITest1, TestDecorator1>(c => c.WithFactory<ITest1>(t1 =>
            {
                Assert.IsType<TestDecorator2>(t1);
                return new TestDecorator1(t1);
            }));
            container.Resolve<ITest1>();
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Factory_Param2(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.Register<TComp>(c => c.AsImplementedTypes());
            container.RegisterDecorator<ITest1, TestDecorator1>(c => c.WithFactory<ITest1, IT1>((t1, t2) =>
            {
                Assert.IsType<Test1>(t1);
                Assert.IsType<TComp>(t2);
                return new TestDecorator1(t1);
            }));
            container.Resolve<ITest1>();
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Factory_Param3(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.Register<TComp>(c => c.AsImplementedTypes());
            container.RegisterDecorator<ITest1, TestDecorator1>(c => c.WithFactory<ITest1, IT1, IT2>((t1, t2, t3) =>
            {
                Assert.IsType<Test1>(t1);
                Assert.IsType<TComp>(t2);
                Assert.IsType<TComp>(t3);
                return new TestDecorator1(t1);
            }));
            container.Resolve<ITest1>();
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Factory_Param4(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.Register<TComp>(c => c.AsImplementedTypes());
            container.RegisterDecorator<ITest1, TestDecorator1>(c => c.WithFactory<ITest1, IT1, IT2, IT4>((t1, t2, t3, t4) =>
            {
                Assert.IsType<Test1>(t1);
                Assert.IsType<TComp>(t2);
                Assert.IsType<TComp>(t3);
                Assert.IsType<TComp>(t4);
                return new TestDecorator1(t1);
            }));
            container.Resolve<ITest1>();
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_Factory_Param5(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.Register<TComp>(c => c.AsImplementedTypes());
            container.RegisterDecorator<ITest1, TestDecorator1>(c => c.WithFactory<ITest1, IT1, IT2, IT4, IT5>((t1, t2, t3, t4, t5) =>
            {
                Assert.IsType<Test1>(t1);
                Assert.IsType<TComp>(t2);
                Assert.IsType<TComp>(t3);
                Assert.IsType<TComp>(t4);
                Assert.IsType<TComp>(t5);
                return new TestDecorator1(t1);
            }));
            container.Resolve<ITest1>();
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_NonGeneric_Factory_Param1(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator(typeof(ITest1), typeof(TestDecorator1), c => c.WithFactory<ITest1>(t1 =>
            {
                Assert.IsType<Test1>(t1);
                return new TestDecorator1(t1);
            }));
            container.Resolve<ITest1>();
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_NonGeneric_Factory_Param_NextDecorator(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator2>();
            container.RegisterDecorator(typeof(ITest1), typeof(TestDecorator1), c => c.WithFactory<ITest1>(t1 =>
            {
                Assert.IsType<TestDecorator2>(t1);
                return new TestDecorator1(t1);
            }));
            container.Resolve<ITest1>();
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_NonGeneric_Factory_Param2(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.Register<TComp>(c => c.AsImplementedTypes());
            container.RegisterDecorator(typeof(ITest1), typeof(TestDecorator1), c => c.WithFactory<ITest1, IT1>((t1, t2) =>
            {
                Assert.IsType<Test1>(t1);
                Assert.IsType<TComp>(t2);
                return new TestDecorator1(t1);
            }));
            container.Resolve<ITest1>();
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_NonGeneric_Factory_Param3(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.Register<TComp>(c => c.AsImplementedTypes());
            container.RegisterDecorator(typeof(ITest1), typeof(TestDecorator1), c => c.WithFactory<ITest1, IT1, IT2>((t1, t2, t3) =>
            {
                Assert.IsType<Test1>(t1);
                Assert.IsType<TComp>(t2);
                Assert.IsType<TComp>(t3);
                return new TestDecorator1(t1);
            }));
            container.Resolve<ITest1>();
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_NonGeneric_Factory_Param4(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.Register<TComp>(c => c.AsImplementedTypes());
            container.RegisterDecorator(typeof(ITest1), typeof(TestDecorator1), c => c.WithFactory<ITest1, IT1, IT2, IT4>((t1, t2, t3, t4) =>
            {
                Assert.IsType<Test1>(t1);
                Assert.IsType<TComp>(t2);
                Assert.IsType<TComp>(t3);
                Assert.IsType<TComp>(t4);
                return new TestDecorator1(t1);
            }));
            container.Resolve<ITest1>();
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void DecoratorTests_NonGeneric_Factory_Param5(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest1, Test1>();
            container.Register<TComp>(c => c.AsImplementedTypes());
            container.RegisterDecorator(typeof(ITest1), typeof(TestDecorator1), c => c.WithFactory<ITest1, IT1, IT2, IT4, IT5>((t1, t2, t3, t4, t5) =>
            {
                Assert.IsType<Test1>(t1);
                Assert.IsType<TComp>(t2);
                Assert.IsType<TComp>(t3);
                Assert.IsType<TComp>(t4);
                Assert.IsType<TComp>(t5);
                return new TestDecorator1(t1);
            }));
            container.Resolve<ITest1>();
        }

        [Fact]
        public void DecoratorTests_Compositor_Works()
        {
            using var container = new StashboxContainer();
            container.RegisterDecorator<ITest1, TestDecorator1>(c => c
                .WithInitializer((d, r) => { })
                .WithSingletonLifetime()
                .WhenDecoratedServiceIs<Test1>());

            var registration = container.ContainerContext.DecoratorRepository.GetRegistrationMappings().First().Value as ComplexRegistration;

            Assert.NotNull(registration.Initializer);
            Assert.Equal(Lifetimes.Singleton, registration.Lifetime);
        }

        interface IT1 { }

        interface IT2 { }

        interface IT3 { }

        interface IT4 { }

        interface IT5 { }

        class TComp : IT1, IT2, IT3, IT4, IT5 { }


        interface ITest1 { ITest1 Test { get; } }

        interface ITest2 { ITest2 Test2 { get; } }

        interface IDecoratorDep { }

        interface IDep { }

        interface ITest1<T> { ITest1<T> Test { get; } }

        interface IDisp : IDisposable
        {
            IDisp Test { get; }

            bool Disposed { get; }
        }

        class Test1 : ITest1
        {
            public ITest1 Test { get; }
        }

        class T2 : ITest2
        {
            public ITest2 Test2 { get; }
        }

        class T3 : ITest2
        {
            public ITest2 Test2 { get; }

            [Dependency]
            public ITest1 Test { get; set; }
        }

        class T4 : ITest1
        {
            [Dependency]
            public ITest2 Test2 { get; set; }

            public ITest1 Test { get; set; }
        }

        class Test11 : ITest1
        {
            public ITest1 Test { get; }
        }

        class Test12 : ITest1
        {
            public ITest1 Test { get; }

            public Test12(IDep dep)
            {

            }
        }

        class TestDisp : IDisp
        {
            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException(nameof(TestDisp));

                this.Disposed = true;
            }

            public bool Disposed { get; private set; }
            public IDisp Test { get; }
        }

        class TestDispDecorator : IDisp
        {
            public TestDispDecorator(IDisp disp)
            {
                this.Test = disp;
            }

            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException(nameof(TestDisp));

                this.Disposed = true;
            }

            public bool Disposed { get; private set; }
            public IDisp Test { get; }
        }

        class Test1<T> : ITest1<T>
        {
            public ITest1<T> Test { get; }
        }

        class TestDecorator1<T> : ITest1<T>
        {
            public ITest1<T> Test { get; }

            public TestDecorator1(ITest1<T> test1)
            {
                this.Test = test1;
            }
        }

        class TestDecorator1 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator1(ITest1 test1)
            {
                this.Test = test1;
            }
        }

        class TestDecorator2 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator2(ITest1 test1)
            {
                this.Test = test1;
            }
        }

        class TestDecorator3 : ITest1
        {
            [Dependency]
            public ITest1 Test { get; set; }
        }

        class TestDecorator14 : ITest1
        {
            [Dependency]
            public ITest1 Test { get; set; }
        }

        class TestDecorator3Attributeless : ITest1
        {
            public ITest1 Test { get; set; }
        }

        class TestDecorator4 : ITest1
        {
            public string Name { get; private set; }

            public ITest1 Test { get; private set; }
        }

        class TestDecorator5 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator5()
            { }

            public TestDecorator5(ITest1 test1)
            {
                this.Test = test1;
            }
        }

        class TestDecorator6 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator6(Lazy<ITest1> test1)
            {
                this.Test = test1.Value;
            }
        }

        class TestDecorator7 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator7(Func<ITest1> test1)
            {
                this.Test = test1();
            }
        }

        class TestDecorator8 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator8(IEnumerable<ITest1> test1)
            {
                this.Test = test1.First();
            }
        }

        class TestDecorator9 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator9(ITest1 test1, IDecoratorDep dep)
            {
                this.Test = test1;
            }
        }

        class TestDecorator10 : ITest2
        {
            public ITest2 Test2 { get; }
            public ITest1 Test1 { get; }

            public TestDecorator10(ITest2 test2, ITest1 test1)
            {
                this.Test2 = test2;
                this.Test1 = test1;
            }
        }

        class TestDecorator11 : ITest2
        {
            public ITest2 Test2 { get; }

            public TestDecorator11(ITest2 test2)
            {
                this.Test2 = test2;
            }
        }

        class TestDecorator12 : ITest2
        {
            public ITest2 Test2 { get; }

            public TestDecorator12(ITest1 test1)
            {
                this.Test2 = ((T4)test1.Test).Test2;
            }
        }

        class TestDecorator13 : ITest1
        {
            public string Name { get; set; }

            [Dependency]
            public ITest1 Test { get; set; }
        }

        class TestHolder1
        {
            [Decorator1, Decorator2]
            public ITest1 Test1 { get; set; }
        }

        class TestHolder2
        {
            [Decorator2]
            public ITest1 Test1 { get; set; }
        }

        class TestHolder3
        {
            [Decorator2, Decorator3]
            public ITest1 Test1 { get; set; }

            [Decorator1]
            public ITest1 Test11 { get; set; }
        }

        class Decorator1Attribute : Attribute { }

        class Decorator2Attribute : Attribute { }

        class Decorator3Attribute : Attribute { }

        [Decorator1]
        class TestService : ITest1
        {
            public ITest1 Test { get; }
        }
    }
}
