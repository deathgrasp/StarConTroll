using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class ShipSpawner : UnitySingleton<ShipSpawner>
    {
        private static Ship _shipPrefab;
        public static Ship ShipPrefab
        {
            get { return _shipPrefab ?? (_shipPrefab = Resources.Load<Ship>("Ship")); }
        }
        public int Player1Ships;
        public int Player2Ships;

        public void CreateShips(Player owner)
        {
            var spawnAmount = owner.Number==1 ? Player1Ships : Player2Ships;
            var pos = owner.Number == 1 ? new Vector3(-spawnAmount, -spawnAmount/2, 0) : new Vector3(spawnAmount, -spawnAmount/2, 0);
            for (int i = 0; i < spawnAmount; i++)
            {
                //Instantiate(ShipPrefab,)
            }
        }

    }
}
