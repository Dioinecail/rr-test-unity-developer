using Dioinecail.ServiceLocator;
using Newtonsoft.Json;
using Project.FileSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Cards
{
    [System.Serializable]
    public class CardInfo
    {
        public int Id;
        public int Health;
        public int Mana;
        public int Attack;
        public string Name;
        public string Desc;
    }

    public static class DeckGenerator
    {
        private static string _cardsPath = Application.dataPath + "/Cards/Cards.json";



        public static Dictionary<int, CardInfo> GenerateCards()
        {
            var cards = new Dictionary<int, CardInfo>();

            var fileManager = ServiceLocator.Get<IFileManager>();
            var cardsDataJson = fileManager.ReadAllText(_cardsPath);
            var cardsData = JsonConvert.DeserializeObject<List<CardInfo>>(cardsDataJson);

            foreach (var card in cardsData)
            {
                cards.Add(card.Id, card);
            }

            return cards;
        }
    }
}