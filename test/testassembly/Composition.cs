using Stashbox;

namespace TestAssembly
{
    public class Composition : ICompositionRoot
    {
        public void Compose(IStashboxContainer container)
        {
            container.Register<IComp, Comp>(c => c.WithName("Comp"));
        }
    }

    public interface IComp { }

    class Comp : IComp { }
}
