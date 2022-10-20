using Dioinecail.ServiceLocator;
using Project.Utility.Pools;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Project.Utility.Math;
using System;
using Project.Board;
using System.Collections;

namespace Project.Cards.Hand
{
    /// <summary>
    /// Only controls containers
    /// </summary>
    public class HandController : MonoBehaviour
    {
        public event Action<CardContainer> onCardBeginDrag;
        public event Action<CardContainer> onCardDrag;
        public event Action<CardContainer> onCardEndDrag;

        [SerializeField] private BoardArea _boardArea;
        [SerializeField] private Transform[] _handCurve;
        [SerializeField] private Transform _handOrigin;
        [SerializeField] private Transform _deckPosition;
        [SerializeField] private float _initialMoveDuration;
        [SerializeField] private float _recalculateMoveDuration;
        [SerializeField] private float _animationDelayPerCard;
        [SerializeField] private float _maxPullDelta;
        [SerializeField] private float _cardLerpSpeed;
        [SerializeField] private float _cardsSpreadZ;
        [SerializeField] private Vector3 _cardHoldOffset;

        private Transform _cardsOrigin;
        private List<CardContainer> _cardsOnHand;
        private List<Vector3> _cardOriginPositions;
        private List<Quaternion> _cardOriginRotations;
        private Camera _camera;
        private CardContainer _currentlyPointingContainer;
        private CardContainer _currenlyDraggedContainer;
        private ICardsPool _cardsPool;
        private IDeckManager _deckManager;



        private void Start()
        {
            _camera = Camera.main;

            _cardsPool = ServiceLocator.Get<ICardsPool>();
            _deckManager = ServiceLocator.Get<IDeckManager>();
            var handManager = ServiceLocator.Get<IHandManager>();

            _cardsOnHand = new List<CardContainer>();
            _cardOriginPositions = new List<Vector3>();
            _cardOriginRotations = new List<Quaternion>();

            handManager.onCardsAdded += OnCardsAdded;
            handManager.onCardsRemoved += OnCardsRemoved;
            handManager.onCardsDestroyed += OnCardsDestroyed;

            if (!_deckManager.IsReady)
            {
                _deckManager.onReady += () =>
                {
                    InitCards();
                };
            }
            else
            {
                InitCards();
            }
        }

        private void Update()
        {
            if (_currenlyDraggedContainer != null)
            {
                HandleDrag(_currenlyDraggedContainer);
                return;
            }

            if (_currentlyPointingContainer != null)
            {
                HandlePointerHover(_currentlyPointingContainer);

                return;
            }    
        }

        private void InitCards()
        {
            var handManager = ServiceLocator.Get<IHandManager>();

            handManager.GetStartingCards();
        }

        private void OnCardsAdded(List<CardData> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];

                var container = _cardsPool.Instantiate();

                container.transform.position = _deckPosition.position;
                container.transform.rotation = _deckPosition.rotation;
                container.Init(card);
                container.SetHero(_deckManager.GetHeroImage(card.Id), _deckManager.GetHeroName(card.Id), _deckManager.GetHeroDescription(card.Id));

                container.OnPointerEnterEvent += OnPointerEnterEvent;
                container.OnPointerMoveEvent += OnPointerMoveEvent;
                container.OnPointerExitEvent += OnPointerExitEvent;
                container.OnBeginDragEvent += OnBeginDragEvent;
                container.OnDragEvent += OnDragEvent;
                container.OnEndDragEvent += OnEndDragEvent;

                StartCoroutine(Delay(i * _animationDelayPerCard, () =>
                {
                    _cardsOnHand.Add(container);

                    RecalculateCardPositions(-1);
                }));
            }
        }

        private void OnCardsRemoved(List<CardData> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                var cardContainer = _cardsOnHand.FindLast((c) => c.CardData.Guid == cards[i].Guid);
                cardContainer.SetSorting(0);

                var index = _cardsOnHand.IndexOf(cardContainer);

                _cardsOnHand.RemoveAt(index);

                cardContainer.OnPointerEnterEvent -= OnPointerEnterEvent;
                cardContainer.OnPointerMoveEvent -= OnPointerMoveEvent;
                cardContainer.OnPointerExitEvent -= OnPointerExitEvent;
                cardContainer.OnBeginDragEvent -= OnBeginDragEvent;
                cardContainer.OnDragEvent -= OnDragEvent;
                cardContainer.OnEndDragEvent -= OnEndDragEvent;

                if (_currenlyDraggedContainer == cardContainer)
                    _currenlyDraggedContainer = null;

                if (_currentlyPointingContainer == cardContainer)
                    _currentlyPointingContainer = null;

                RecalculateCardPositions();
            }
        }

        private void OnCardsDestroyed(List<CardData> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                var cardContainer = _cardsOnHand.FindLast((c) => c.CardData.Guid == cards[i].Guid);
                cardContainer.SetSorting(0);

                var index = _cardsOnHand.IndexOf(cardContainer);

                _cardsOnHand.RemoveAt(index);

                cardContainer.OnPointerEnterEvent -= OnPointerEnterEvent;
                cardContainer.OnPointerMoveEvent -= OnPointerMoveEvent;
                cardContainer.OnPointerExitEvent -= OnPointerExitEvent;
                cardContainer.OnBeginDragEvent -= OnBeginDragEvent;
                cardContainer.OnDragEvent -= OnDragEvent;
                cardContainer.OnEndDragEvent -= OnEndDragEvent;
                _cardsPool.ReturnCard(cardContainer);

                if (_currenlyDraggedContainer == cardContainer)
                    _currenlyDraggedContainer = null;

                if (_currentlyPointingContainer == cardContainer)
                    _currentlyPointingContainer = null;

                RecalculateCardPositions();
            }
        }

        private void RecalculateCardPositions(float animationDuration = -1)
        {
            if (animationDuration < 0) 
                animationDuration = _recalculateMoveDuration;

            _cardOriginPositions = new List<Vector3>();
            _cardOriginRotations = new List<Quaternion>();

            for (int i = 0; i < _cardsOnHand.Count; i++)
            {
                _cardsOnHand[i].SetSorting(i + 1);
                var lerp = (float)(i + 0.5) / _cardsOnHand.Count;
                var pos = BezierUtility.Bezier(_handCurve, lerp);
                pos.z += i * _cardsSpreadZ;
                _cardOriginPositions.Add(pos);

                var direction = (pos - _handOrigin.position).normalized;
                var rot = Quaternion.LookRotation(Vector3.forward, direction);
                _cardOriginRotations.Add(rot);
            }

            for (int i = 0; i < _cardsOnHand.Count; i++)
            {
                _cardsOnHand[i].gameObject.SetActive(true);
                _cardsOnHand[i].transform
                    .DOMove(_cardOriginPositions[i], animationDuration)
                    .SetEase(Ease.OutCubic);

                _cardsOnHand[i].transform
                    .DORotate(_cardOriginRotations[i].eulerAngles, animationDuration)
                    .SetEase(Ease.OutCubic);
            }
        }

        #region HANDLE CARD DRAG EVENTS

        private void OnBeginDragEvent(CardContainer card, UnityEngine.EventSystems.PointerEventData eventData)
        {
            _currenlyDraggedContainer = card;
            onCardBeginDrag?.Invoke(_currenlyDraggedContainer);
        }

        private void OnDragEvent(CardContainer card, UnityEngine.EventSystems.PointerEventData eventData)
        {

        }

        private void HandleDrag(CardContainer currenlyDraggedContainer)
        {
            var index = _cardsOnHand.IndexOf(currenlyDraggedContainer);
            var position = GetCursorPosition(Input.mousePosition, index) + _cardHoldOffset;

            currenlyDraggedContainer.CachedTransform.position = Vector3.Lerp(currenlyDraggedContainer.CachedTransform.position, position, _cardLerpSpeed * Time.deltaTime);

            onCardDrag?.Invoke(_currenlyDraggedContainer);
        }

        private void OnEndDragEvent(CardContainer card, UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(_currenlyDraggedContainer == card)
            {
                var checkPosition = _currenlyDraggedContainer.CachedTransform.position - _cardHoldOffset;

                if (_boardArea.IsInsideBoardArea(checkPosition) 
                    && _boardArea.IsHoveringValidPosition())
                {
                    var index = _cardsOnHand.IndexOf(card);

                    _boardArea.Add(_currenlyDraggedContainer);
                    ServiceLocator.Get<IHandManager>().RemoveCard(index);
                }
                else
                {
                    var index = _cardsOnHand.IndexOf(card);
                    var pos = _cardOriginPositions[index];

                    _currenlyDraggedContainer.CachedTransform.DOMove(pos, 0.35f).SetEase(Ease.OutQuad);
                }

            }

            onCardEndDrag?.Invoke(_currenlyDraggedContainer);
            _currenlyDraggedContainer = null;
        }

        #endregion

        #region HANDLE CARD POINTER EVENTS

        private void OnPointerEnterEvent(CardContainer card, UnityEngine.EventSystems.PointerEventData eventData)
        {
            _currentlyPointingContainer = card;
        }

        private void OnPointerMoveEvent(CardContainer card, UnityEngine.EventSystems.PointerEventData eventData)
        {

        }

        private void HandlePointerHover(CardContainer currentlyPointingContainer)
        {
            var index = _cardsOnHand.IndexOf(currentlyPointingContainer);
            var position = GetCursorPosition(Input.mousePosition, index);

            var origin = _cardOriginPositions[index];
            var direction = Vector3.ClampMagnitude(position - origin, _maxPullDelta);
            var clampedPosition = origin + direction;

            currentlyPointingContainer.CachedTransform.position = Vector3.Lerp(currentlyPointingContainer.CachedTransform.position, clampedPosition, _cardLerpSpeed * Time.deltaTime);
        }

        private void OnPointerExitEvent(CardContainer card, UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (_currentlyPointingContainer == card)
            {
                if(_currenlyDraggedContainer == null)
                {
                    var index = _cardsOnHand.IndexOf(card);
                    var pos = _cardOriginPositions[index];
                    _currentlyPointingContainer.CachedTransform.DOMove(pos, 0.35f).SetEase(Ease.OutExpo);
                }

                _currentlyPointingContainer = null;
            }
        }

        #endregion

        #region UTILITY 

        private IEnumerator Delay(float duration, Action callback)
        {
            yield return new WaitForSeconds(duration);

            callback?.Invoke();
        }

        private Vector3 GetCursorPosition(Vector2 cursorPosition, int index)
        {
            var ray = _camera.ScreenPointToRay(cursorPosition);

            var angle = Vector3.Angle(Vector3.forward, ray.direction); // beta
            var cosBeta = Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)); // cos(beta);
            var a = Mathf.Abs(ray.origin.z - index * _cardsSpreadZ); // a
            var distance = (a / cosBeta);

            return ray.origin + (ray.direction * distance);
        }

        private void OnDrawGizmos()
        {
            if (_handCurve != null && _handCurve.Length == 4)
            {
                Utility.Math.BezierUtility.DrawBezier(_handCurve, 15);
            }
        }

        #endregion
    }
}