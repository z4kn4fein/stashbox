using Ronin.Common;
using Stashbox.Infrastructure;
using System.Collections.Generic;

namespace Stashbox.Extensions
{
    public class Bag : ConcurrentKeyValueStore<string, IBagItem>
    {
        public TResult Get<TResult>(string key)
            where TResult : class, IBagItem
        {
            using (base.ReaderWriterLock.AquireReadLock())
            {
                IBagItem result;
                if (base.Repository.TryGetValue(key, out result))
                {
                    return result as TResult;
                }
                else
                {
                    throw new KeyNotFoundException($"{key} not found.");
                }
            }
        }
    }
}
