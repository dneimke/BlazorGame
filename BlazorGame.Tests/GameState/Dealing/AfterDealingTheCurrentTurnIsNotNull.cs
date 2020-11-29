using BlazorGame.Data;
using BlazorGame.Tests.Helpers;
using Shouldly;

namespace BlazorGame.Tests.Dealing
{
    public class AfterDealingTheCurrentTurnIsNotNull : TestBase
    {
        Game _game;
        Player _player;

        public AfterDealingTheCurrentTurnIsNotNull(ContainerFixture fixture) : base(fixture)
        {
            
        }

        public void GivenANewGame()
        {
            _player = new Player("Darren", "Darren");
            _game = new(1000, _player, _fixture.GetService<ICardProvider>());
        }

        public void WhenTheCardsAreDealt()
        {
            _game.DealCards();
        }

        public void ThenTheCurrentTurnIsNotNull()
        {
            _game.ActivePlayerId.ShouldBe(_player.UserId);
        }
    }
}
