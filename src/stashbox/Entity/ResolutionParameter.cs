using Stashbox.Infrastructure;

namespace Stashbox.Entity
{
    public class ResolutionParameter
    {
        public ParameterInformation ParameterInfo { get; set; }

        public Resolver Resolver { get; set; }
    }
}
