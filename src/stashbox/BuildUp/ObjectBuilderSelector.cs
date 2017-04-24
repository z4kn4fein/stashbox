using Stashbox.BuildUp.Expressions;
using Stashbox.Infrastructure;

namespace Stashbox.BuildUp
{
    internal class ObjectBuilderSelector : IObjectBuilderSelector
    {
        private readonly IObjectBuilder[] builderRepository;

        public ObjectBuilderSelector(IContainerContext containerContext, IExpressionBuilder expressionBuilder)
        {
            this.builderRepository = new IObjectBuilder[]
            {
                new DefaultObjectBuilder(containerContext, expressionBuilder),
                new FactoryObjectBuilder(containerContext, expressionBuilder),
                new GenericTypeObjectBuilder(containerContext),
                new InstanceObjectBuilder(containerContext),
                new WireUpObjectBuilder(containerContext, expressionBuilder),
                new FuncObjectBuilder(containerContext)
            };
        }

        public IObjectBuilder Get(ObjectBuilder builderType) => this.builderRepository[(int)builderType].Produce();
    }
}
