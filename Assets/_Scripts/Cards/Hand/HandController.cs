using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Cards.Hand
{
    public class HandController : MonoBehaviour
    {
        [SerializeField] private Transform[] _handCurve;
        [SerializeField] private Transform _handOrigin;

        private Transform _cardsOrigin;
        private List<CardContainer> _cards;



        private void OnDrawGizmos()
        {
            if(_handCurve != null && _handCurve.Length == 4)
            {
                Utility.Math.BezierUtility.DrawBezier(_handCurve, 15);
            }
        }
    }
}