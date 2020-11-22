using TestStack.BDDfy;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace BlazorGame.Tests
{
    [Collection("Sequential")]
    public class TestBase
    {
        [Fact]
        public virtual void ExecuteScenario()
        {
            this.BDDfy(GetType().Name);
        }
    }
}
