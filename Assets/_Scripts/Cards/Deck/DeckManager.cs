using Dioinecail.ServiceLocator;
using Project.Utility.Resources;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Cards
{
    [DefaultImplementation(typeof(IDeckManager))]
    public class DeckManager : IDeckManager
    {
        public event System.Action onReady;

        private const int DECK_SIZE = 50;

        public int RemainingCards => _remainingCards.Count;
        public bool IsReady => _isReady;

        private bool _isReady;
        private Queue<int> _remainingCards;
        private Dictionary<int, CardInfo> _allCards;
        private Dictionary<int, Sprite> _heroSprites;



        public void InitDeps()
        {
            // no deps
        }

        public void Start()
        {
            _remainingCards = new Queue<int>();
            _heroSprites = new Dictionary<int, Sprite>();

            _allCards = DeckGenerator.GenerateCards();

            var imageLoader = ServiceLocator.Get<IImageLoader>();
            var ids = _allCards.Keys;

            for (int i = 0; i < DECK_SIZE; i++)
            {
                var randomIndex = Random.Range(0, ids.Count);
                _remainingCards.Enqueue(randomIndex);
            }

            imageLoader.GetImages(_allCards.Count, null, (images) =>
            {
                foreach (var card in _allCards)
                {
                    _heroSprites.Add(card.Key, images[card.Key]);
                }

                _isReady = true;
                onReady?.Invoke();
            });
        }

        public void Clean()
        {

        }

        public void AddCards(int id, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                AddCard(id);
            }
        }

        public void AddCard(int id)
        {
            // possibly want to allow card inserting logic here (ex. insert last/first/middle/random)
            _remainingCards.Enqueue(id);
        }

        public CardInfo GetCard()
        {
            var cardId = _remainingCards.Dequeue();

            return _allCards[cardId];
        }

        public Sprite GetHeroImage(int id)
        {
            return _heroSprites[id];
        }

        public string GetHeroName(int id)
        {
            return _allCards[id].Name;
        }

        public string GetHeroDescription(int id)
        {
            return _allCards[id].Desc;
        }
    }
}
