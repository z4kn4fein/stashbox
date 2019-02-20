using Stashbox.BuildUp.Expressions;
using Stashbox.Registration;

namespace Stashbox.BuildUp
{
    internal class ObjectBuilderSelector : IObjectBuilderSelector
    {
        private readonly IObjectBuilder[] builderRepository;

        public ObjectBuilderSelector(IExpressionBuilder expressionBuilder, IServiceRegistrator serviceRegistrator)
        {
            this.builderRepository = new IObjectBuilder[]
            {
                new DefaultObjectBuilder(expressionBuilder),
                new FactoryObjectBuilder(expressionBuilder),
                new GenericTypeObjectBuilder(serviceRegistrator),
                new InstanceObjectBuilder(),
                new WireUpObjectBuilder(expressionBuilder),
                new FuncObjectBuilder()
            };
        }

        public IObjectBuilder Get(ObjectBuilder builderType) => this.builderRepository[(int)builderType];
    }
}
