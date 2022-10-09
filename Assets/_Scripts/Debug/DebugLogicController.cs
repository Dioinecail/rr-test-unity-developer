using Dioinecail.ServiceLocator;
using Project.Board;
using Project.Cards.Hand;
using System.Collections;
using UnityEngine;

public class DebugLogicController : MonoBehaviour
{
    private const int RANDOM_VALUE_MIN = -2;
    private const int RANDOM_VALUE_MAX = 10;

    [SerializeField] private float _stuffDelay = 1;

    private IHandManager _handManager;
    private IBoardManager _boardManager;
    private Coroutine _coroutineStuff;



    public void AddRandomStuff()
    {
        if (_coroutineStuff != null)
            return;

        _coroutineStuff = StartCoroutine(CoroutineStuff());
    }

    public void AddCardFromDeck()
    {
        _handManager.AddCardsFromDeck(1);
    }

    public void RemoveRandomCard()
    {
        var index = Random.Range(0, _handManager.Cards.Count);
        _handManager.DestroyCard(index);
    }

    public void ClearBoard()
    {
        var cards = _boardManager.Cards;
        var count = cards.Count - 1;

        for (int i = count; i >= 0; i--)
        {
            _boardManager.Destroy(i);
        }
    }

    private void Start()
    {
        _handManager = ServiceLocator.Get<IHandManager>();
        _boardManager = ServiceLocator.Get<IBoardManager>();
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
