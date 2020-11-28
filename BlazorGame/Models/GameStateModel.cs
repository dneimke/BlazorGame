using BlazorGame.Data;
using System;
using System.Collections.Generic;

namespace BlazorGame.Models
{
    public enum GameRole { Creator = 1, Player = 2 };
    
    public record GameStateModel
    {
        public GameStateModel(Game game)
        {

            GameSessionId = game!.Id;
            HasDealtCards = game.HasDealtCards;
            IsComplete = game.IsComplete;
            ActivePlayerId = game.ActivePlayerId;
            GameCreatorId = game.GameCreatorId;
            GameCreatorName = game.GameCreatorName;
            PinCode = game.PinCode;
            Hands = game.Hands;
        }

        public PlayedCard? UpCard { get; init; }
        public PlayedCard? MatchingCard { get; init; }

        public bool CanPlayNextCard => UpCard?.Card is not null && MatchingCard?.Card is not null;

        public Guid GameSessionId { get; }
        public bool HasDealtCards { get; }
        public bool IsComplete { get; }
        public string ActivePlayerId { get; }
        public string GameCreatorId { get; }
        public string GameCreatorName { get; }
        public int PinCode { get; }
        public List<CardHand> Hands { get; }
    }

    public record PlayedCard(Card Card, string Player);
}
