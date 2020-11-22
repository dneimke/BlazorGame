using BlazorGame.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGame.Models
{
    public class CardHand
    {
        public string UserId { get; set; }
        public List<Card> Cards { get; set; }
    }
}
