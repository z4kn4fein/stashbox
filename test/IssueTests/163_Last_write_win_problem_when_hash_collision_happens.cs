using Stashbox.Tests.Utils;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class LastWriteWinProblemWhenHashCollisionHappens 
{
    [Fact]
    public void Ensure_Collision_Doesnt_Happen()
    {
        var (type1, type2) = TypeGen.GetCollidingTypes();
        
        var services = new StashboxContainer();
        services.Register(type1);
        services.Register(type2);
        
        Assert.NotNull(services.Resolve(type2));
        Assert.NotNull(services.Resolve(type1));
    }
}