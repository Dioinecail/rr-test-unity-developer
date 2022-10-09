using Dioinecail.ServiceLocator;
using Project.Utility.Pools;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Cards.Hand
{
    /// <summary>
    /// Only handles HAND card data
    /// </summary>
    [DefaultImplementation(typeof(IHandManager))]
    public class HandManager : IHandManager
    {
        public event System.Action<List<CardData>> onCardsAdded;
        public event System.Action<List<CardData>> onCardsRemoved;
        public event System.Action<List<CardData>> onCardsDestroyed;

        private const int HAND_SIZE_MIN = 4;
        private const int HAND_SIZE_MAX = 7;

        public List<CardData> Cards => _cards;

        private List<CardData> _cards;
        private IDeckManager _deckManager;



        public void AddCards(List<CardData> cards)
        {
            _cards.AddRange(cards);

            onCardsAdded?.Invoke(cards);
        }

        public void RemoveCard(int index)
        {
            var card = _cards[index];

            _cards.RemoveAt(index);

            onCardsRemoved?.Invoke(new List<CardData>() { card });
        }

        public void DestroyCard(int index)
        {
            var card = _cards[index];

            _cards.RemoveAt(index);

            onCardsDestroyed?.Invoke(new List<CardData>() { card });
        }

        public void AddCardsFromDeck(int amount)
        {
            List<CardData> addedCards = new List<CardData>();

            for (int i = 0; i < amount; i++)
            {
                var cardInfo = _deckManager.GetCard();
                var cardData = new CardData(cardInfo.Id, cardInfo.Health, cardInfo.Mana, cardInfo.Attack);

                _cards.Add(cardData);
                addedCards.Add(cardData);
            }

            onCardsAdded?.Invoke(addedCards);
        }

        public void GetStartingCards()
        {
            var handSize = Random.Range(HAND_SIZE_MIN, HAND_SIZE_MAX);
            AddCardsFromDeck(handSize);
        }

        public void Init()
        {
            _deckManager = ServiceLocator.Get<IDeckManager>();
            _cards = new List<CardData>();
        }

        public void Clean()
        {

        }
    }
}