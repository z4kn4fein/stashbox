using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox
{
    internal sealed partial class ResolutionScope
    {
        private sealed class AsyncInitializable
        {
            private readonly object item;
            private readonly Func<object, IDependencyResolver, CancellationToken, Task> initializer;
            private int initialized;

            public AsyncInitializable(object item, Func<object, IDependencyResolver, CancellationToken, Task> initializer)
            {
                this.item = item;
                this.initializer = initializer;
                this.initialized = 0;
            }

            public ValueTask InvokeAsync(IDependencyResolver resolver, CancellationToken token) =>
                Interlocked.CompareExchange(ref initialized, 1, 0) != 0
                    ? default
                    : new ValueTask(initializer(item, resolver, token));
        }

        private ImmutableLinkedList<AsyncInitializable> initializables = ImmutableLinkedList<AsyncInitializable>.Empty;

        public object AddWithAsyncInitializer(object initializable, Func<object, IDependencyResolver, CancellationToken, Task> initializer)
        {
            this.ThrowIfDisposed();

            Swap.SwapValue(ref this.initializables, (t1, t2, _, _, root) =>
                    root.Add(new AsyncInitializable(t1, t2)), initializable, initializer,
                Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return initializable;
        }

        /// <inheritdoc />
        public async ValueTask InvokeAsyncInitializers(CancellationToken token = default)
        {
            this.ThrowIfDisposed();

            if (this.ParentScope != null)
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
