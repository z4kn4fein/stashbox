using Stashbox.Entity;
using Stashbox.MetaInfo;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Stashbox.BuildUp.Expressions;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public TKey Resolve<TKey>(string name = null) where TKey : class
        {
            return this.ResolveInternal(typeof(TKey), name) as TKey;
        }

        /// <inheritdoc />
        public object Resolve(Type typeFrom, string name = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            return this.ResolveInternal(typeFrom, name);
        }

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>() where TKey : class
        {
            return this.Resolve<IEnumerable<TKey>>();
        }

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            var type = typeof(IEnumerable<>).MakeGenericType(typeFrom);
            return (IEnumerable<object>)this.Resolve(type);
        }

        /// <inheritdoc />
        public Delegate ResolveFactory(Type typeFrom, string name = null, params Type[] parameterTypes)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));

            var typeInfo = new TypeInformation { Type = typeFrom, DependencyName = name };
            var resolutionInfo = new ResolutionInfo
            {
                ParameterExpressions = parameterTypes.Length == 0 ? null : parameterTypes.Select(Expression.Parameter).ToArray()
            };

            return this.ActivationContext.ActivateFactory(resolutionInfo, typeInfo, parameterTypes);
        }

        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance)
        {
            var typeTo = instance.GetType();
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, new MetaInfoCache(this.ContainerContext.ContainerConfigurator, typeTo));

            var resolutionInfo = new ResolutionInfo();
            var typeInfo = new TypeInformation { Type = typeTo };

            var expr = ExpressionDelegateFactory.CreateFillExpression(this.containerExtensionManager, this.ContainerContext,
                Expression.Constant(instance), resolutionInfo, typeInfo, null, metaInfoProvider.GetResolutionMembers(resolutionInfo), metaInfoProvider.GetResolutionMethods(resolutionInfo));

            var factory = Expression.Lambda<Func<TTo>>(expr).Compile();
            return factory();
        }

        private object ResolveInternal(Type typeFrom, string name = null)
        {
            var typeInfo = new TypeInformation { Type = typeFrom, DependencyName = name };
            return this.ActivationContext.Activate(new ResolutionInfo(), typeInfo);
        }
    }
}
