using BlazorGame.Tests.Helpers;
using TestStack.BDDfy;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace BlazorGame.Tests
{
    [Collection("Sequential")]
    public class TestBase : IClassFixture<ContainerFixture>
    {
        protected readonly ContainerFixture _fixture;

        public TestBase(ContainerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public virtual void ExecuteScenario()
        {
            this.BDDfy(GetType().Name);
        }
    }
}
