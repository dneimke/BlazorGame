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
        private readonly IHubContext<SessionHub> _hubContext;
        // adding by PIN Code for now
        private Dictionary<int, List<CurrentSessionModel>> _currentGames = new();

        public GameSessionService(IHubContext<SessionHub> hubContext) => _hubContext = hubContext;

        public async Task<CurrentSessionModel> CreateGame(string userId, string userName, int pinCode)
        {
            var gameId = Guid.NewGuid();
            var session = new CurrentSessionModel
            {
                GameSessionId = gameId,
                UserId = userId,
                Username = userName,
                PINCode = pinCode,
                Role = GameRole.Creator
            };

            // For now we'll just overwrite any existing game
            if(_currentGames.ContainsKey(pinCode))
            {
                _currentGames[pinCode] = new() { session };
            }
            else
            {
                _currentGames.Add(pinCode, new() { session });
            }

            await _hubContext.Groups.AddToGroupAsync(userId, gameId.ToString());
            await _hubContext.Clients.Group(gameId.ToString())
                .SendAsync("GameCreated", $"{userId} has created the game {session.PINCode}.");

            return session;
        }

        public async Task<CurrentSessionModel?> JoinGame(string userId, string userName, int pinCode)
        {
            if(!_currentGames.ContainsKey(pinCode))
            {
                return null;
            }

            var currentSessions = _currentGames[pinCode];
            if(currentSessions.Count != 1)
            {
                return null;
            }

            var gameId = currentSessions[0].GameSessionId;
            var session = new CurrentSessionModel
            {
                GameSessionId = gameId,
                UserId = userId,
                Username = userName,
                PINCode = pinCode,
                Role = GameRole.Creator
            };

            currentSessions.Add(session);

            await _hubContext.Groups.AddToGroupAsync(userId, gameId.ToString());
            await _hubContext.Clients.Group(gameId.ToString())
                .SendAsync("GameCreated", $"{userId} has joined the game {session.PINCode}.");

            return session;
        }

        public async Task LeaveGame(string userId, int pinCode)
        {
            if (!_currentGames.ContainsKey(pinCode))
            {
                return;
            }

            var currentSessions = _currentGames[pinCode];

            var session = currentSessions.FirstOrDefault(x => x.UserId == userId);

            if (session == null)
                return;

            currentSessions.Remove(session);

            if (currentSessions.Count == 0)
                _currentGames.Remove(pinCode);

            await _hubContext.Groups.RemoveFromGroupAsync(userId, session.GameSessionId.ToString());
            await _hubContext.Clients.Group(session.GameSessionId.ToString())
                .SendAsync("UserLoggedOff", $"{userId} has left the game {session.PINCode}.");
        }
    }
}
