using System.Collections;
using System.Collections.Generic;

namespace Stashbox.Utils
{
    /// <summary>
    /// Represents a concurrent collection.
    /// </summary>
    /// <typeparam name="TContent">The content type generic parameter.</typeparam>
    public class ConcurrentStore<TContent> : IEnumerable<TContent>
    {
        private readonly Swap<LinkedStore<TContent>> repository;

        /// <summary>
        /// Constructs a <see cref="ConcurrentStore{TContent}"/>
        /// </summary>
        public ConcurrentStore()
        {
            this.repository = new Swap<LinkedStore<TContent>>(new LinkedStore<TContent>(default(TContent), null, 0));
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="content">The item to be added.</param>
        public void Add(TContent content)
        {
            var current = repository.Value;
            var newRepo = this.repository.Value.Add(content);

            if(!this.repository.TrySwapCurrent(current, newRepo))
                this.repository.SwapCurrent(repo => repo.Add(content));
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<TContent> GetEnumerator() => this.repository.Value.GetEnumerator();
    }
}
