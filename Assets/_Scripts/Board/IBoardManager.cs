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
    public class IBoardManager : IService
    {
        private Player[] _players;



        public void Init()
        {
            _players = CreatePlayers();
        }

        public void Clean()
        {

        }

        private Player[] CreatePlayers()
        {
            return null;
        }
    }
}