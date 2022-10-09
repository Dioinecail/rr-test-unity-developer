using Dioinecail.ServiceLocator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Board
{

    /// <summary>
    /// Basically game manager
    /// </summary>
    public interface IBoardManager : IService
    {
        event Action<int> onTurnStarted;
        event Action<int> onTurnEnded;

        void HandleTurnStarted();
        void HandleTurnEnded();
    }
}