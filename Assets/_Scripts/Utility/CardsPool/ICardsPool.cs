using Dioinecail.ServiceLocator;
using Project.Cards;

namespace Project.Utility.Pools
{
    public interface ICardsPool : IService
    {
        CardContainer Instantiate();
        void ReturnCard(CardContainer container);
    }
}