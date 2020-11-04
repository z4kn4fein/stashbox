using Stashbox.Registration.Fluent;
using System;
using System.Linq;
using Xunit;

namespace Stashbox.Tests.IssueTests
{
    public class LifetimeIssues
    {
        class DoResolveAttribute : Attribute { }
        interface ITier1 { }
        class Tier1 : ITier1 {[DoResolve] public ITier2 Inner { get; set; }[DoResolve] public TierBase OtherInner { get; set; } }
        interface ITier2 { }
        class Tier2 : ITier2 { public Tier2(string name) { Name = name; } public string Name { get; set; } }
        abstract class TierBase { public int Id { get; protected set; } }
        class Tier3 : TierBase { public Tier3(int id) { Id = id; }[DoResolve] public ITier2 Inner { get; set; } }

        public abstract class PrivateArgs
        {
            public object[] ArgList { get; protected set; }
            public abstract Type Target { get; }
        }
        public class PrivateArgs<T> : PrivateArgs
        {
            static readonly Type _Target = typeof(T);

            public PrivateArgs(params object[] args) : base()
            {
                ArgList = args;
            }

            public static PrivateArgs<T> Get(params object[] args)
            {
                var res = new PrivateArgs<T>();
                res.ArgList = args;
                return res;
            }

            public override Type Target { get; } = _Target;
        }

        [Fact]
        public void ContextEstablishedInChildContainersCanBeAccessedWhenUsingAParentScopeConstruction()
        {
            StashboxContainer sb1 = CreateContainer(c => c.WithSingletonLifetime());
            StashboxContainer sb2 = CreateContainer(c => c.WithSingletonLifetime());

            // This works
            sb1.PutInstanceInScope(typeof(PrivateArgs<ITier2>), PrivateArgs<ITier2>.Get("Bob"));
            sb1.PutInstanceInScope(typeof(PrivateArgs<TierBase>), PrivateArgs<TierBase>.Get(5));
            ITier1 renderer = (ITier1)sb1.Resolve(typeof(ITier1));


            using var scope = sb2.BeginScope();
            scope.PutInstanceInScope(typeof(PrivateArgs<ITier2>), PrivateArgs<ITier2>.Get("Bob"));
            scope.PutInstanceInScope(typeof(PrivateArgs<TierBase>), PrivateArgs<TierBase>.Get(5));
            ITier1 renderer2 = (ITier1)scope.Resolve(typeof(ITier1));

        }

        [Fact]
        public void ContextEstablishedInChildContainersCanBeAccessedWhenUsingAParentScopeConstructionWithChildContainer()
        {
            StashboxContainer sb1 = CreateContainer(c => c.WithScopedLifetime());
            StashboxContainer sb2 = CreateContainer(c => c.WithScopedLifetime());

            // This works
            using var scope1 = sb1.BeginScope();
            scope1.PutInstanceInScope(typeof(PrivateArgs<ITier2>), PrivateArgs<ITier2>.Get("Bob"));
            scope1.PutInstanceInScope(typeof(PrivateArgs<TierBase>), PrivateArgs<TierBase>.Get(5));
            ITier1 renderer = (ITier1)scope1.Resolve(typeof(ITier1));

            // This fails
            using var sbc = sb2.CreateChildContainer();
            using var scope2 = sbc.BeginScope();
            scope2.PutInstanceInScope(typeof(PrivateArgs<ITier2>), PrivateArgs<ITier2>.Get("Bob"));
            scope2.PutInstanceInScope(typeof(PrivateArgs<TierBase>), PrivateArgs<TierBase>.Get(5));
            ITier1 renderer2 = (ITier1)scope2.Resolve(typeof(ITier1));
        }

        private static StashboxContainer CreateContainer(Action<RegistrationConfigurator> scopeConfig = null)
        {
            var sb = new StashboxContainer(
                            config => config
                                .WithUnknownTypeResolution(ctx => ctx.WhenHas<DoResolveAttribute>())
                                .WithAutoMemberInjection(
                                    Stashbox.Configuration.Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter,
                                    ti => ti.CustomAttributes.OfType<DoResolveAttribute>().Any()
                                    )
                            );

            var allTypes = new[]
                {
                    new Tuple<Type, Type>(typeof(ITier1), typeof(Tier1)),
                    new Tuple<Type, Type>(typeof(ITier2), typeof(Tier2)),
                    new Tuple<Type, Type>(typeof(TierBase), typeof(Tier3)),
                }.ToList();

            foreach (var type in allTypes)
            {
                var tbuilt = type.Item2;
                var tinterface = type.Item1;


                var targs = typeof(PrivateArgs<>).MakeGenericType(tinterface);
                sb.Register(tinterface, tbuilt,
                    c =>
                            c
                            .WithFactory(d =>
                            {
                                PrivateArgs argContainer = d.Resolve(targs) as PrivateArgs;
                                object[] args = argContainer?.ArgList;
                                object res = null;
                                try { res = Activator.CreateInstance(tbuilt, args ?? new object[0]); }
                                catch { }

                                return res;
                            })
                            .WithAutoMemberInjection(
                                Stashbox.Configuration.Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter,
                                ti => ti.CustomAttributes.OfType<DoResolveAttribute>().Any())
                            // Simple extension method allowing conditional configuration inline
                            .ApplyIf(scopeConfig != null, scopeConfig)
                    );
            }

            return sb;
        }
    }

    public static class RegExt
    {
        public static void ApplyIf(this RegistrationConfigurator configurator, bool b, Action<RegistrationConfigurator> scopeConfig)
        {
            if (b)
                scopeConfig.Invoke(configurator);
        }
    }
}
