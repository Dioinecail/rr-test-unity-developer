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
        private const int HAND_SIZE_MIN = 4;
        private const int HAND_SIZE_MAX = 7;
        private Quaternion INITIAL_ROTATION = Quaternion.Euler(0, 90, 0);

        public List<CardData> Cards => _cards;

        private List<CardData> _cards;
        private IDeckManager _deckManager;



        public void AddCards(List<CardData> cards)
        {
            _cards.AddRange(cards);
        }

        public void RemoveCard(int index)
        {
            _cards.RemoveAt(index);
        }

        public List<CardContainer> GetCardContainers()
        {
            var pool = ServiceLocator.Get<ICardsPool>();
            var cards = new List<CardContainer>();

            for (int i = 0; i < _cards.Count; i++)
            {
                var c = _cards[i];
                var container = pool.Instantiate();
                container.transform.rotation = INITIAL_ROTATION;
                container.Init(c);
                container.SetSorting(i);
                container.SetHero(_deckManager.GetHeroImage(c.Id), _deckManager.GetHeroName(c.Id), _deckManager.GetHeroDescription(c.Id));
                cards.Add(container);
            }

            return cards;
        }

        private void GetStartingCards()
        {
            var handSize = Random.Range(HAND_SIZE_MIN, HAND_SIZE_MAX);

            for (int i = 0; i < handSize; i++)
            {
                var cardInfo = _deckManager.GetCard();
                var cardData = new CardData(cardInfo.Id, cardInfo.Health, cardInfo.Mana, cardInfo.Attack);

                _cards.Add(cardData);
            }
        }

        public void Init()
        {
            _deckManager = ServiceLocator.Get<IDeckManager>();

            _cards = new List<CardData>();

            GetStartingCards();
        }

        public void Clean()
        {

        }
    }
}