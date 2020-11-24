using System;
using System.Collections.Generic;

namespace BlazorGame.Models
{
    public enum GameRole { Creator = 1, Player = 2 };
    
    public record GameStateModel(Guid GameSessionId, bool HasDealtCards, string? CurrentPlayerId, int PinCode, List<CardHand> Hands);
}
