using Dioinecail.ServiceLocator;
using Project.Cards;
using System;
using System.Collections.Generic;

namespace Project.Board
{

    /// <summary>
    /// Basically game manager
    /// </summary>
    public interface IBoardManager : IService
    {
        const int BOARD_SIZE = 7;

        event Action<List<CardData>> onCardsAdded;
        event Action<List<CardData>> onCardsRemoved;
        event Action<List<CardData>> onCardsDestroyed;

        List<CardData> Cards { get; }

        void Add(CardData card, int index);
        void Remove(int index);
        void Destroy(int index);
    }
}