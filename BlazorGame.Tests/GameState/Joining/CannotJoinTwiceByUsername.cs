using BlazorGame.Data;
using BlazorGame.Tests.Helpers;
using Shouldly;
using System;

namespace BlazorGame.Tests.Joining
{
    public class CannotJoinTwiceByUsername : TestBase
    {
        Game _game;

        public CannotJoinTwiceByUsername(ContainerFixture fixture) : base(fixture)
        {

        }

        public void GivenANewGame()
        {
            _game = new(1000, new Player("Darren", "Darren"), _fixture.GetService<ICardProvider>());
        }

        public void WhenANewPlayerJoins()
        {
            var player = new Player("New Player", "New Player");
            player.Join(_game);
        }

        public void ThenTheyCannotJoinTwice()
        {
            var player = new Player("Different New Player", "New Player");
            Should.Throw<InvalidOperationException>(() => player.Join(_game))
                .Message.ShouldBe("Username is already in use");
        }
    }
}
