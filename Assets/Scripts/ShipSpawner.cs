using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Utils;

namespace Assets.Scripts
{
    public class ShipSpawner : UnitySingleton<ShipSpawner>
    {
        public int Player1Ships;
        public int Player2Ships;

        public void CreateShips(Player owner)
        {
            var spawnAmount = owner.Number==1 ? Player1Ships : Player2Ships;
        }

    }
}
