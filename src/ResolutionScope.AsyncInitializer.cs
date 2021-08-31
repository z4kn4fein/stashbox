using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox
{
    internal sealed partial class ResolutionScope
    {
        private class AsyncInitializable
        {
            public readonly object Item;
            public readonly Func<object, IDependencyResolver, CancellationToken, Task> Initializer;
            private int initialized;

            public AsyncInitializable(object item, Func<object, IDependencyResolver, CancellationToken, Task> initializer)
            {
                this.Item = item;
                this.Initializer = initializer;
                this.initialized = 0;
            }

            public ValueTask InvokeAsync(IDependencyResolver resolver, CancellationToken token)
            {
                if (Interlocked.CompareExchange(ref initialized, 1, 0) != 0)
                    return default;

                return new ValueTask(Initializer(Item, resolver, token));
            }
        }

        private ImmutableLinkedList<AsyncInitializable> initializables = ImmutableLinkedList<AsyncInitializable>.Empty;

        public object AddWithAsyncInitializer(object initializable, Func<object, IDependencyResolver, CancellationToken, Task> initializer)
        {
            this.ThrowIfDisposed();

            Swap.SwapValue(ref this.initializables, (t1, t2, t3, t4, root) =>
                    root.Add(new AsyncInitializable(t1, t2)), initializable, initializer,
                Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return initializable;
        }

        public async ValueTask InvokeAsyncInitializers(CancellationToken token)
        {
            if(this.ParentScope != null)
                await this.ParentScope.InvokeAsyncInitializers(token).ConfigureAwait(false);

            var initializable = this.initializables;
            while (!ReferenceEquals(initializable, ImmutableLinkedList<AsyncInitializable>.Empty))
            {
                await initializable.Value.InvokeAsync(this, token).ConfigureAwait(false);
                initializable = initializable.Next;
            }
        }
    }
}
