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

        private Transform _cardsOrigin;
        private List<CardContainer> _cardsOnHand;
        private List<CardContainer> _cardsOnTable;
        private List<Vector3> _cardOriginPositions;
        private List<Quaternion> _cardOriginRotations;



        private void Start()
        {
            var handManager = ServiceLocator.Get<IHandManager>();
            var deckManager = ServiceLocator.Get<IDeckManager>();


            if (!deckManager.IsReady)
            {
                deckManager.onReady += () =>
                {
                    _cardsOnHand = handManager.GetCardContainers();
                    PositionCards();
                };
            }
            else
            {
                _cardsOnHand = handManager.GetCardContainers();
                PositionCards();
            }
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

        private void OnDrawGizmos()
        {
            if(_handCurve != null && _handCurve.Length == 4)
            {
                Utility.Math.BezierUtility.DrawBezier(_handCurve, 15);
            }
        }
    }
}