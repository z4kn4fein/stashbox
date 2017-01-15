namespace Stashbox.Utils
{
    /// <summary>
    /// Represents a concurrent collection.
    /// </summary>
    /// <typeparam name="TContent">The content type generic parameter.</typeparam>
    public class ConcurrentStore<TContent> : ConcurrentKeyValueStore<int, TContent>
    {
        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="content">The item to be added.</param>
        public void Add(TContent content)
        {
            base.Add(content.GetHashCode(), content);
        }
    }
}
