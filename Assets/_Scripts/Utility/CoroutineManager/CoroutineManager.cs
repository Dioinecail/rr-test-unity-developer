using Dioinecail.ServiceLocator;
using System.Collections;
using UnityEngine;

namespace Project.Core
{
    [DefaultImplementation(typeof(ICoroutineManager))]
    public class CoroutineManager : ICoroutineManager
    {
        private CoroutineMonobehaviour m_CoroutineObject;



        public void Init()
        {
            m_CoroutineObject = new GameObject("CoroutineMonobehaviour").AddComponent<CoroutineMonobehaviour>();

            Object.DontDestroyOnLoad(m_CoroutineObject.gameObject);
        }

        public void Clean()
        {

        }

        public Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return m_CoroutineObject.StartCoroutine(coroutine);
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            m_CoroutineObject.StopCoroutine(coroutine);
        }
    }
}