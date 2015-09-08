namespace Stashbox.Overrides
{
    public class NamedOverride : Override
    {
        public string OverrideName { get; private set; }

        public NamedOverride(string name, object value)
            : base(value)
        {
            this.OverrideName = name;
        }
    }
}