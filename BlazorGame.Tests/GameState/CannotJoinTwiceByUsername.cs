using BlazorGame.Data;
using Shouldly;
using System;

namespace BlazorGame.Tests
{
    public class CannotJoinTwiceByUsername : TestBase
    {
        Game _game;

        public void GivenANewGame()
        {
            _game = new(1000, new Player("Darren", "Darren"));
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
