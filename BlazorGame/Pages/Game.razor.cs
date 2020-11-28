using BlazorGame.Data;
using BlazorGame.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorGame.Pages
{
    public partial class Game
    {
        [Inject] GameSessionService _gameService { get; set; }
        [Inject] IJSRuntime JS { get; set; }
        [Inject] NavigationManager NavManager { get; set; }

        Guid? gameId;

        GameStateModel? _gameState = null;
        public string UserId { get; set; } = "";
        public string Username { get; set; } = "";
        private Guid CurrentGameId { get; set; }

        string _gameUrl => string.Format($"{NavManager.BaseUri}?gameId={CurrentGameId}");

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
                _ => await _gameService.JoinGame(UserId, Username, gameId.GetValueOrDefault(), joinGameModel.PINCode)
            };

            CurrentGameId = _gameState.GameSessionId;

            StateHasChanged();
        }

        private async Task OnCardClicked(Card card)
        {
            if (await _gameService.TryPlayCard(UserId, card, CurrentGameId, _gameState!.PinCode)) {
                await JS.InvokeVoidAsync("console.info", $"User played {card.Name}");
            } else
            {
                await JS.InvokeVoidAsync("console.warn", $"Cannot play {card.Name}");
            }
        }

        private async Task OnDealCards()
        {
            if (_gameState is not null)
            {
                await _gameService.DealCards(UserId, CurrentGameId, _gameState.PinCode);
            }
        }

        private async Task OnPlayNext()
        {
            if (_gameState is not null)
            {
                await _gameService.NextTurn(UserId, CurrentGameId, _gameState.PinCode);
            }
        }

        private async Task OnNewGame()
        {
            if (_gameState is not null)
            {
                await _gameService.NewGame(UserId, CurrentGameId, _gameState.PinCode);
            }
        }

        private async Task OnLeaveGame()
        {
            if (_gameState is not null)
            {
                await _gameService.LeaveGame(UserId, CurrentGameId, _gameState.PinCode);
                _gameState = null;
                StateHasChanged();
            }
        }

        [JSInvokable("RefreshGame")]
        public async Task RefreshGame()
        {
            if (_gameState is not null)
            {
                _gameState = await _gameService.GetCurrentState(CurrentGameId, _gameState.PinCode);
                StateHasChanged();
            }
        }

        protected override void OnInitialized()
        {
            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("gameId", out var qGame))
            {
                if(Guid.TryParse(qGame, out var initialGame)) 
                {
                    gameId = initialGame;
                }
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
