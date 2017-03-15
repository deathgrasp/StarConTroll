using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class AIPlayer : Player
    {
        public AIPlayer(int number, Boolean isHuman = false) : base(number,isHuman)
        {
        }

        public Vector3 IssueOrders(Ship ship) //returns where to shoot missile
            //TODO: turn into a general strategy, not for a single ship
        {
            var startLoc = ship.transform.position;
            var startRotation = ship.transform.rotation;
            var locations = new List<List<Vector3>> {new List<Vector3>()};
            var rotations = new List<List<Quaternion>> {new List<Quaternion>()};
            locations[0].Add(startLoc);
            rotations[0].Add(startRotation);
            locations[0].Add(startLoc);
            rotations[0].Add(startRotation);
            locations[0].Add(startLoc);
            rotations[0].Add(startRotation);
            locations[0].Add(startLoc);
            rotations[0].Add(startRotation);
            for (int i = 0; i*10 < ConfigurationManager.UpdatesInTurn; i++)
            {
                var newLocs = new List<Vector3>();
                var newRots = new List<Quaternion>();
                for (int j = 0; j < 4; j++)
                {
                    ship.transform.position = locations[i][j];
                    ship.transform.rotation = rotations[i][j];
                    switch (j)
                    {
                        case 1:
                            ship.Destination = startLoc + new Vector3(0, 5);
                            break;
                        case 2:
                            ship.Destination = startLoc + new Vector3(0, -5);
                            break;
                        case 3:
                            ship.Destination = startLoc + new Vector3(5, 0);
                            break;
                        case 0:
                            ship.Destination = startLoc + new Vector3(-5, 0);
                            break;
                    }
                    for (int k = 0; k < 10; k++)
                    {
                        ship.Move(ship.Destination);
                    }
                    newLocs.Add(ship.transform.position);
                    newRots.Add(ship.transform.rotation);
                }
                locations.Add(newLocs);
                rotations.Add(newRots);
            }
            var missiles = GameObject.FindGameObjectsWithTag("SpaceObject").Select(obj => obj.GetComponent<Missile>()).ToList();
            missiles.RemoveAll(x => x == null);
            var missilesStartRotation = new List<Quaternion>();
            var missilesStartLocation = new List<Vector3>();
            for (int i = 0; i < missiles.Count; i++)
            {
                missilesStartLocation.Add(missiles[i].transform.position);
                missilesStartRotation.Add(missiles[i].transform.rotation);
            }
            var toRemove=new List<Vector3>();
            for (int i = 0; i < ConfigurationManager.UpdatesInTurn; i++)
            {
                foreach (var missile in missiles)
                {
                    missile.Move(missile.Destination);
                }
                for (int j = 0; j < 1+(i/10); j++)
                {
                    toRemove.AddRange(from v in locations[j] from missile in missiles where ((v - missile.transform.position).sqrMagnitude < 1)||((v-missile.Destination).magnitude<Missile.BlastRadius) select v);//get all locations with distance of less than 1 from any missile
                    //foreach (var v in locations[j])
                    //{
                    //    foreach (var missile in missiles)
                    //    {
                    //        if(((v-missile.transform.position).sqrMagnitude<2)||)
                    //    }
                    //}
                    foreach (var k in toRemove)
                    {
                        locations[j].Remove(k);
                    }
                    toRemove.Clear();
                }
            }
            ship.transform.position = startLoc;
            ship.transform.rotation = startRotation;
            ship.SetDestination(startLoc+new Vector3(5,5));
            var lv3 = locations.First(x => x.Count > 0);
            if (lv3!=null && lv3.Count>0)
            {
                ship.Destination = lv3[0];
            }
            for (int i = 0; i < missiles.Count; i++)
            {
                missiles[i].transform.position = missilesStartLocation[i];
                missiles[i].transform.rotation = missilesStartRotation[i];
            }
            Vector3 destination;
            //ship.Destination;
            if (GameWorld.Instance.Ship1.LineRenderer.numPositions>100)
            {
                destination = GameWorld.Instance.Ship1.LineRenderer.GetPosition(100);
            }
            else
            {
                destination = GameWorld.Instance.Ship1.transform.position;
            }
            return destination;
        }
    }
}
