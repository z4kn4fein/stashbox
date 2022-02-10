using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Tests
{
    public class ScopeTests
    {
        [Fact]
        public void GetOrAdd_Ensure_Evaluator_DoesNotThrow()
        {
            for (int i = 0; i < 5000; i++)
            {
                using var scope = (IResolutionScope)new StashboxContainer().BeginScope();
                Parallel.For(0, 50, i =>
                {
                    var inst = scope.GetOrAddScopedObject(1, (_, _) => new object(), null, typeof(object));
                    Assert.NotNull(inst);
                });
            }
            
        }
    }
}
