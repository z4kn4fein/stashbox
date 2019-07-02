using Stashbox.BuildUp.Expressions;
using Stashbox.BuildUp.ObjectBuilders;
using Stashbox.Registration;

namespace Stashbox.BuildUp
{
    internal class ObjectBuilderSelector : IObjectBuilderSelector
    {
        private readonly IObjectBuilder[] builderRepository;

        public ObjectBuilderSelector(IExpressionBuilder expressionBuilder, IServiceRegistrator serviceRegistrator)
        {
            var defaultObjectBuilder = new DefaultObjectBuilder(expressionBuilder);
            this.builderRepository = new IObjectBuilder[]
            {
                defaultObjectBuilder,
                new FactoryObjectBuilder(expressionBuilder),
                new GenericTypeObjectBuilder(serviceRegistrator, defaultObjectBuilder),
                new InstanceObjectBuilder(),
                new WireUpObjectBuilder(expressionBuilder),
                new FuncObjectBuilder()
            };
        }

        public IObjectBuilder Get(ObjectBuilder builderType) => this.builderRepository[(int)builderType];
    }
}
