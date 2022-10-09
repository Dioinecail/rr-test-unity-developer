using Dioinecail.ServiceLocator;
using Project.Cards.Hand;
using System.Collections;
using UnityEngine;

public class DebugLogicController : MonoBehaviour
{
    private const int RANDOM_VALUE_MIN = -2;
    private const int RANDOM_VALUE_MAX = 10;

    [SerializeField] private float _stuffDelay = 1;

    private IHandManager _handManager;
    private Coroutine _coroutineStuff;



    public void AddRandomStuff()
    {
        if (_coroutineStuff != null)
            return;

        _coroutineStuff = StartCoroutine(CoroutineStuff());
    }

    private void Start()
    {
        _handManager = ServiceLocator.Get<IHandManager>();
    }

    private IEnumerator CoroutineStuff()
    {
        foreach (var card in _handManager.Cards)
        {
            var randomAttackValue = Random.Range(RANDOM_VALUE_MIN, RANDOM_VALUE_MAX);
            var randomHealthValue = Random.Range(RANDOM_VALUE_MIN, RANDOM_VALUE_MAX);
            var randomManaValue = Random.Range(RANDOM_VALUE_MIN, RANDOM_VALUE_MAX);

            card.ChangeAttack(randomAttackValue);
            card.ChangeHealth(randomHealthValue);
            card.ChangeMana(randomManaValue);

            yield return new WaitForSeconds(_stuffDelay);
        }

        _coroutineStuff = null;
    }
}
