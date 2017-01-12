using System.Collections.Generic;

namespace Stashbox.Utils
{
    /// <summary>
    /// Represents a portable thread safe key-value store.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TContent">The value type.</typeparam>
    public class ConcurrentKeyValueStore<TKey, TContent>
    {
        /// <summary>
        /// Delegate for adding a new entry.
        /// </summary>
        public delegate TContent AddDelegate();

        /// <summary>
        /// Delegate for updating an entry.
        /// </summary>
        /// <param name="oldValue">The existing value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        public delegate TContent UpdateDelegate(TContent oldValue, TContent newValue);

        /// <summary>
        /// The key-value repository.
        /// </summary>
        protected readonly IDictionary<TKey, TContent> Repository;

        /// <summary>
        /// The synchronization lock.
        /// </summary>
        protected readonly DisposableReaderWriterLock ReaderWriterLock;

        /// <summary>
        /// Constructs a <see cref="ConcurrentKeyValueStore{TKey, TContent}"/>
        /// </summary>
        public ConcurrentKeyValueStore()
        {
            this.Repository = new Dictionary<TKey, TContent>();
            this.ReaderWriterLock = new DisposableReaderWriterLock();
        }

        /// <summary>
        /// Returns with the key collection of the store.
        /// </summary>
        public ICollection<TKey> Keys => this.Repository.Keys;

        /// <summary>
        /// Adds or updates a value in the collection.
        /// </summary>
        /// <param name="key">Key of the value which will be added or updated.</param>
        /// <param name="addFunc">The factory which creates the value which will be added.</param>
        /// <param name="updateFunc">The factory which creates the value which will be used for replacing the existing value.</param>
        public void AddOrUpdate(TKey key, AddDelegate addFunc, UpdateDelegate updateFunc)
        {
            Shield.EnsureNotNull(addFunc, nameof(addFunc));
            Shield.EnsureNotNull(updateFunc, nameof(updateFunc));

            using (this.ReaderWriterLock.AcquireWriteLock())
            {
                if (this.Repository.ContainsKey(key))
                {
                    var content = updateFunc(this.Repository[key], addFunc());
                    this.Repository[key] = content;
                }
                else
                {
                    var content = addFunc();
                    this.Repository.Add(key, content);
                }
            }
        }

        /// <summary>
        /// Gets an existing value or adds a new one if it's doesn't exists.
        /// </summary>
        /// <param name="key">Key of the new or existing entry.</param>
        /// <param name="valueFactory">The factory which creates the value which will be added to the collection.</param>
        /// <returns>The value which will be added if it doesn't exists.</returns>
        public TContent GetOrAdd(TKey key, AddDelegate valueFactory)
        {
            Shield.EnsureNotNull(valueFactory, nameof(valueFactory));

            using (var context = this.ReaderWriterLock.AcquireUpgreadeableReadLock())
            {
                TContent existingContent;
                if (this.Repository.TryGetValue(key, out existingContent))
                    return existingContent;

                using (context.AcquireWriteLock())
                {
                    if (this.Repository.TryGetValue(key, out existingContent))
                        return existingContent;

                    var content = valueFactory();
                    this.Repository.Add(key, content);
                    return content;
                }
            }
        }

        /// <summary>
        /// Tries to retrieve an entry from the collection.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <param name="content">The value which will be filled with the retrieved entry.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool TryGet(TKey key, out TContent content)
        {
            using (this.ReaderWriterLock.AcquireReadLock())
            {
                return this.Repository.TryGetValue(key, out content);
            }
        }

        /// <summary>
        /// Retrieves all entry from the collection.
        /// </summary>
        /// <returns>All entries of the collection.</returns>
        public IEnumerable<TContent> GetAll()
        {
            using (this.ReaderWriterLock.AcquireReadLock())
            {
                return this.Repository.Values;
            }
        }

        /// <summary>
        /// Adds an entry to the collection.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <param name="content">The value of the entry.</param>
        public void Add(TKey key, TContent content)
        {
            using (this.ReaderWriterLock.AcquireWriteLock())
            {
                this.Repository.Add(key, content);
            }
        }

        /// <summary>
        /// Sets an already existing entry in the collection.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <param name="content">The new value of the existing entry.</param>
        public void Set(TKey key, TContent content)
        {
            using (this.ReaderWriterLock.AcquireWriteLock())
            {
                if (this.Repository.ContainsKey(key))
                {
                    this.Repository[key] = content;
                }
                else
                {
                    throw new KeyNotFoundException($"The given key {key} not found.");
                }
            }
        }

        /// <summary>
        /// Removes an entry from the collection.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        public void Remove(TKey key)
        {
            using (var context = this.ReaderWriterLock.AcquireUpgreadeableReadLock())
            {
                if (!this.Repository.ContainsKey(key))
                    throw new KeyNotFoundException($"The given key {key} not found.");
                using (context.AcquireWriteLock())
                {
                    if (!this.Repository.ContainsKey(key))
                        throw new KeyNotFoundException($"The given key {key} not found.");
                    this.Repository.Remove(key);
                }
            }
        }

        /// <summary>
        /// Removes an entry from the collection if exists.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool TryRemove(TKey key)
        {
            using (var context = this.ReaderWriterLock.AcquireUpgreadeableReadLock())
            {
                if (!this.Repository.ContainsKey(key)) return false;
                using (context.AcquireWriteLock())
                {
                    if (!this.Repository.ContainsKey(key)) return false;
                    this.Repository.Remove(key);
                    return true;
                }
            }
        }
    }
}
