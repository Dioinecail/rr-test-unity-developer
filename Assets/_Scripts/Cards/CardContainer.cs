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

    public class CardContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action<CardContainer, PointerEventData> OnPointerEnterEvent;
        public event Action<CardContainer, PointerEventData> OnPointerMoveEvent;
        public event Action<CardContainer, PointerEventData> OnPointerExitEvent;
        public event Action<CardContainer, PointerEventData> OnBeginDragEvent;
        public event Action<CardContainer, PointerEventData> OnDragEvent;
        public event Action<CardContainer, PointerEventData> OnEndDragEvent;

        private const int CARD_SORTING = 50;

        [SerializeField] private UnityEvent _onPointerEnterUnityEvent;
        [SerializeField] private UnityEvent _onPointerExitUnityEvent;
        [SerializeField] private UnityEvent _onBeginDragUnityEvent;
        [SerializeField] private UnityEvent _onEndDragUnityEvent;

        [SerializeField] private AnimatableValue _attackValue;
        [SerializeField] private AnimatableValue _manaValue;
        [SerializeField] private AnimatableValue _healthValue;
        [SerializeField] private TMP_Text _heroName;
        [SerializeField] private TMP_Text _heroDescription;
        [SerializeField] private SpriteRenderer _heroImage;
        [SerializeField] private SpriteMask _heroMask;
        [SerializeField] private SpriteRenderer[] _cardGraphics;
        [SerializeField] private TextMeshPro[] _textGraphics;

        public Transform CachedTransform { get; private set; }



        public void Init(CardData card)
        {
            CachedTransform = transform;
            _attackValue.TargetValue.text = card.CurrentAttack.ToString();
            _healthValue.TargetValue.text = card.CurrentHealth.ToString();
            _manaValue.TargetValue.text = card.CurrentMana.ToString();

            // in case it's a card from pool
            card.onAttackChanged -= UpdateAttack;
            card.onHealthChanged-= UpdateHealth;
            card.onManaChanged -= UpdateMana;

            card.onAttackChanged += UpdateAttack;
            card.onHealthChanged += UpdateHealth;
            card.onManaChanged += UpdateMana;
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
            _onBeginDragUnityEvent?.Invoke();
            OnBeginDragEvent?.Invoke(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragEvent?.Invoke(this, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _onEndDragUnityEvent?.Invoke();
            OnEndDragEvent?.Invoke(this, eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _onPointerEnterUnityEvent?.Invoke();
            OnPointerEnterEvent?.Invoke(this, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _onPointerExitUnityEvent?.Invoke();
            OnPointerExitEvent?.Invoke(this, eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            OnPointerMoveEvent?.Invoke(this, eventData);
        }

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