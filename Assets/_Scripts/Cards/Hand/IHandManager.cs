using Dioinecail.ServiceLocator;
using System;
using System.Collections.Generic;

namespace Project.Cards.Hand
{
    public interface IHandManager : IService
    {
        event Action<List<CardData>> onCardsAdded;
        event Action<List<CardData>> onCardsRemoved;
        event Action<List<CardData>> onCardsDestroyed;

        List<CardData> Cards { get; }

        void GetStartingCards();
        void AddCards(List<CardData> cards);
        void RemoveCard(int index);
        void DestroyCard(int index);
        void AddCardsFromDeck(int amount);
    }
}