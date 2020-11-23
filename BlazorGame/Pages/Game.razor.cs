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
        GameSessionService _sessionService { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }

        Lazy<DotNetObjectReference<Game>> _serverReference
        {
            get
            {
                return new(() => DotNetObjectReference.Create(this));
            }
        }

        PlayerSessionModel? _currentSession = null;
        List<Player> _currentPlayers = new();
        bool _canDeal = false;

        private async Task OnJoinGame(JoinGameModel joinGameModel)
        {
            var connectionId = await JS.InvokeAsync<string>("Game.GetConnectionId");

            _currentSession = joinGameModel.Mode switch
            {
                JoinMode.CreateNew => await _sessionService.CreateGame(connectionId, joinGameModel.Username, joinGameModel.PINCode),
                _ => await _sessionService.JoinGame(connectionId, joinGameModel.Username, joinGameModel.PINCode)
            };

            if (_currentSession is not null)
            {
                _canDeal = await _sessionService.CanDeal(_currentSession.PinCode);
                _currentPlayers = await _sessionService.GetPlayers(_currentSession.PinCode);
                await JS.InvokeVoidAsync("Game.InitializeGameState", _serverReference.Value);
            }
        }

        private async Task OnDealCards()
        {
            
            if (_currentSession is not null)
            {
                _canDeal = await _sessionService.CanDeal(_currentSession.PinCode);
                if(_canDeal)
                {
                    var connectionId = await JS.InvokeAsync<string>("Game.GetConnectionId");
                    await _sessionService.DealCards(connectionId, _currentSession.PinCode);
                    _canDeal = false;
                }
            }
        }

        private async Task OnLeaveGame()
        {
            if (_currentSession is not null)
            {
                var connectionId = await JS.InvokeAsync<string>("Game.GetConnectionId");
                await _sessionService.LeaveGame(connectionId, _currentSession.PinCode);
                _currentSession = null;
                _canDeal = false;
            }
        }

        [JSInvokable("RefreshGame")]
        public async Task RefreshGame()
        {
            if (_currentSession is not null)
            {
                _currentPlayers = await _sessionService.GetPlayers(_currentSession.PinCode);
                _canDeal = await _sessionService.CanDeal(_currentSession.PinCode);
            }

            StateHasChanged();
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
