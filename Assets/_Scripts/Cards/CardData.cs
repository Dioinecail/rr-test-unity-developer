using System;

namespace Project.Cards
{
    public class CardData
    {
        public event Action<int, int> onHealthChanged;
        public event Action<int, int> onAttackChanged;
        public event Action<int, int> onManaChanged;

        public event Action<int> onCardCreated;
        public event Action<int> onCardDestroyed;

        public int Id { get; }
        public int CurrentHealth => _currentHealth;
        public int CurrentMana => _currentMana;
        public int CurrentAttack => _currentAttack;

        private int _currentHealth;
        private int _currentMana;
        private int _currentAttack;



        public void ChangeHealth(int amount)
        {
            var oldValue = _currentHealth;
            _currentHealth += amount;

            if(oldValue != _currentHealth)
            {
                onHealthChanged?.Invoke(oldValue, _currentHealth);
            }
        }

        public void ChangeMana(int amount)
        {
            var oldValue = _currentMana;
            _currentMana += amount;

            if (oldValue != _currentMana)
            {
                onManaChanged?.Invoke(oldValue, _currentMana);
            }
        }

        public void ChangeAttack(int amount)
        {
            var oldValue = _currentAttack;
            _currentAttack += amount;

            if (oldValue != _currentAttack)
            {
                onAttackChanged?.Invoke(oldValue, _currentAttack);
            }
        }
    }
}
