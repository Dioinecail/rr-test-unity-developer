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
        public TMP_Text TargetValue;

        [SerializeField] private Animation _animationComponent;
        [SerializeField] private float _animationSegment;



        public void Animate(int to)
        {
            TargetValue.text = to.ToString();
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
        [SerializeField] private TMP_Text _heroName;
        [SerializeField] private TMP_Text _heroDescription;
        [SerializeField] private SpriteRenderer _heroImage;
        [SerializeField] private SpriteMask _heroMask;
        [SerializeField] private SpriteRenderer[] _cardGraphics;
        [SerializeField] private TextMeshPro[] _textGraphics;

        private CardData _currentCard;



        public void Init(CardData card)
        {
            _currentCard = card;
            _attackValue.TargetValue.text = _currentCard.CurrentAttack.ToString();
            _healthValue.TargetValue.text = _currentCard.CurrentHealth.ToString();
            _manaValue.TargetValue.text = _currentCard.CurrentMana.ToString();

            // in case it's a card from pool
            _currentCard.onAttackChanged -= UpdateAttack;
            _currentCard.onHealthChanged-= UpdateHealth;
            _currentCard.onManaChanged -= UpdateMana;

            _currentCard.onAttackChanged += UpdateAttack;
            _currentCard.onHealthChanged += UpdateHealth;
            _currentCard.onManaChanged += UpdateMana;
        }

        public void SetHero(Sprite heroSprite, string name, string description)
        {
            _heroImage.sprite = heroSprite;
            _heroName.text = name;
            _heroDescription.text = description;
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
            _attackValue.Animate(newValue);
        }

        private void UpdateMana(int oldValue, int newValue)
        {
            _manaValue.Animate(newValue);
        }

        private void UpdateHealth(int oldValue, int newValue)
        {
            _healthValue.Animate(newValue);
        }
    }
}