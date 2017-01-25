using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Stashbox.Utils
{
    internal class ConcurrentTree<TKey, TValue> : IEnumerable<TValue>
    {
        public static ConcurrentTree<TKey, TValue> Create() => new ConcurrentTree<TKey, TValue>();

        private AvlTree<TKey, TValue> repository;

        public TValue Value => this.repository.Value;
        public bool HasMultipleItems => this.repository.HasMultipleItems;

        public ConcurrentTree()
        {
            this.repository = new AvlTree<TKey, TValue>();
        }

        public TValue GetOrDefault(TKey key) => this.repository.GetOrDefault(key);

        public ConcurrentTree<TKey, TValue> AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null)
        {
            var currentRepo = this.repository;
            var newRepo = this.repository.AddOrUpdate(key, value, updateDelegate);

            if (!this.TrySwapCurrentRepository(currentRepo, newRepo))
                this.SwapCurrentRepository(repo => repo.AddOrUpdate(key, value, updateDelegate));

            return this;
        }

        private bool TrySwapCurrentRepository(AvlTree<TKey, TValue> currentRepo, AvlTree<TKey, TValue> newRepo) =>
            Interlocked.CompareExchange(ref repository, newRepo, currentRepo) == currentRepo;

        private void SwapCurrentRepository(Func<AvlTree<TKey, TValue>, AvlTree<TKey, TValue>> repoFactory)
        {
            AvlTree<TKey, TValue> currentRepo;
            AvlTree<TKey, TValue> newRepo;
            int counter = 0;

            do
            {
                if (++counter > 20)
                    throw new InvalidOperationException("Swap quota exceeded.");

                currentRepo = this.repository;
                newRepo = repoFactory(currentRepo);
            } while (Interlocked.CompareExchange(ref repository, newRepo, currentRepo) != currentRepo);
        }

        public IEnumerator<TValue> GetEnumerator() => this.repository.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.repository.GetEnumerator();
    }
}
