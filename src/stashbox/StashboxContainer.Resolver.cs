using Stashbox.Entity;
using Stashbox.MetaInfo;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Stashbox.Registration;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public TKey Resolve<TKey>(string name = null) where TKey : class =>
            this.ActivationContext.Activate(typeof(TKey), name) as TKey;

        /// <inheritdoc />
        public object Resolve(Type typeFrom, string name = null) =>
            this.ActivationContext.Activate(typeFrom, name);

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>() where TKey : class =>
            this.Resolve<IEnumerable<TKey>>();

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            var type = typeof(IEnumerable<>).MakeGenericType(typeFrom);
            return (IEnumerable<object>)this.Resolve(type);
        }

        /// <inheritdoc />
        public Delegate ResolveFactory(Type typeFrom, string name = null, params Type[] parameterTypes) =>
            this.ActivationContext.ActivateFactory(typeFrom, parameterTypes, name);

        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance) where TTo : class
        {
            var typeTo = instance.GetType();
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, RegistrationContextData.Empty, typeTo);

            var resolutionInfo = ResolutionInfo.New();
            var expr = this.expressionBuilder.CreateFillExpression(this.containerExtensionManager, this.ContainerContext,
                Expression.Constant(instance), resolutionInfo, typeTo, null, metaInfoProvider.GetResolutionMembers(resolutionInfo),
                metaInfoProvider.GetResolutionMethods(resolutionInfo));

            var factory = expr.CompileDelegate();
            return factory() as TTo;
        }
    }
}
