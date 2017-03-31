namespace Stashbox.Infrastructure
{
    internal enum ObjectBuilder
    {
        Default,
        Factory,
        Generic,
        Instance,
        WireUp
    }
    
    internal interface IObjectBuilderSelector
    {
        IObjectBuilder Get(ObjectBuilder builderType);
    }
}
