using BlazorGame.Data;
using Shouldly;

namespace BlazorGame.Tests.Dealing
{
    public class AfterDealingTheCurrentTurnIsNotNull : TestBase
    {
        Game _game;
        Player _player;

        public void GivenANewGame()
        {
            _player = new Player("Darren", "Darren");
            _game = new(1000, _player);
        }

        public void WhenTheCardsAreDealt()
        {
            _game.DealCards();
        }

        public void ThenTheCurrentTurnIsNotNull()
        {
            _game.CurrentTurn.ShouldBe(_player);
        }
    }
}
