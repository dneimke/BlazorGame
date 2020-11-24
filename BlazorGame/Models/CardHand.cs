using BlazorGame.Data;
using System.Collections.Generic;

namespace BlazorGame.Models
{
    public record CardHand(string UserId, string Name)
    {
        public List<Card> Cards { get; init;} = new();
    }
}
