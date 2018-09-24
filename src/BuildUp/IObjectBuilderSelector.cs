namespace Stashbox.BuildUp
{
    internal enum ObjectBuilder
    {
        Default,
        Factory,
        Generic,
        Instance,
        WireUp,
        Func
    }

    internal interface IObjectBuilderSelector
    {
        IObjectBuilder Get(ObjectBuilder builderType);
    }
}
