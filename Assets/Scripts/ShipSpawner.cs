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

        private static Material _player1materialPrefab;
        public static Material Player1MaterialPrefabPrefab
        {
            get { return _player1materialPrefab ?? (_player1materialPrefab = Resources.Load<Material>("Player1Material")); }
        }

        private static Material _player2materialPrefab;
        public static Material Player2MaterialPrefabPrefab
        {
            get { return _player2materialPrefab ?? (_player2materialPrefab = Resources.Load<Material>("Player2Material")); }
        }

        public int Player1Ships=5;
        public int Player2Ships=5;

        public List<Ship> CreateShips(Player owner)
        {
            var list = new List<Ship>();
            var spawnAmount = owner.Number==1 ? Player1Ships : Player2Ships;
            var pos = owner.Number == 1 ? new Vector3(-spawnAmount, -spawnAmount, 0) : new Vector3(spawnAmount, -spawnAmount, 0);
            for (int i = 0; i < spawnAmount; i++)
            {
                var ship=Instantiate(ShipPrefab, pos, Quaternion.FromToRotation(pos, Vector3.zero)) as Ship;
                ship.RotateShipTowards(new Vector3(0,pos.y,0),360);
                pos+=Vector3.up*2;
                ship.Owner = owner;
                list.Add(ship);
                ship.name="Ship #"+(i+1)+" of player "+owner.Number;
                ship.transform.GetChild(0).GetComponent<Renderer>().material=(owner.Number == 1
                        ? Player1MaterialPrefabPrefab
                        : Player2MaterialPrefabPrefab);
            }
            GameWorld.Instance.Camera.orthographicSize += spawnAmount;
            return list;
        }

    }
}
