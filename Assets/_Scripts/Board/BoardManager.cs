using Dioinecail.ServiceLocator;
using Project.Cards;
using System;
using System.Collections.Generic;

namespace Project.Board
{
    [DefaultImplementation(typeof(IBoardManager))]
    public class BoardManager : IBoardManager
    {
        public event Action<List<CardData>> onCardsAdded;
        public event Action<List<CardData>> onCardsRemoved;
        public event Action<List<CardData>> onCardsDestroyed;

        public List<CardData> Cards => _cards;

        private List<CardData> _cards;



        public void Init() 
        {
            _cards = new List<CardData>();

            for (int i = 0; i < IBoardManager.BOARD_SIZE; i++)
            {
                _cards.Add(null);
            }
        }

        public void Clean() { }

        public void Add(CardData card, int index)
        {
            if(_cards.Count > index)
            {
                _cards[index] = card;
                onCardsAdded?.Invoke(new List<CardData>() { card });
            }
        }

        public void Remove(int index)
        {
            if (_cards.Count > index)
            {
                var card = _cards[index];

                if (card == null)
                    return;

                _cards[index] = null;
                onCardsRemoved?.Invoke(new List<CardData>() { card });
            }
        }

        public void Destroy(int index)
        {
            if (_cards.Count > index)
            {
                var card = _cards[index];

                if (card == null)
                    return;

                _cards[index] = null;
                onCardsDestroyed?.Invoke(new List<CardData>() { card });
            }
        }
    }
}