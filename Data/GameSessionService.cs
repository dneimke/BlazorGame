using BlazorGame.Hubs;
using BlazorGame.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BlazorGame.Data
{
    public class GameSessionService
    {
        private readonly IHubContext<GameHub> _hubContext;
        
        // adding by PIN Code for now
        private Dictionary<Guid, GameState> _currentGames = new();

        public GameSessionService(IHubContext<GameHub> hubContext) => _hubContext = hubContext;

        public async Task<GameCreatedModel> CreateGame(string userId, string userName, int pinCode)
        {
            var player = new Player(userId, userName);
            var game = new GameState(pinCode, player);

            var session = new GameCreatedModel
            {
                GameSessionId = game.Id,
                UserId = userId,
                Username = userName,
                PinCode = pinCode,
                Role = GameRole.Creator
            };

            _currentGames[game.Id] = game;

            await _hubContext.Groups.AddToGroupAsync(userId, game.Id.ToString());
            await _hubContext.Clients.Group(game.Id.ToString())
                .SendAsync("GameCreated", session);

            return session;
        }

        public async Task<PlayerJoinedModel?> JoinGame(string userId, string userName, int pinCode)
        {
            // Crude implementation assumes unique PINCode
            var item = _currentGames.FirstOrDefault(x => x.Value.PinCode == pinCode);
            if(item.Key == Guid.Empty)
            {
                return null;
            }

            var game = item.Value;
            
            var player = new Player(userId, userName);
            player.Join(game);

            var session = new PlayerJoinedModel
            {
                GameSessionId = game.Id,
                UserId = userId,
                Username = userName,
                PinCode = pinCode,
                Role = GameRole.Player
            };

            await _hubContext.Groups.AddToGroupAsync(userId, game.Id.ToString());
            await _hubContext.Clients.Group(game.Id.ToString())
                .SendAsync("PlayerJoined", session);

            return session;
        }

        public async Task LeaveGame(string userId, int pinCode)
        {
            // Crude implementation assumes unique PINCode
            var item = _currentGames.FirstOrDefault(x => x.Value.PinCode == pinCode);
            if (item.Key == Guid.Empty)
            {
                return;
            }

            var game = item.Value;
            game.RetirePlayer(userId);

            await _hubContext.Groups.RemoveFromGroupAsync(userId, game.Id.ToString());
            await _hubContext.Clients.Group(game.Id.ToString())
                .SendAsync("PlayerRetired", $"{userId} has left the game {pinCode}.");
        }
    }
}
