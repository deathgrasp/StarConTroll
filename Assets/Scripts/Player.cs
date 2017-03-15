using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player {

        // TODO: turn from int to ENUM
        //public const int NO_ONE = 0; // used during turn-resolve phase
        //public const int PLAYER_1 = 1;
        //public const int PLAYER_2 = 2;

        public bool IsHuman;
        public int Number;
        public Player(int number, Boolean isHuman=true)
        {
            IsHuman = isHuman;
            Number = number;
        }
    }
}
