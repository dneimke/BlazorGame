using BlazorGame.Data;
using Shouldly;

namespace BlazorGame.Tests.Dealing
{
    public class CurrentTurnIsNullWhenNoPlayers : TestBase
    {
        Game _game;
        Player _player;

        public void GivenANewGame()
        {
            _player = new Player("Darren", "Darren");
            _game = new(1000, _player);
        }

        public void WhenTheLastPlayerLeaves()
        {
            _game.RetirePlayer(_player.UserId);
        }

        public void AndWhenTheCardsAreDealt()
        {
            _game.DealCards();
        }

        public void ThenTheCurrentTurnShouldBeNull()
        {
            _game.ActivePlayerId.ShouldBeEmpty();
        }
    }
}
