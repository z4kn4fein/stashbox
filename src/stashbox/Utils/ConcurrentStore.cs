using System.Collections;
using System.Collections.Generic;

namespace Stashbox.Utils
{
    /// <summary>
    /// Represents a concurrent collection.
    /// </summary>
    /// <typeparam name="TContent">The content type generic parameter.</typeparam>
    public class ConcurrentOrderedStore<TContent>
    {
        private readonly Swap<ArrayStore<TContent>> repository;

        /// <summary>
        /// The length of the collection.
        /// </summary>
        public int Lenght => this.repository.Value.Length;

        /// <summary>
        /// Constructs a <see cref="ConcurrentOrderedStore{TContent}"/>
        /// </summary>
        public ConcurrentOrderedStore()
        {
            this.repository = new Swap<ArrayStore<TContent>>(ArrayStore<TContent>.Empty);
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="content">The item to be added.</param>
        public void Add(TContent content)
        {
            var current = this.repository.Value;
            var newRepo = this.repository.Value.Add(content);

            if(!this.repository.TrySwapCurrent(current, newRepo))
                this.repository.SwapCurrent(repo => repo.Add(content));
        }

        /// <summary>
        /// Gets an item from the collection.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The item.</returns>
        public TContent Get(int index) => this.repository.Value.Get(index);
    }

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
            this.repository = new Swap<LinkedStore<TContent>>(LinkedStore<TContent>.Empty);
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="content">The item to be added.</param>
        public void Add(TContent content)
        {
            var current = this.repository.Value;
            var newRepo = this.repository.Value.Add(content);

            if (!this.repository.TrySwapCurrent(current, newRepo))
                this.repository.SwapCurrent(repo => repo.Add(content));
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<TContent> GetEnumerator() => this.repository.Value.GetEnumerator();
    }
}
