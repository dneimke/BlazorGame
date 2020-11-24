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

        GameStateModel? _gameState = null;
        public string UserId { get; set; } = "";
        public string Username { get; set; } = "";

        Lazy<DotNetObjectReference<Game>> _serverReference
        {
            get
            {
                return new(() => DotNetObjectReference.Create(this));
            }
        }

        private async Task OnJoinGame(JoinGameModel joinGameModel)
        {
            UserId = await JS.InvokeAsync<string>("Game.GetConnectionId");
            Username = joinGameModel.Username;

            _gameState = joinGameModel.Mode switch
            {
                JoinMode.CreateNew => await _gameService.CreateGame(UserId, Username, joinGameModel.PINCode),
                _ => await _gameService.JoinGame(UserId, Username, joinGameModel.PINCode)
            };

            StateHasChanged();
        }

        private async Task OnDealCards()
        {
            if (_gameState is not null)
            {
                await _gameService.DealCards(UserId, _gameState.PinCode);
            }
        }

        private async Task OnLeaveGame()
        {
            if (_gameState is not null)
            {
                await _gameService.LeaveGame(UserId, _gameState.PinCode);
                _gameState = null;
                StateHasChanged();
            }
        }

        [JSInvokable("RefreshGame")]
        public async Task RefreshGame()
        {
            if (_gameState is not null)
            {
                _gameState = await _gameService.GetCurrentState(_gameState.PinCode);
                StateHasChanged();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("Game.InitializeGameState", _serverReference.Value);
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
