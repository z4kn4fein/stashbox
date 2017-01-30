using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Overrides;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Resolution
{
    internal class FuncResolver : Resolver
    {
        public static ISet<Type> SupportedTypes = new HashSet<Type>
        {
            typeof(Func<>),
            typeof(Func<,>),
            typeof(Func<,,>),
            typeof(Func<,,,>)
        };

        private readonly IServiceRegistration registrationCache;
        private delegate object ResolverDelegate(ResolutionInfo resolutionInfo);
        private readonly ResolverDelegate resolverDelegate;
        private readonly TypeInformation funcArgumentInfo;
        private readonly MethodInfo resolverMethodInfo;

        public FuncResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            this.funcArgumentInfo = new TypeInformation
            {
                Type = typeInfo.Type.GenericTypeArguments.Last(),
                CustomAttributes = typeInfo.CustomAttributes,
                ParentType = typeInfo.ParentType,
                DependencyName = typeInfo.DependencyName
            };

            if (!containerContext.RegistrationRepository.TryGetRegistrationWithConditions(this.funcArgumentInfo, out this.registrationCache))
                throw new ResolutionFailedException(typeInfo.Type.FullName);

            var methodName = "ResolveFuncP" + (typeInfo.Type.GenericTypeArguments.Length - 1);

            var resolverMethod = this.GetType().GetTypeInfo().GetDeclaredMethod(methodName);
            this.resolverMethodInfo = resolverMethod.MakeGenericMethod(typeInfo.Type.GenericTypeArguments);
            this.resolverDelegate = (ResolverDelegate)resolverMethodInfo.CreateDelegate(typeof(ResolverDelegate), this);
        }

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            return this.resolverDelegate(resolutionInfo);
        }

        public override Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression)
        {
            return Expression.Call(Expression.Constant(this), this.resolverMethodInfo, resolutionInfoExpression);
        }

        private Func<TResult> ResolveFuncP0<TResult>(ResolutionInfo resolutionInfo)
        {
            return () => (TResult)registrationCache.GetInstance(resolutionInfo, this.funcArgumentInfo);
        }

        private Func<T, TResult> ResolveFuncP1<T, TResult>(ResolutionInfo resolutionInfo)
        {
            return (t) =>
            {
                if (resolutionInfo.OverrideManager == null)
                    resolutionInfo.OverrideManager = new OverrideManager(new Override[] { new TypeOverride(typeof(T), t) });
                else
                    resolutionInfo.OverrideManager.AddTypedOverride(new TypeOverride(typeof(T), t));
                return (TResult)registrationCache.GetInstance(resolutionInfo, this.funcArgumentInfo);
            };
        }

        private Func<T, T1, TResult> ResolveFuncP2<T, T1, TResult>(ResolutionInfo resolutionInfo)
        {
            return (t, t1) =>
            {
                if (resolutionInfo.OverrideManager == null)
                    resolutionInfo.OverrideManager = new OverrideManager(new Override[] { new TypeOverride(typeof(T), t), new TypeOverride(typeof(T1), t1) });
                else
                {
                    resolutionInfo.OverrideManager.AddTypedOverride(new TypeOverride(typeof(T), t));
                    resolutionInfo.OverrideManager.AddTypedOverride(new TypeOverride(typeof(T1), t1));
                }
                return (TResult)registrationCache.GetInstance(resolutionInfo, this.funcArgumentInfo);
            };
        }

        private Func<T, T1, T2, TResult> ResolveFuncP3<T, T1, T2, TResult>(ResolutionInfo resolutionInfo)
        {
            return (t, t1, t2) =>
            {
                if (resolutionInfo.OverrideManager == null)
                    resolutionInfo.OverrideManager = new OverrideManager(new Override[] { new TypeOverride(typeof(T), t), new TypeOverride(typeof(T1), t1), new TypeOverride(typeof(T2), t2) });
                else
                {
                    resolutionInfo.OverrideManager.AddTypedOverride(new TypeOverride(typeof(T), t));
                    resolutionInfo.OverrideManager.AddTypedOverride(new TypeOverride(typeof(T1), t1));
                    resolutionInfo.OverrideManager.AddTypedOverride(new TypeOverride(typeof(T2), t2));
                }
                return (TResult)registrationCache.GetInstance(resolutionInfo, this.funcArgumentInfo);
            };
        }
    }
}
