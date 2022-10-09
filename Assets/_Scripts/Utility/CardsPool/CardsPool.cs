using Dioinecail.ServiceLocator;
using Project.Cards;
using Project.FileSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utility.Pools
{
    [DefaultImplementation(typeof(ICardsPool))]
    public class CardsPool : ICardsPool
    {
        private static string PREFAB_PATH = "Prefabs/Prefab_CardContainer";
        private const int PREWARM_INSTANTIATE = 10;

        private Stack<CardContainer> _pool;
        private GameObject _prefab;



        public CardContainer Instantiate()
        {
            CardContainer card;

            if(_pool.Count > 0)
            {
                card = _pool.Pop();
            }
            else
            {
                card = Object.Instantiate(_prefab, Vector3.zero, Quaternion.identity).GetComponent<CardContainer>();
            }

            return card;
        }

        public void ReturnCard(CardContainer container)
        {
            _pool.Push(container);
            container.gameObject.SetActive(false);
        }

        public void Init()
        {
            _pool = new Stack<CardContainer>();
            _prefab = ServiceLocator.Get<IFileManager>().LoadResource<GameObject>(PREFAB_PATH);

            Prewarm();
        }

        public void Clean()
        {

        }

        private void Prewarm()
        {
            var prewarmPool = new Stack<CardContainer>();

            for (int i = 0; i < PREWARM_INSTANTIATE; i++)
            {
                var card = Instantiate();
                prewarmPool.Push(card);
            }

            _pool = prewarmPool;
        }
    }
}