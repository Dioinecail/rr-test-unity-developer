using Dioinecail.ServiceLocator;

namespace Project.Cards
{
    public interface IDeckManager : IService
    {
        int RemainingCards { get; }

        void AddCards(int id, int amount);
        void AddCard(int id);
    }
}