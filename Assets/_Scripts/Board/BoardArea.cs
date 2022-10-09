using Dioinecail.ServiceLocator;
using Project.Cards;
using Project.Cards.Hand;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Project.Utility.Pools;

namespace Project.Board
{
    public class BoardArea : MonoBehaviour
    {
        [SerializeField] private HandController _handController;
        [SerializeField] private List<BoardSlot> _boardSlots;

        [SerializeField] private Vector2 boardBottomLeft;
        [SerializeField] private Vector2 boardTopRight;

        private List<CardContainer> _cardsOnBoard;
        private BoardSlot _currentlyHoveredSlot;
        private IBoardManager _boardManager;
        private ICardsPool _cardsPool;



        public bool IsInsideBoardArea(Vector3 position)
        {
            var x = position.x > boardBottomLeft.x && position.x < boardTopRight.x;
            var y = position.y > boardBottomLeft.y && position.y < boardTopRight.y;

            return x && y;
        }

        public bool IsHoveringValidPosition()
        {
            return _currentlyHoveredSlot != null;
        }

        public void Add(CardContainer card)
        {
            var index = GetHoveredSlotIndex();

            if(index > -1)
            {
                _cardsOnBoard.Add(card);

                var pos = _boardSlots[index].transform.position;
                var rot = _boardSlots[index].transform.rotation;

                card.CachedTransform.DOMove(pos, 0.25f).SetEase(Ease.OutExpo);
                card.CachedTransform.DORotate(rot.eulerAngles, 0.25f).SetEase(Ease.OutExpo);

                ServiceLocator.Get<IBoardManager>().Add(card.CardData, index);
            }
        }

        public void Remove(CardContainer card)
        {
            var index = _cardsOnBoard.IndexOf(card);
            _boardManager.Remove(index);
        }

        private void Start()
        {
            _boardManager = ServiceLocator.Get<IBoardManager>();
            _cardsOnBoard = new List<CardContainer>();
            _cardsPool = ServiceLocator.Get<ICardsPool>();

            _boardManager.onCardsAdded += OnCardsAdded;
            _boardManager.onCardsRemoved += OnCardsRemoved;
            _boardManager.onCardsDestroyed += OnCardsDestroyed;

            foreach (var slot in _boardSlots)
            {
                slot.onPointerEnter += OnPointerEnter;
                slot.onPointerExit += OnPointerExit;
            }
        }

        private void OnCardsAdded(List<CardData> cards)
        {

        }

        private void OnCardsRemoved(List<CardData> cards)
        {
            foreach (var card in cards)
            {
                var index = _cardsOnBoard.FindLastIndex((c) => c.CardData.Guid == card.Guid);
                var cardContainer = _cardsOnBoard[index];
                _cardsOnBoard.RemoveAt(index);
            }
        }

        private void OnCardsDestroyed(List<CardData> cards)
        {
            foreach (var card in cards)
            {
                var index = _cardsOnBoard.FindLastIndex((c) => c.CardData.Guid == card.Guid);
                var cardContainer = _cardsOnBoard[index];
                _cardsPool.ReturnCard(cardContainer);
                _cardsOnBoard.RemoveAt(index);
            }
        }

        private void OnPointerEnter(BoardSlot slot)
        {
            _currentlyHoveredSlot = slot;
        }

        private void OnPointerExit(BoardSlot slot)
        {
            if(_currentlyHoveredSlot == slot)
            {
                _currentlyHoveredSlot = null;
            }    
        }

        private int GetHoveredSlotIndex()
        {
            if(_currentlyHoveredSlot != null)
            {
                return _boardSlots.IndexOf(_currentlyHoveredSlot);
            }
            else
            {
                return -1;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(boardBottomLeft, boardBottomLeft + Vector2.up);
            Gizmos.DrawLine(boardBottomLeft, boardBottomLeft + Vector2.right);
            Gizmos.DrawLine(boardTopRight, boardTopRight + Vector2.down);
            Gizmos.DrawLine(boardTopRight, boardTopRight + Vector2.left);
        }
    }
}