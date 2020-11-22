using BlazorGame.Data;
using Shouldly;
using System;

namespace BlazorGame.Tests
{
    public class CannotJoinTwice : TestBase
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

        public void ThenTheyCannotJoinTwice()
        {
            Should.Throw<InvalidOperationException>(() => _player.Join(_game))
                .Message.ShouldBe("Player has already joined the game");
        }
    }
}
