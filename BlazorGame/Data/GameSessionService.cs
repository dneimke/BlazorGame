﻿using BlazorGame.Hubs;
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

        public Task<bool> CanDeal(Guid gameId, int pinCode)
        {
            if (!TryGetGame(gameId, pinCode, out var game))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(!game.HasDealtCards);
        }

        public async Task<GameStateModel> CreateGame(string userId, string userName, int pinCode)
        {
            var player = new Player(userId, userName);
            var game = new Game(pinCode, player);
            var gameState = new GameStateModel(game);

            _currentGames[game.Id] = game;

            await _hubContext.Groups.AddToGroupAsync(userId, game.Id.ToString());
            await _hubContext.Clients.Group(game.Id.ToString())
                .SendAsync("GameCreated", gameState);

            return gameState;
        }

        internal async Task<bool> TryPlayCard(string userId, Card card, Guid gameId, int pinCode)
        {
            var result = false;
            if (!TryGetGame(gameId, pinCode, out var game))
            {
                return result;
            }

            if(game!.TryPlayCard(userId, card))
            {
                var gameState = CurrentState(game);

                await _hubContext.Clients.Group(game.Id.ToString())
                    .SendAsync("GameStateChanged", gameState);
            }

            return result;
        }

        internal async Task NewGame(string userId, Guid gameId, int pinCode)
        {
            if (TryGetGame(gameId, pinCode, out var game))
            {
                game!.Reset().DealCards();

                var gameState = CurrentState(game);

                await _hubContext.Clients.Group(game.Id.ToString())
                        .SendAsync("GameStateChanged", gameState);
            }
        }

        internal async Task NextTurn(string userId, Guid gameId, int pinCode)
        {
            if (TryGetGame(gameId, pinCode, out var game))
            {
                game!.NextTurn();
                var gameState = CurrentState(game);

                await _hubContext.Clients.Group(game.Id.ToString())
                    .SendAsync("GameStateChanged", gameState);

            }
        }

        public async Task<GameStateModel?> JoinGame(string userId, string userName, Guid gameId, int pinCode)
        {
            if(!TryGetGame(gameId, pinCode, out var game)) {
                return null;
            }
            
            var player = new Player(userId, userName);
            player.Join(game!);

            var gameState = CurrentState(game!);

            await _hubContext.Groups.AddToGroupAsync(userId, game!.Id.ToString());
            await _hubContext.Clients.Group(game.Id.ToString())
                .SendAsync("PlayerJoined", gameState);

            return gameState;
        }

        public async Task LeaveGame(string userId, Guid gameId, int pinCode)
        {
            if (TryGetGame(gameId, pinCode, out var game))
            {
                game!.RetirePlayer(userId);

                await _hubContext.Groups.RemoveFromGroupAsync(userId, game.Id.ToString());
                await _hubContext.Clients.Group(game.Id.ToString())
                    .SendAsync("PlayerRetired", new { UserId = userId });
            }
        }

        public async Task<GameStateModel?> GetCurrentState(Guid gameId, int pinCode)
        {
            if (!TryGetGame(gameId, pinCode, out var game))
            {
                return null;
            }

            return await Task.FromResult(CurrentState(game!));
        }

        public async Task DealCards(string userId, Guid gameId, int pinCode)
        {
            if (TryGetGame(gameId, pinCode, out var game))
            {
                game!.DealCards();
                var gameState = CurrentState(game);

                await _hubContext.Clients.Group(game.Id.ToString())
                    .SendAsync("GameStateChanged", gameState);
            }            
        }

        private static GameStateModel CurrentState(Game game)
        {
            return new GameStateModel(game)
            {
                UpCard = new(game.Upcard, game.GetPlayerName(game.ActivePlayerId)),
                MatchingCard = new(game.MatchingCard, game.GetPlayerName(game.MatchingPlayerId))
            };
        }

        // Crude implementation assumes unique PINCode
        private bool TryGetGame(Guid gameId, int pinCode, out Game? game)
        {
            game = null;
            var item = _currentGames.FirstOrDefault(x => x.Value.Id == gameId && x.Value.PinCode == pinCode && x.Value.State == GameStatus.Open);
            if (item.Key != Guid.Empty)
            {
                game = item.Value;
                return true;
            }

            return false;
        }
    }
}
