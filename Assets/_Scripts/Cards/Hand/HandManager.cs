using Dioinecail.ServiceLocator;
using System.Collections.Generic;

namespace Project.Cards.Hand
{
    [DefaultImplementation(typeof(IHandManager))]
    public class HandManager : IHandManager
    {
        public List<CardData> Cards => _cards;
        private List<CardData>_cards;



        public void AddCards(List<CardData> cards)
        {
            _cards.AddRange(cards);
        }

        public void RemoveCard(int index)
        {
            _cards.RemoveAt(index);
        }

        public void Init()
        {
            _cards = new List<CardData>();
        }

        public void Clean()
        {

        }
    }
}