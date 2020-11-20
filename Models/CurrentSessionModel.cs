using BlazorGame.Data;
using System;
using System.Collections.Generic;

namespace BlazorGame.Models
{
    public enum GameRole { Creator = 1, Player = 2 };
    
    public class PlayerSessionModel
    {
        public Guid GameSessionId { get; set; }
        public string UserId { get; set; } = "";
        public string Username { get; set; } = "";
        public int PinCode { get; set; }
        public GameRole Role { get; set; }
    }

    public class GameCreatedModel : PlayerSessionModel
    {
       
    }

    public class PlayerJoinedModel : PlayerSessionModel
    {

    }

    public class DealtCardsModel
    {
        public Guid GameSessionId { get; set; }
        public bool CanDealCards { get; set; }
        public List<CardHand> Hands { get; set; } = new();
    }
}
