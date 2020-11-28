using BlazorGame.Data;
using BlazorGame.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System;
using System.Linq;
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

        string GameUrl => string.Format($"{NavManager.BaseUri}?gameId={CurrentGameId}");
        CardHand? MyHand => _gameState?.Hands.Where(x => x.UserId == UserId).FirstOrDefault();

        bool CanDealCards => _gameState.HasDealtCards ? false :
            _gameState?.GameCreatorId == UserId ? 
                _gameState.Hands.Count > 1 ? true : false
                    : false;

        bool CanPlayAgain => _gameState.IsComplete && _gameState?.GameCreatorId == UserId;
        bool CanMoveNext => _gameState.CanPlayNextCard && _gameState?.GameCreatorId == UserId;
        bool ShowEntryScreen => _gameState == null;
        bool ShowHand => MyHand != null && MyHand.Cards.Any();

        string TurnMessage
        {
            get
            {
                if ((bool)(_gameState?.HasDealtCards))
                {
                    if (_gameState.UpCard?.Card is null)
                    {
                        return _gameState.ActivePlayerId == UserId ? "It's your turn to lead with a card" :
                            $"Waiting for {_gameState.UpCard.Player} to lead with a card";
                    }
                    else if(_gameState.MatchingCard?.Card is null)
                    {
                        return _gameState.ActivePlayerId == UserId ? "Waiting for the player with the matching card" :
                            "Can you play a matching card from your hand?";
                    }
                    else
                    {
                        return _gameState.IsComplete ? "Game is complete" : "Cards are matched.";
                    }
                }
                else if(CanDealCards)
                {
                    return _gameState?.GameCreatorId == UserId ? "Deal the cards to begin the game"
                        : $"Waiting for {_gameState.GameCreatorName} to deal the cards";
                } else
                {
                    return "Waiting for the game to start.";
                }
            }
        }

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

            if(_gameState != null)
            {
                CurrentGameId = _gameState.GameSessionId;
            }

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
                await _gameService.RestartGame(UserId, CurrentGameId, _gameState.PinCode);
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
