using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace BlazorGame.Data
{
    public interface ICardProvider
    {
        List<Card> Cards();
    }

    public class CardProvider : ICardProvider
    {
        private readonly List<Card> _cards;

        public CardProvider(IConfiguration configuration)
        {
            List<Card> cards = new();
            configuration.GetSection("Cards").Bind(cards);
            _cards = cards;
        }

        public List<Card> Cards()
        {
            return _cards;
        }
    }
}
