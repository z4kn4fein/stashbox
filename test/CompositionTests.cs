using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stashbox.Tests
{
    public class CompositionTests
    {
        [Fact]
        public void Composition_Test()
        {
            using var container = new StashboxContainer()
                .Register<IA, A>()
                .Register<IB, B>()
                .Register<IC, C>()
                .RegisterDecorator<IA, D1>(c => c.AsImplementedTypes());

            var a = container.Resolve<IA>();

            Assert.IsType<D1>(a);
            Assert.IsType<A>(((D1)a).A);
            Assert.IsType<B>(((D1)a).B);
            Assert.IsType<C>(((D1)a).C);

            var b = container.Resolve<IB>();

            Assert.IsType<D1>(b);
            Assert.IsType<A>(((D1)b).A);
            Assert.IsType<B>(((D1)b).B);
            Assert.IsType<C>(((D1)b).C);

            var c = container.Resolve<IC>();

            Assert.IsType<D1>(c);
            Assert.IsType<A>(((D1)c).A);
            Assert.IsType<B>(((D1)c).B);
            Assert.IsType<C>(((D1)c).C);
        }

        [Fact]
        public void Composition_Test_Generic()
        {
            using var container = new StashboxContainer()
                .Register(typeof(IG<>), typeof(G<>))
                .Register(typeof(IG1<>), typeof(G1<>))
                .RegisterDecorator(typeof(D4<>));

            var m = container.ContainerContext.DecoratorRepository.GetRegistrationMappings();

            var a = container.Resolve<IG<string>>();

            Assert.IsType<D4<string>>(a);
            Assert.IsType<G<string>>(((D4<string>)a).A);
            Assert.IsType<G1<string>>(((D4<string>)a).B);

            var b = container.Resolve<IG1<string>>();

            Assert.IsType<D4<string>>(a);
            Assert.IsType<G<string>>(((D4<string>)a).A);
            Assert.IsType<G1<string>>(((D4<string>)a).B);
        }

        [Fact]
        public void Composition_Test_MultipleDecorators()
        {
            using var container = new StashboxContainer()
                .Register<IA, A>()
                .Register<IB, B>()
                .Register<IC, C>()
                .RegisterDecorator<IA, AD>()
                .RegisterDecorator<IB, BD>()
                .RegisterDecorator<IC, CD>()
                .RegisterDecorator<D1>();

            var d = container.Resolve<IA>();

            Assert.IsType<D1>(d);
            Assert.IsType<AD>(((D1)d).A);
            Assert.IsType<A>(((AD)((D1)d).A).A);
            Assert.IsType<BD>(((D1)d).B);
            Assert.IsType<B>(((BD)((D1)d).B).B);
            Assert.IsType<CD>(((D1)d).C);
            Assert.IsType<C>(((CD)((D1)d).C).C);
        }

        [Fact]
        public void Composition_Test_MultipleDecorators_Factory()
        {
            using var container = new StashboxContainer()
                .Register<IA, A>()
                .Register<IB, B>()
                .Register<IC, C>()
                .RegisterDecorator<IA, AD>()
                .RegisterDecorator<IB, BD>()
                .RegisterDecorator<IC, CD>()
                .RegisterDecorator<D1>(c => c.WithFactory<IA, IA, IB, IC>((a, a1, b, c) =>
                {
                    Assert.IsType<AD>(a);
                    Assert.IsType<A>(((AD)a).A);
                    Assert.IsType<AD>(a1);
                    Assert.IsType<A>(((AD)a1).A);
                    Assert.IsType<BD>(b);
                    Assert.IsType<B>(((BD)b).B);
                    Assert.IsType<CD>(c);
                    Assert.IsType<C>(((CD)c).C);

                    return new D1(a, b, c);
                }));

            container.Resolve<IA>();
        }

        [Fact]
        public void Composition_Test_Enumerable()
        {
            using var container = new StashboxContainer()
                .Register<IA, A>()
                .Register<IA, AA>()
                .RegisterDecorator<IA, D2>();

            var d = container.Resolve<IA>();

            Assert.IsType<D2>(d);
            Assert.IsAssignableFrom<IEnumerable<IA>>(((D2)d).A);
            Assert.IsType<A>(((D2)d).A.ElementAt(0));
            Assert.IsType<AA>(((D2)d).A.ElementAt(1));
        }

        [Fact]
        public void Composition_Test_Enumerable_MultipleDecorators()
        {
            using var container = new StashboxContainer()
                .Register<IA, A>()
                .Register<IA, AA>()
                .RegisterDecorator<IA, AD>()
                .RegisterDecorator<IA, D2>();

            var d = container.Resolve<IA>();

            Assert.IsType<D2>(d);
            Assert.IsType<AD>(((D2)d).A.ElementAt(0));
            Assert.IsType<A>(((AD)((D2)d).A.ElementAt(0)).A);
            Assert.IsType<AD>(((D2)d).A.ElementAt(1));
            Assert.IsType<AA>(((AD)((D2)d).A.ElementAt(1)).A);
        }

        [Fact]
        public void Composition_Test_Enumerable_Generic()
        {
            using var container = new StashboxContainer()
                .Register(typeof(IG<>), typeof(G<>))
                .Register(typeof(IG<>), typeof(GG<>))
                .RegisterDecorator(typeof(IG<>), typeof(D3<>));

            var d = container.Resolve<IG<string>>();

            Assert.IsType<D3<string>>(d);
            Assert.IsType<G<string>>(((D3<string>)d).G.ElementAt(0));
            Assert.IsType<GG<string>>(((D3<string>)d).G.ElementAt(1));
        }

        interface IA { }

        interface IB { }

        interface IC { }

        interface IG<T> { }

        interface IG1<T> { }

        class A : IA { }

        class AA : IA { }

        class B : IB { }

        class BB : IB { }

        class C : IC { }

        class CC : IC { }

        class G<T> : IG<T> { }

        class G1<T> : IG1<T> { }

        class GG<T>/*WP*/ : IG<T> { }

        class D1 : IA, IB, IC
        {
            public D1(IA a, IB b, IC c)
            {
                A = a;
                B = b;
                C = c;
            }

            public IA A { get; }
            public IB B { get; }
            public IC C { get; }
        }

        class D2 : IA
        {
            public D2(IEnumerable<IA> a)
            {
                A = a;
            }

            public IEnumerable<IA> A { get; }
        }

        class D3<T> : IG<T>
        {
            public D3(IEnumerable<IG<T>> g)
            {
                G = g;
            }

            public IEnumerable<IG<T>> G { get; }
        }

        class D4<T> : IG<T>, IG1<T>
        {
            public D4(IG<T> a, IG1<T> b)
            {
                A = a;
                B = b;
            }

            public IG<T> A { get; }
            public IG1<T> B { get; }
        }

        class AD : IA
        {
            public AD(IA a)
            {
                A = a;
            }

            public IA A { get; }
        }

        class BD : IB
        {
            public BD(IB b)
            {
                B = b;
            }

            public IB B { get; }
        }

        class CD : IC
        {
            public CD(IC c)
            {
                C = c;
            }

            public IC C { get; }
        }
    }
}
