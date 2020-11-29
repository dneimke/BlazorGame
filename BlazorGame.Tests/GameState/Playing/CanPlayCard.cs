using BlazorGame.Data;
using BlazorGame.Models;
using BlazorGame.Tests.Helpers;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGame.Tests.Playing
{
    public class CanPlayCard : TestBase
    {
        GameStateModel _game;
        Player _player;
        int _pinCode = 1000;
        int _initialCount = -1;
        
        public CanPlayCard(ContainerFixture fixture) : base(fixture)
        {
            
        }

        public async Task GivenANewGame()
        {
            _player = new Player("Darren", "Darren");
           _game = await _fixture.GameService.CreateGame(_player.UserId, _player.Name, _pinCode);
        }

        public async Task AndGivenAnotherPlayerHasJoinedTheGame()
        {
            _game = await _fixture.GameService.JoinGame("Another", "Another", _game.GameSessionId, _game.PinCode);
        }

        public async Task AndGivenTheCardsAreDealt()
        {
            _game = await _fixture.GameService.DealCards(_player.UserId, _game.GameSessionId, _game.PinCode);
            _initialCount = _game.Hands[0].Cards.Count;
        }

        public async Task WhenTheFirstCardIsPlayed()
        {
            await _fixture.GameService.TryPlayCard(_player.UserId, _game.Hands[0].Cards.First(), _game.GameSessionId, _game.PinCode);
        }

        public void ThenTheCardIsRemovedFromTheHand()
        {
            _game.Hands[0].Cards.Count.ShouldBe(_initialCount - 1);
        }
    }
}
