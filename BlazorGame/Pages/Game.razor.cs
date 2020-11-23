using BlazorGame.Data;
using BlazorGame.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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

        private async Task OnRefreshGame()
        {
            var connectionId = await JS.InvokeAsync<string>("Game.GetConnectionId");
            if (currentSession is not null)
            {
                currentPlayers = await _sessionService.GetPlayers(currentSession.PinCode);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("StartTimer");
            }

            if(_isDirty && currentSession != null)
            {
                await JS.InvokeVoidAsync("Game.InitializeGameState", currentSession, _dealButton);
                _isDirty = false;
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
