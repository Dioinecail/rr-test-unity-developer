using System;

namespace Project.Board
{
    public class BoardManager : IBoardManager
    {
        public event Action<int> onTurnStarted;
        public event Action<int> onTurnEnded;



        public void Init() { }
        public void Clean() { }

        public void HandleTurnEnded()
        {
            // some logic for future gameplay here
        }

        public void HandleTurnStarted()
        {
            // some logic for future gameplay here
        }
    }
}