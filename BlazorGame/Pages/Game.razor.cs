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

        StartGameModel startGameModel = new();
        PlayerSessionModel? currentSession = null;
        List<Player> currentPlayers = new();

        private async Task OnEnterGame()
        {
            var connectionId = await JS.InvokeAsync<string>("Game.GetConnectionId");

            try
            {
                if (startGameModel.Mode == JoinMode.CreateNew)
                {
                    currentSession = await _sessionService.CreateGame(connectionId, startGameModel.Username, startGameModel.PINCode);
                }
                else
                {
                    currentSession = await _sessionService.JoinGame(connectionId, startGameModel.Username, startGameModel.PINCode);
                }

            }
            catch { }

            if (currentSession is not null)
            {
                currentPlayers = await _sessionService.GetPlayers(currentSession.PinCode);
                await JS.InvokeVoidAsync("Game.InitializeGameState", currentSession);
                startGameModel = new();
            }
        }

        private async Task OnLeaveGame()
        {
            if (currentSession is not null)
            {
                var connectionId = await JS.InvokeAsync<string>("Game.GetConnectionId");
                await _sessionService.LeaveGame(connectionId, currentSession.PinCode);
                startGameModel = new();
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

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
