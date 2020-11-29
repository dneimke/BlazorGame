using BlazorGame.Data;
using BlazorGame.Models;
using BlazorGame.Tests.Helpers;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGame.Tests.Playing
{
    public class CannotPlayOutOfTurn : TestBase
    {
        GameStateModel _game;
        Player _player;
        Player _otherPlayer;
        int _pinCode = 1000;
        bool _playResult = false;
        int _initialCount = -1;


        public CannotPlayOutOfTurn(ContainerFixture fixture) : base(fixture)
        {
            
        }

        public async Task GivenANewGame()
        {
            _player = new("Darren", "Darren");
           _game = await _fixture.GameService.CreateGame(_player.UserId, _player.Name, _pinCode);
        }

        public async Task AndGivenAnotherPlayerHasJoinedTheGame()
        {
            _otherPlayer = new("Other", "Other");
            _game = await _fixture.GameService.JoinGame(_otherPlayer.UserId, _otherPlayer.Name, _game.GameSessionId, _game.PinCode);
        }

        public async Task AndGivenTheCardsAreDealt()
        {
            _game = await _fixture.GameService.DealCards(_player.UserId, _game.GameSessionId, _game.PinCode);
            _initialCount = _game.Hands[1].Cards.Count;
        }

        public async Task WhenTheOtherPlayerAttemptsToPlayTheFirstCard()
        {
            _playResult = await _fixture.GameService.TryPlayCard(_otherPlayer.UserId, _game.Hands[1].Cards.First(), _game.GameSessionId, _game.PinCode);
        }

        public void ThenTheCardIsNotPlayed()
        {
            _playResult.ShouldBeFalse();
        }

        public void AndTheCardIsNotRemovedFromTheOtherPlayersHand()
        {
            _game.Hands[1].Cards.Count.ShouldBe(_initialCount);
        }
    }
}
