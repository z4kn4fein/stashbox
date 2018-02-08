using Stashbox.BuildUp.Expressions;

namespace Stashbox.BuildUp
{
    internal class ObjectBuilderSelector : IObjectBuilderSelector
    {
        private readonly IObjectBuilder[] builderRepository;

        public ObjectBuilderSelector(IExpressionBuilder expressionBuilder)
        {
            this.builderRepository = new IObjectBuilder[]
            {
                new DefaultObjectBuilder(expressionBuilder),
                new FactoryObjectBuilder(expressionBuilder),
                new GenericTypeObjectBuilder(),
                new InstanceObjectBuilder(),
                new WireUpObjectBuilder(expressionBuilder),
                new FuncObjectBuilder()
            };
        }

        public IObjectBuilder Get(ObjectBuilder builderType) => this.builderRepository[(int)builderType].Produce();
    }
}
