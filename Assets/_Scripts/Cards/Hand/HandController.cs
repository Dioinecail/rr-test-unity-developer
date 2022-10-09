using Dioinecail.ServiceLocator;
using Project.Utility.Pools;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Project.Utility.Math;

namespace Project.Cards.Hand
{
    /// <summary>
    /// Only controls containers
    /// </summary>
    public class HandController : MonoBehaviour
    {
        [SerializeField] private Transform[] _handCurve;
        [SerializeField] private Transform _handOrigin;
        [SerializeField] private Transform _deckPosition;
        [SerializeField] private float _initialMoveDuration;
        [SerializeField] private float _animationDelayPerCard;
        [SerializeField] private float _maxPullDelta;

        private Transform _cardsOrigin;
        private List<CardContainer> _cardsOnHand;
        private List<CardContainer> _cardsOnTable;
        private List<Vector3> _cardOriginPositions;
        private List<Quaternion> _cardOriginRotations;
        private Camera _camera;
        private CardContainer _currentlyPointingContainer;



        private void Start()
        {
            _camera = Camera.main;

            var deckManager = ServiceLocator.Get<IDeckManager>();

            if (!deckManager.IsReady)
            {
                deckManager.onReady += () =>
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
            if (_currentlyPointingContainer == null)
                return;

            var card = _currentlyPointingContainer;

            var position = GetPosition(Input.mousePosition);

            var index = _cardsOnHand.IndexOf(card);
            var origin = _cardOriginPositions[index];
            var direction = Vector3.ClampMagnitude(position - origin, _maxPullDelta);
            var clampedPosition = origin + direction;

            card.CachedTransform.position = Vector3.Lerp(card.CachedTransform.position, clampedPosition, 0.05f);
        }

        private void InitCards()
        {
            var handManager = ServiceLocator.Get<IHandManager>();

            _cardsOnHand = handManager.GetCardContainers();

            foreach (var card in _cardsOnHand)
            {
                card.OnPointerEnterEvent += OnPointerEnterEvent;
                card.OnPointerMoveEvent += OnPointerMoveEvent;
                card.OnPointerExitEvent += OnPointerExitEvent;
                card.OnBeginDragEvent += OnBeginDragEvent;
                card.OnDragEvent += OnDragEvent;
                card.OnEndDragEvent += OnEndDragEvent;
            }

            PositionCards();
        }

        [ContextMenu("Reposition Cards")]
        private void PositionCards()
        {
            _cardOriginPositions = new List<Vector3>();
            _cardOriginRotations = new List<Quaternion>();

            for (int i = 0; i < _cardsOnHand.Count; i++)
            {
                _cardsOnHand[i].transform.position = _deckPosition.position;
                _cardsOnHand[i].transform.rotation = _deckPosition.rotation;
                var lerp = (float)(i + 0.5) / _cardsOnHand.Count;
                var pos = BezierUtility.Bezier(_handCurve, lerp);
                _cardOriginPositions.Add(pos);

                var direction = (pos - _handOrigin.position).normalized;
                var rot = Quaternion.LookRotation(Vector3.forward, direction);
                _cardOriginRotations.Add(rot);
            }

            for (int i = 0; i < _cardsOnHand.Count; i++)
            {
                _cardsOnHand[i].gameObject.SetActive(true);
                _cardsOnHand[i].transform
                    .DOMove(_cardOriginPositions[i], _initialMoveDuration)
                    .SetEase(Ease.InOutQuad)
                    .SetDelay(i * _animationDelayPerCard);

                _cardsOnHand[i].transform
                    .DORotate(_cardOriginRotations[i].eulerAngles, _initialMoveDuration)
                    .SetEase(Ease.InOutQuad)
                    .SetDelay(i * _animationDelayPerCard);
            }
        }

        #region HANDLE CARD EVENTS

        private void OnBeginDragEvent(CardContainer card, UnityEngine.EventSystems.PointerEventData eventData)
        {

        }

        private void OnDragEvent(CardContainer card, UnityEngine.EventSystems.PointerEventData eventData)
        {

        }

        private void OnEndDragEvent(CardContainer card, UnityEngine.EventSystems.PointerEventData eventData)
        {

        }

        private void OnPointerEnterEvent(CardContainer card, UnityEngine.EventSystems.PointerEventData eventData)
        {

        }

        private void OnPointerMoveEvent(CardContainer card, UnityEngine.EventSystems.PointerEventData eventData)
        {
            _currentlyPointingContainer = card;
        }

        private void OnPointerExitEvent(CardContainer card, UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (_currentlyPointingContainer == card)
            {
                var index = _cardsOnHand.IndexOf(card);
                var pos = _cardOriginPositions[index];
                _currentlyPointingContainer.CachedTransform.DOMove(pos, 0.35f).SetEase(Ease.OutQuad);
                _currentlyPointingContainer = null;
            }
        }

        #endregion

        private Vector3 GetPosition(Vector2 cursorPosition)
        {
            var ray = _camera.ScreenPointToRay(cursorPosition);

            var angle = Vector3.Angle(Vector3.forward, ray.direction); // beta
            var cosBeta = Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)); // cos(beta);
            var a = Mathf.Abs(_camera.transform.position.z); // a
            var distance = a / cosBeta;

            return ray.origin + (ray.direction * distance);
        }

        private void OnDrawGizmos()
        {
            if (_handCurve != null && _handCurve.Length == 4)
            {
                Utility.Math.BezierUtility.DrawBezier(_handCurve, 15);
            }
        }
    }
}