using BlazorGame.Data;
using System;
using System.Collections.Generic;

namespace BlazorGame.Models
{
    public enum GameRole { Creator = 1, Player = 2 };
    
    public record GameStateModel(Guid GameSessionId, bool HasDealtCards, string? ActivePlayerId, int PinCode, List<CardHand> Hands)
    {
        public PlayedCard? UpCard { get; init; }
        public PlayedCard? MatchingCard { get; init; }

        public bool CanPlayNextCard => UpCard?.Card is not null && MatchingCard?.Card is not null;
    }

    public record PlayedCard(Card Card, string Player);
}
