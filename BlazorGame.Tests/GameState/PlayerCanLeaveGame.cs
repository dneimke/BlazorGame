using BlazorGame.Data;
using Shouldly;
using System.Linq;

namespace BlazorGame.Tests
{
    public class PlayerCanLeaveGame : TestBase
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

        public void AndWhenTheyThenLeave()
        {
            _game.RetirePlayer(_player.UserId);
            _player.LeaveGame();
        }

        public void ThenTheyShouldNotBeInTheGame()
        {
            _game.Players.Any(x => x.UserId == _player.UserId).ShouldBeFalse();
        }
    }
}
