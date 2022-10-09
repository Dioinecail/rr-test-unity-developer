using Dioinecail.ServiceLocator;
using System.Collections.Generic;

namespace Project.Cards.Hand
{
    public interface IHandManager : IService
    {
        List<CardData> Cards { get; }

        List<CardContainer> GetCardContainers();
        void AddCards(List<CardData> cards);
        void RemoveCard(int index);
    }
}