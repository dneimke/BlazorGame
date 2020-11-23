using BlazorGame.Data;
using BlazorGame.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorGame.Pages
{
    public partial class Game
    {
        [Inject]
        GameSessionService _gameService { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }

        Lazy<DotNetObjectReference<Game>> _serverReference
        {
            get
            {
                return new(() => DotNetObjectReference.Create(this));
            }
        }

        PlayerSessionModel? _currentGame = null;
        List<Player> _currentPlayers = new();
        bool _canDeal = false;

        private async Task OnJoinGame(JoinGameModel joinGameModel)
        {
            var connectionId = await JS.InvokeAsync<string>("Game.GetConnectionId");

            _currentGame = joinGameModel.Mode switch
            {
                JoinMode.CreateNew => await _gameService.CreateGame(connectionId, joinGameModel.Username, joinGameModel.PINCode),
                _ => await _gameService.JoinGame(connectionId, joinGameModel.Username, joinGameModel.PINCode)
            };

            if (_currentGame is not null)
            {
                _canDeal = await _gameService.CanDeal(_currentGame.PinCode);
                _currentPlayers = await _gameService.GetPlayers(_currentGame.PinCode);
                await JS.InvokeVoidAsync("Game.InitializeGameState", _serverReference.Value);
            }
        }

        private async Task OnDealCards()
        {
            
            if (_currentGame is not null)
            {
                _canDeal = await _gameService.CanDeal(_currentGame.PinCode);
                if(_canDeal)
                {
                    var connectionId = await JS.InvokeAsync<string>("Game.GetConnectionId");
                    await _gameService.DealCards(connectionId, _currentGame.PinCode);
                    _canDeal = false;
                }
            }
        }

        private async Task OnLeaveGame()
        {
            if (_currentGame is not null)
            {
                var connectionId = await JS.InvokeAsync<string>("Game.GetConnectionId");
                await _gameService.LeaveGame(connectionId, _currentGame.PinCode);
                _currentGame = null;
                _canDeal = false;
            }
        }

        [JSInvokable("RefreshGame")]
        public async Task RefreshGame()
        {
            if (_currentGame is not null)
            {
                _currentPlayers = await _gameService.GetPlayers(_currentGame.PinCode);
                _canDeal = await _gameService.CanDeal(_currentGame.PinCode);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (_serverReference.IsValueCreated)
            {
                _serverReference.Value.Dispose();
            }
        }
    }
}
