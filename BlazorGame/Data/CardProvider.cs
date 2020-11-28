using System;
using System.Collections.Generic;

namespace BlazorGame.Data
{
    public interface ICardProvider
    {
        List<Card> Cards();
    }

    public class CardProvider : ICardProvider
    {
        List<string> _animals = new() { "Monkey", "Panda", "Spider", "Tiger" };
        List<string> _colors = new() { "primary", "secondary", "danger", "warning" };
        List<string> _suits = new() { "monkey", "panda", "spider", "tiger" };

        public List<Card> Cards()
        {
            return _cards;
        }

        List<Card> _cards
        {
            get {
                var list = new List<Card>();
                for (var i = 0; i < _animals.Count; i++)
                {
                    list.Add(new(_animals[i], _colors[i], _suits[i]));
                    list.Add(new(_animals[i], _colors[i], _suits[i]));
                }
                return list;
            }
        }
    }
}
