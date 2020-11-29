using BlazorGame.Data;
using BlazorGame.Tests.Helpers;
using Shouldly;
using System;

namespace BlazorGame.Tests.Joining
{
    public class CannotJoinTwice : TestBase
    {
        Game _game;
        Player _player;

        public CannotJoinTwice(ContainerFixture fixture) : base(fixture)
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

        public void ThenTheyCannotJoinTwice()
        {
            Should.Throw<InvalidOperationException>(() => _player.Join(_game))
                .Message.ShouldBe("Player has already joined the game");
        }
    }
}
