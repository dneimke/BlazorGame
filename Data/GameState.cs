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
        Open,
        Complete
    }

    public class Game
    {
        List<string> _animals = new() { Animals.Monkey, Animals.Panda, Animals.Spider, Animals.Tiger };
        List<string> _colors = new() { "primary", "secondary", "danger", "warning" };
        List<string> _icons = new() { "🐵", "🐼", "🕷", "🐯" };

        List <Player> _players = new();
        List<Card> _cards = new();

        bool _hasDealtCards = false;

        public List<Card> Deck { get => _cards; }
        public List<Player> Players { get => _players; }
        public Guid Id { get; }
        public int PinCode { get; }
        public bool CanDealCards { 
            get
            {
                return _hasDealtCards is false;
            }
        }

        public List<CardHand> Hands => Players.Select(x => new CardHand
        {
            UserId = x.UserId,
            Cards = x.Hand
        }).ToList();


        public List<CardHand> GetHandByPlayer(string userId)
        {
            return Players.Where(x => x.UserId == userId).Select(x => new CardHand
            {
                UserId = x.UserId,
                Cards = x.Hand
            }).ToList();
        }

        private readonly Player? _creator;
        private GameStatus _state = GameStatus.Complete;

        public Game(int pinCode, Player creator)
        {
            Id = Guid.NewGuid();
            PinCode = pinCode;
            _players.Add(creator);
            _creator = creator;
            _state = GameStatus.Open;
            _cards = BuildDeck();
        }

        private List<Card> BuildDeck()
        {
            var list = new List<Card>();
            for(var i = 0; i < _animals.Count; i++)
            {
                list.Add(new(_animals[i], _colors[i], _icons[i]));
                list.Add(new(_animals[i], _colors[i], _icons[i]));
            }
            return list;
        }

        public void DealCards()
        {
            if (_hasDealtCards)
                throw new InvalidOperationException("Cards have been dealt");

            if (_state is GameStatus.Complete)
            {
                throw new InvalidOperationException("Game is closed");
            }

            if (!_players.Any())
            {
                _hasDealtCards = false;
                return;
            }

            _players.ForEach(p => p.Hand.Clear());
            int i = 0;

            Shuffle().ForEach((card) => {
                var p = i % _players.Count;
                _players[p].Hand.Add(card);
                i++;
            });

            _hasDealtCards = true;
        }


        public Game AddPlayer(Player player)
        {
            if (_players.Any(x => x.UserId == player.UserId))
                throw new InvalidOperationException("Player has already joined the game");

            if (_players.Any(x => x.Name == player.Name))
                throw new InvalidOperationException("Username is already in use");

            if (_hasDealtCards)
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
    }

    public record Card(string Name, string Color, string Icon);

    public class Player
    {
        Game? _game;
        readonly List<Card> _hand = new();

        public string Name { get; init; }
        public string UserId { get; init; }

        public Player(string userId, string name)
        {
            UserId = userId;
            Name = name;
        }

        public List<Card> Hand { get => _hand; }

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
