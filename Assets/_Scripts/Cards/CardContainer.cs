using Dioinecail.ServiceLocator;
using Project.Core;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Cards
{
    [Serializable]
    public class AnimatableValue
    {

        [SerializeField] private TMP_Text _targetValue;
        [SerializeField] private Animation _animationComponent;
        [SerializeField] private float _animationSegment;

        private Coroutine _coroutineAnimation;
        private ICoroutineManager _coroutineManager;



        public void Animate(int from, int to)
        {
            if (_coroutineManager == null)
                _coroutineManager = ServiceLocator.Get<ICoroutineManager>();

            if (_coroutineAnimation != null)
                _coroutineManager.StopCoroutine(_coroutineAnimation);

            _coroutineAnimation = _coroutineManager.StartCoroutine(CoroutineAnimation(_targetValue, from, to, _animationSegment));
        }

        private IEnumerator CoroutineAnimation(TMP_Text target, int from, int to, float segmentDuration)
        {
            int segments = Mathf.Abs(to - from);

            for (int i = 0; i < segments; i++)
            {
                target.text = Mathf.Lerp(from, to, (float)i / segments).ToString();
                _animationComponent.Play(PlayMode.StopAll);

                yield return new WaitForSeconds(segmentDuration);
            }

            target.text = to.ToString();
            _animationComponent.Play(PlayMode.StopAll);
        }
    }

    public class CardContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private const int CARD_SORTING = 50;

        [SerializeField] private UnityEvent _onPointerEnterEvent;
        [SerializeField] private UnityEvent _onPointerExitEvent;
        [SerializeField] private UnityEvent _onBeginDragEvent;
        [SerializeField] private UnityEvent _onEndDragEvent;

        [SerializeField] private AnimatableValue _attackValue;
        [SerializeField] private AnimatableValue _manaValue;
        [SerializeField] private AnimatableValue _healthValue;
        [SerializeField] private SpriteRenderer _heroImage;
        [SerializeField] private SpriteMask _heroMask;
        [SerializeField] private SpriteRenderer[] _cardGraphics;
        [SerializeField] private TextMeshPro[] _textGraphics;

        private CardData _currentCard;



        public void Init(CardData card)
        {
            _currentCard = card;
            _currentCard.onAttackChanged += UpdateAttack;
            _currentCard.onHealthChanged += UpdateHealth;
            _currentCard.onManaChanged += UpdateMana;
        }

        public void SetHeroImage(Sprite heroSprite)
        {
            _heroImage.sprite = heroSprite;
        }

        public void SetSorting(int index)
        {
            var initialValue = CARD_SORTING * index;

            _heroMask.frontSortingOrder = initialValue + 4;
            _heroMask.backSortingOrder = initialValue + 3;

            for (int i = 0; i < _cardGraphics.Length; i++)
            {
                _cardGraphics[i].sortingOrder = initialValue + i;
            }

            for (int i = 0; i < _textGraphics.Length; i++)
            {
                _textGraphics[i].sortingOrder = initialValue + _cardGraphics.Length + 1;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _onBeginDragEvent?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _onEndDragEvent?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData) => _onPointerEnterEvent?.Invoke();
        public void OnPointerExit(PointerEventData eventData) => _onPointerExitEvent?.Invoke();

        private void UpdateAttack(int oldValue, int newValue)
        {
            _attackValue.Animate(oldValue, newValue);
        }

        private void UpdateMana(int oldValue, int newValue)
        {
            _manaValue.Animate(oldValue, newValue);
        }

        private void UpdateHealth(int oldValue, int newValue)
        {
            _healthValue.Animate(oldValue, newValue);
        }
    }
}