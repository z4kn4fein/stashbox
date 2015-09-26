using Stashbox.Infrastructure;

namespace Stashbox.Entity
{
    public class ResolutionTarget
    {
        public TypeInformation TypeInformation { get; set; }
        public Resolver Resolver { get; set; }
        public string ResolutionTargetName { get; set; }
        public object ResolutionTargetValue { get; set; }
    }
}
