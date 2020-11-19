using System;

namespace BlazorGame.Models
{
    public enum GameRole { Creator, Player };
    public class CurrentSessionModel
    {
        public Guid GameSessionId { get; set; }
        public string UserId { get; set; } = "";
        public string Username { get; set; } = "";
        public int PINCode { get; set; }
        public GameRole Role { get; set; }
    }
}
