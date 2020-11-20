using BlazorGame.Data;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BlazorGame.Hubs
{
    public class GameHub : Hub
    {
        readonly GameSessionService _gameService;

        public GameHub(GameSessionService sessionService) => _gameService = sessionService;

        public async Task CreateGame(string userName, int pinCode) => await _gameService.CreateGame(Context.ConnectionId, userName, pinCode);
        public async Task JoinGame(string userName, int pinCode) => await _gameService.JoinGame(Context.ConnectionId, userName, pinCode);
        public async Task LeaveGame(int pinCode) => await _gameService.LeaveGame(Context.ConnectionId, pinCode);
    }
}
