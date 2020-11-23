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

        ElementReference _dealButton;
        DotNetObjectReference<Game> _serverReference;

        PlayerSessionModel? currentSession = null;
        List<Player> currentPlayers = new();
        bool _isDirty = false;

        private async Task OnJoinGame(JoinGameModel joinGameModel)
        {
            var connectionId = await JS.InvokeAsync<string>("Game.GetConnectionId");

            try
            {
                if (joinGameModel.Mode == JoinMode.CreateNew)
                {
                    currentSession = await _sessionService.CreateGame(connectionId, joinGameModel.Username, joinGameModel.PINCode);
                }
                else
                {
                    currentSession = await _sessionService.JoinGame(connectionId, joinGameModel.Username, joinGameModel.PINCode);
                }

            }
            catch { }

            if (currentSession is not null)
            {
                currentPlayers = await _sessionService.GetPlayers(currentSession.PinCode);
                _isDirty = true;
            }
        }

        private async Task OnLeaveGame()
        {
            if (currentSession is not null)
            {
                var connectionId = await JS.InvokeAsync<string>("Game.GetConnectionId");
                await _sessionService.LeaveGame(connectionId, currentSession.PinCode);
                currentSession = null;
            }
        }

        [JSInvokable("RefreshGame")]
        public async Task RefreshGame()
        {
            if (currentSession is not null)
            {
                currentPlayers = await _sessionService.GetPlayers(currentSession.PinCode);
            }

            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _serverReference = DotNetObjectReference.Create(this);
            }

            if(_isDirty && currentSession != null)
            {
                await JS.InvokeVoidAsync("Game.InitializeGameState", currentSession, _dealButton, _serverReference);
                _isDirty = false;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (_serverReference != null)
            {
                _serverReference.Dispose();
            }
        }
    }
}
