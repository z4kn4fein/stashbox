using Stashbox.MetaInfo;
using System;
namespace Stashbox.Infrastructure
{
    public interface IMetaInfoCacheRepository
    {
        MetaInfoCache GetOrAddTypeCache(Type type);
    }
}
