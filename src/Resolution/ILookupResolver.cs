namespace Stashbox.Resolution;

internal interface ILookup
{
    bool CanLookupService(TypeInformation typeInfo, ResolutionContext resolutionContext);
}