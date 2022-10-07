using Dioinecail.ServiceLocator;
using System.Collections.Generic;

namespace Project.Cards
{
    [DefaultImplementation(typeof(IDeckManager))]
    public class DeckManager : IDeckManager
    {
        public int RemainingCards => _remainingCards.Count;

        private Queue<int> _remainingCards;
        private Dictionary<int, CardInfo> _allCards;



        public void Init()
        {
            _allCards = DeckGenerator.GenerateCards();
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
    }
}
