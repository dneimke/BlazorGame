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

    public class GameState
    {
        List<string> _animals = new() { Animals.Monkey, Animals.Panda, Animals.Spider, Animals.Tiger };
        List<Player> _players = new();
        List<Card> _cards = new();

        bool _hasDealtCards = false;

        public List<Card> Deck { get => _cards; }
        public List<Player> Players { get => _players; }
        public Guid Id { get; }
        public int PinCode { get; }
        private Player? _creator;
        private GameStatus _state = GameStatus.Complete;

        public GameState(int pinCode, Player creator)
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
            return _animals.SelectMany(x => Enumerable.Repeat(new Card(x), 2)).ToList();
        }

        public void DealCards()
        {
            if (_hasDealtCards)
                throw new InvalidOperationException("Cards have been dealt");

            if (_state == GameStatus.Complete)
            {
                throw new InvalidOperationException("Game is closed");
            }

            int i = 0;
           
            Shuffle().ForEach((card) => {
                if(_players.Any())
                {
                    var p = i % _players.Count;
                    _players[p].Hand.Add(card);
                    i++;
                }
            });

            if (_players.Any())
                _hasDealtCards = true;
        }


        public GameState AddPlayer(Player player)
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

    public class Card
    {
        public Card(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class Player
    {
        GameState? _game;
        List<Card> _hand = new();

        public Player(string userId, string name)
        {
            Name = name;
            UserId = userId;
        }

        
        public string Name { get; }
        public string UserId { get; }

        public List<Card> Hand { get => _hand; }

        public void Join(GameState game)
        {
            _game = game.AddPlayer(this);
        }

        public void LeaveGame()
        {
            if(_game == null)
            {
                return;
            }

            _game = null;
        }
    }
}
