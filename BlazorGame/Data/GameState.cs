using BlazorGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorGame.Data
{
    static class Animals
    {
        public const string Tiger = nameof(Tiger);
        public const string Panda = nameof(Panda);
        public const string Monkey = nameof(Monkey);
        public const string Spider = nameof(Spider);
    }

    public enum GameStatus
    {
        None,
        Open,
        Complete
    }

    public class Game
    {
        List<string> _animals = new() { Animals.Monkey, Animals.Panda, Animals.Spider, Animals.Tiger };
        List<string> _colors = new() { "primary", "secondary", "danger", "warning" };
        List<string> _suits = new() { "monkey", "panda", "spider", "tiger" };

        List<Player> _players = new();
        List<Card> _cards = new();
        private int _currentTurnIndex = -1;

        public Guid Id { get; }
        public int PinCode { get; }
        public bool HasDealtCards => _currentTurnIndex >= 0;
        public GameStatus State { get; private set; } = GameStatus.None;
        public List<Player> Players { get => _players; }
        public Card? Upcard { get; private set; }
        public string MatchingPlayerId { get; private set; } = "";
        public Card? MatchingCard { get; private set; }
        public string GameCreatorId { get; init; }
        public string GameCreatorName { get; init; }


        public Game(int pinCode, Player creator)
        {
            Id = Guid.NewGuid();
            PinCode = pinCode;
            _players.Add(creator);
            GameCreatorId = creator.UserId;
            GameCreatorName = creator.Name;
            State = GameStatus.Open;
            _cards = BuildDeck();
        }

        public bool IsComplete
        {
            get
            {
                return HasDealtCards && Players.All(x => !x.Hand.Any());
            }
        }

        public string ActivePlayerId
        {
            get
            {
                if (_currentTurnIndex < 0)
                    return "";

                _currentTurnIndex %= _players.Count;

                return _players.Skip(_currentTurnIndex).Take(1).First().UserId;
            }
        }

        public void CompleteTurn()
        {
            if (_currentTurnIndex < 0)
                throw new InvalidOperationException("Cards have not been dealt.");

            if (++_currentTurnIndex == _players.Count)
                _currentTurnIndex = 0;
        }

        internal bool TryPlayCard(string userId, Card card)
        {
            if (string.IsNullOrEmpty(ActivePlayerId))
                throw new InvalidOperationException("Cards have not been dealt.");

            var player = Players.Single(x => x.UserId == userId);

            if (ActivePlayerId == userId && Upcard is null)
            {
                Upcard = card;
            } else if (ActivePlayerId != userId && Upcard is not null && Upcard.Name == card.Name)
            {
                MatchingPlayerId = userId;
                MatchingCard = card;
            }
            else
            {
                return false;
            }

            player.Hand.Remove(card);
            return true;
        }

        internal void NextTurn()
        {
            if (Upcard is null || MatchingCard is null)
            {
                return;
            }

            Upcard = null;
            MatchingCard = null;
            _currentTurnIndex++;
        }

        private List<Card> BuildDeck()
        {
            var list = new List<Card>();
            for(var i = 0; i < _animals.Count; i++)
            {
                list.Add(new(_animals[i], _colors[i], _suits[i]));
                list.Add(new(_animals[i], _colors[i], _suits[i]));
            }
            return list;
        }

        public void DealCards()
        {
            if (HasDealtCards)
                throw new InvalidOperationException("Cards have been dealt");

            if (State is GameStatus.Complete)
            {
                throw new InvalidOperationException("Game is closed");
            }

            if (!_players.Any())
            {
                return;
            }

            _players.ForEach(p => p.Hand.Clear());
            int i = 0;

            Shuffle().ForEach((card) => {
                var p = i % _players.Count;
                _players[p].Hand.Add(card);
                i++;
            });

            _currentTurnIndex = 0;
        }

        public Game Reset()
        {
            Upcard = null;
            MatchingCard = null;
            _currentTurnIndex = -1;
            return this;
        }


        public Game AddPlayer(Player player)
        {
            if (_players.Any(x => x.UserId == player.UserId))
                throw new InvalidOperationException("Player has already joined the game");

            if (_players.Any(x => x.Name == player.Name))
                throw new InvalidOperationException("Username is already in use");

            if (HasDealtCards)
                throw new InvalidOperationException("Cards have been dealt");

            _players.Add(player);
            return this;
        }

        private List<Card> Shuffle()
        {
            // TODO: Add shuffle logic
            return _cards;
        }

        public Player? RetirePlayer(string userId)
        {
            if (!_players.Any(x => x.UserId == userId))
                return null;

            var player = _players.Single(x => x.UserId == userId);
            
            var cards = player.Hand; // TODO: do something with these
            player.Hand.Clear();
            _players.Remove(player);

            return player;
        }

        public List<CardHand> Hands => Players.Select(x => new CardHand(x.UserId, x.Name)
        {
            Cards = x.Hand
        }).ToList();

        public string GetPlayerName(string userId)
        {
            var player = Players.FirstOrDefault(x => x.UserId == userId);
            return player is null ? "" : player.Name;
        }
    }

    public record Card(string Name, string Color, string Icon);

    public class Player
    {
        Game? _game;
        
        public string Name { get; init; }
        public string UserId { get; init; }

        public Player(string userId, string name)
        {
            UserId = userId;
            Name = name;
        }

        public List<Card> Hand { get; init; } = new();

        public void Join(Game game)
        {
            _game = game.AddPlayer(this);
        }

        public void LeaveGame()
        {
            if (_game is not null)
            {
                _game = null;
            }
        }
    }
}
