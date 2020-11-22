using BlazorGame.Data;
using Shouldly;
using System;
using System.Linq;

namespace BlazorGame.Tests
{
    public class CannotJoinAfterCardsDealt : TestBase
    {
        Game _game;
        Player _player;

        public void GivenANewGameWithPlayers()
        {
            _game = new(1000, new Player("Darren", "Darren"));
        }

        public void WhenTheCardsAreDealt()
        {
            _game.DealCards();
        }

        public void ThenAnotherPlayerCannotBeAdded()
        {
            _player = new Player("Other", "Other");
            Should.Throw<InvalidOperationException>(() => _player.Join(_game))
                .Message.ShouldBe("Cards have been dealt");
        }

        public void AndTheyShouldNotBeInTheGame()
        {
            _game.Players.Any(x => x.UserId == _player.UserId).ShouldBeFalse();
        }
    }
}
