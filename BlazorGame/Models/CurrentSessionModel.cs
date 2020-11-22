using BlazorGame.Data;
using System;
using System.Collections.Generic;

namespace BlazorGame.Models
{
    public enum GameRole { Creator = 1, Player = 2 };
    
    public record PlayerSessionModel
    {
        public Guid GameSessionId { get; init; }
        public string UserId { get; init; } = "";
        public string Username { get; init; } = "";
        public int PinCode { get; init; }
        public GameRole Role { get; init; }
    }

    public record GameCreatedModel : PlayerSessionModel;
    public record PlayerJoinedModel : PlayerSessionModel;
    public record DealtCardsModel(Guid GameSessionId, bool CanDealCards, List<CardHand> Hands);
}
