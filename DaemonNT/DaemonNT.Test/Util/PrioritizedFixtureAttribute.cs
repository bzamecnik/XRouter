// from Xunit.net samples
// Microsoft Public License (Ms-PL)

namespace Xunit.Extensions
{
    public class PrioritizedFixtureAttribute : RunWithAttribute
    {
        public PrioritizedFixtureAttribute() : base(typeof(OrderedFixtureClassCommand)) { }
    }
}