using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Project.Board
{
    public class BoardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<BoardSlot> onPointerEnter;
        public event Action<BoardSlot> onPointerExit;

        [SerializeField] private UnityEvent onPointerEnterUnityEvent;
        [SerializeField] private UnityEvent onPointerExitUnityEvent;



        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnterUnityEvent?.Invoke();
            onPointerEnter?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExitUnityEvent?.Invoke();
            onPointerExit?.Invoke(this);
        }
    }
}