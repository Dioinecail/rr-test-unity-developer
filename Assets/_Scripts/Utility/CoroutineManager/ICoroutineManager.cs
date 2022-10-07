using Dioinecail.ServiceLocator;
using System.Collections;
using UnityEngine;

namespace Project.Core
{
    public interface ICoroutineManager : IService
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
        void StopCoroutine(Coroutine coroutine);
    }
}