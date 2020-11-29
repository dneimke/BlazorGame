using BlazorGame.Data;
using BlazorGame.Tests.Helpers;
using Shouldly;
using System.Linq;

namespace BlazorGame.Tests.Joining
{
    public class PlayerCanLeaveGame : TestBase
    {
        Game _game;
        Player _player;

        public PlayerCanLeaveGame(ContainerFixture fixture) : base(fixture)
        {

        }

        public void GivenANewGame()
        {
            _game = new(1000, new Player("Darren", "Darren"), _fixture.GetService<ICardProvider>());
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
