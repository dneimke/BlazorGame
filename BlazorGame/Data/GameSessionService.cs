using BlazorGame.Hubs;
using BlazorGame.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGame.Data
{
    public class GameSessionService
    {
        private readonly IHubContext<GameHub> _hubContext;
        
        // adding by PIN Code for now
        private Dictionary<Guid, Game> _currentGames = new();

        public GameSessionService(IHubContext<GameHub> hubContext) => _hubContext = hubContext;

        public Task<bool> CanDeal(int pinCode)
        {
            if (!TryGetGame(pinCode, out var game))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(game!.CanDealCards);
        }

        public async Task<List<Player>> GetPlayers(int pinCode)
        {
            if (!TryGetGame(pinCode, out var game))
            {
                return new();
            }

            return await Task.FromResult(game!.Players);
        }

        public async Task<GameCreatedModel> CreateGame(string userId, string userName, int pinCode)
        {
            var player = new Player(userId, userName);
            var game = new Game(pinCode, player);

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
            if(!TryGetGame(pinCode, out var game)) {
                return null;
            }
            
            var player = new Player(userId, userName);
            player.Join(game!);

            var session = new PlayerJoinedModel
            {
                GameSessionId = game!.Id,
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
            if (!TryGetGame(pinCode, out var game))
            {
                return;
            }

            game!.RetirePlayer(userId);

            await _hubContext.Groups.RemoveFromGroupAsync(userId, game.Id.ToString());
            await _hubContext.Clients.Group(game.Id.ToString())
                .SendAsync("PlayerRetired", new { UserId = userId });
        }

        public async Task DealCards(string userId, int pinCode)
        {
            if (!TryGetGame(pinCode, out var game))
            {
                return;
            }

            game!.DealCards();

            await _hubContext.Clients.Group(game.Id.ToString())
                .SendAsync("GameStateChanged", new DealtCardsModel(game.Id, game.CanDealCards, game.Hands));
        }

        // Crude implementation assumes unique PINCode
        private bool TryGetGame(int pinCode, out Game? game)
        {
            game = null;
            var item = _currentGames.FirstOrDefault(x => x.Value.PinCode == pinCode && x.Value.State == GameStatus.Open);
            if (item.Key != Guid.Empty)
            {
                game = item.Value;
                return true;
            }

            return false;
        }
    }
}
