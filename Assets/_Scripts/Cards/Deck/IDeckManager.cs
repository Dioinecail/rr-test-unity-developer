using Dioinecail.ServiceLocator;
using System;
using UnityEngine;

namespace Project.Cards
{
    public interface IDeckManager : IService
    {
        event Action onReady;

        int RemainingCards { get; }
        bool IsReady { get; }

        void AddCards(int id, int amount);
        void AddCard(int id);
        CardInfo GetCard();
        Sprite GetHeroImage(int id);
        string GetHeroName(int id);
        string GetHeroDescription(int id);
    }
}