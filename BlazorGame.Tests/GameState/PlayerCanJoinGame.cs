using BlazorGame.Data;
using Shouldly;
using System.Linq;

namespace BlazorGame.Tests
{
    public class PlayerCanJoinGame : TestBase
    {
        Game _game;
        Player _player;

        public void GivenANewGame()
        {
            _game = new(1000, new Player("Darren", "Darren"));
        }

        public void WhenANewPlayerJoins()
        {
            _player = new Player("New Player", "New Player");
            _player.Join(_game);
        }

        public void ThenTheyShouldBeInTheGame()
        {
            _game.Players.Any(x => x.UserId == _player.UserId).ShouldBeTrue();
        }
    }
}
