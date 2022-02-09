using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stashbox.Tests
{
    public class KeyValueTests
    {
        [Fact]
        public void KeyValueTests_NotFound()
        {
            var container = new StashboxContainer();
            container.Register<IT, A>("A");

            Assert.Throws<ResolutionFailedException>(() => container.Resolve<KeyValuePair<string, IT>>("A"));
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void KeyValueTests_Resolve(CompilerType compilerType)
        {
            var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<IT, A>("A");
            container.Register<IT, B>("B");
            var a = container.Resolve<KeyValuePair<object, IT>>("A");

            Assert.IsType<A>(a.Value);
            Assert.Equal("A", a.Key);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void KeyValueTests_Resolve_Wrapped(CompilerType compilerType)
        {
            var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<IT, A>("A");
            container.Register<IT, B>("B");
            var a = container.Resolve<KeyValuePair<object, Lazy<IT>>>("A");

            Assert.IsType<A>(a.Value.Value);
            Assert.Equal("A", a.Key);
        }
        
        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void KeyValueTests_Resolve_Wrapped_Enumerable(CompilerType compilerType)
        {
            var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<IT, A>("A");
            container.Register<IT, B>("B");
            var a = container.Resolve<KeyValuePair<object, IT[]>>();

            Assert.IsType<A>(a.Value[0]);
            Assert.IsType<B>(a.Value[1]);
            Assert.Null(a.Key);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void KeyValueTests_Resolve_Enumerable(CompilerType compilerType)
        {
            var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<IT, A>("A");
            container.Register<IT, B>("B");
            var values = container.Resolve<KeyValuePair<object, IT>[]>();

            Assert.Equal(2, values.Length);
            Assert.IsType<A>(values[0].Value);
            Assert.Equal("A", values[0].Key);
            Assert.IsType<B>(values[1].Value);
            Assert.Equal("B", values[1].Key);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void KeyValueTests_Resolve_Enumerable_Includes_Non_Named(CompilerType compilerType)
        {
            var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<IT, A>("A");
            container.Register<IT, B>();
            var values = container.Resolve<KeyValuePair<object, IT>[]>();

            Assert.Equal(2, values.Length);
            Assert.IsType<A>(values[0].Value);
            Assert.Equal("A", values[0].Key);
            Assert.IsType<B>(values[1].Value);
            Assert.Null(values[1].Key);
        }

        [Fact]
        public void ReadOnlyKeyValueTests_NotFound()
        {
            var container = new StashboxContainer();
            container.Register<IT, A>("A");

            Assert.Throws<ResolutionFailedException>(() => container.Resolve<ReadOnlyKeyValue<string, IT>>("A"));
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void ReadOnlyKeyValueTests_Resolve(CompilerType compilerType)
        {
            var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<IT, A>("A");
            container.Register<IT, B>("B");
            var a = container.Resolve<ReadOnlyKeyValue<object, IT>>("A");

            Assert.IsType<A>(a.Value);
            Assert.Equal("A", a.Key);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void ReadOnlyKeyValueTests_Resolve_Wrapped(CompilerType compilerType)
        {
            var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<IT, A>("A");
            container.Register<IT, B>("B");
            var a = container.Resolve<ReadOnlyKeyValue<object, Lazy<IT>>>("A");

            Assert.IsType<A>(a.Value.Value);
            Assert.Equal("A", a.Key);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void ReadOnlyKeyValueTests_Resolve_Wrapped_Enumerable(CompilerType compilerType)
        {
            var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<IT, A>("A");
            container.Register<IT, B>("B");
            var a = container.Resolve<ReadOnlyKeyValue<object, IT[]>>();

            Assert.IsType<A>(a.Value[0]);
            Assert.IsType<B>(a.Value[1]);
            Assert.Null(a.Key);
        }
        
        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void ReadOnlyKeyValueTests_Resolve_Enumerable(CompilerType compilerType)
        {
            var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<IT, A>("A");
            container.Register<IT, B>("B");
            var values = container.Resolve<ReadOnlyKeyValue<object, IT>[]>();

            Assert.Equal(2, values.Length);
            Assert.IsType<A>(values[0].Value);
            Assert.Equal("A", values[0].Key);
            Assert.IsType<B>(values[1].Value);
            Assert.Equal("B", values[1].Key);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void ReadOnlyKeyValueTests_Resolve_Enumerable_Includes_Non_Named(CompilerType compilerType)
        {
            var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<IT, A>("A");
            container.Register<IT, B>();
            var values = container.Resolve<ReadOnlyKeyValue<object, IT>[]>();

            Assert.Equal(2, values.Length);
            Assert.IsType<A>(values[0].Value);
            Assert.Equal("A", values[0].Key);
            Assert.IsType<B>(values[1].Value);
            Assert.Null(values[1].Key);
        }

        interface IT { }

        class A : IT { }

        class B : IT { }
    }
}
