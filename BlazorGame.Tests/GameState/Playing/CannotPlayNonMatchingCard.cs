using BlazorGame.Data;
using BlazorGame.Models;
using BlazorGame.Tests.Helpers;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGame.Tests.Playing
{
    public class CannotPlayNonMatchingCard : TestBase
    {
        GameStateModel _game;
        Player _player;
        Player _otherPlayer;
        int _pinCode = 1000;
        bool _playResult = false;
        int _initialCount = -1;
        Card _playedCard;


        public CannotPlayNonMatchingCard(ContainerFixture fixture) : base(fixture)
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

        public async Task AndGivenTheFirstCardHasBeenPlayed()
        {
            _playedCard = _game.Hands[0].Cards.First();
            await _fixture.GameService.TryPlayCard(_player.UserId, _playedCard, _game.GameSessionId, _game.PinCode);
        }

        public async Task WhenTheOtherPlayerAttemptsToPlayANonMatchingCard()
        {
            var nonMatchingCard = _game.Hands[1].Cards.First(x => x.Name != _playedCard.Name);
            _playResult = await _fixture.GameService.TryPlayCard(_otherPlayer.UserId, nonMatchingCard, _game.GameSessionId, _game.PinCode);
        }

        public void ThenTheCardIsNonPlayed()
        {
            _playResult.ShouldBeFalse();
        }

        public void AndTheCardIsNonRemovedFromTheOtherPlayersHand()
        {
            _game.Hands[1].Cards.Count.ShouldBe(_initialCount);
        }
    }
}
