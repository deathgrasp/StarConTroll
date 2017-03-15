using Assets.Scripts;
using UnityEngine;
using UnityTest;
/*
 * Option determines the test scenario:
 * case 1: tests movement from -2,0,0 to 0,0,0  
 * case 2: tests movement from -2,0,0 to 0,0,0 AND being in the same location as the points in the path
 * case 3: tests missile damage ship by missile damage amount
 */
namespace Assets.Test_suits
{
    public class TestSuit : MonoBehaviour
    {
        public int Option;
        public float CallTest;
        public Ship Ship;
        public SpaceObject SO;
        private bool _pass;
        private int _counter = 0;
        void Start()
        {
            CallTest = ConfigurationManager.TurnDuration;
            switch (Option)//makes sure stuff like destination is always correct, instead of counting on unity's Start build order.
            {
                case 3://tests missile damage ship by missile damage amount
                    _counter = Ship.Health;
                    break;
            }
        }

        void FixedUpdate()
        {
            switch (Option)//makes sure stuff like destination is always correct, instead of counting on unity's Start build order.
            {
                case 1://tests movement from -2,0,0 to 0,0,0
                    ShipMovementTestInit();
                    break;
                case 2://tests movement from -2,0,0 to 0,0,0 AND being in the same location as the points in the path
                    ShipMovementTestInit();
                    if (_counter==0)
                    {
                        PathsManager.Instance.DrawPath(Ship, Vector3.zero);
                    }
                    break;
            }
            switch (Option)
            {
                case 2:

                    if (_counter<Ship.LineRenderer.numPositions&&Ship.LineRenderer.GetPosition(_counter) != Ship.transform.position)
                    {
                        IntegrationTest.Fail();
                    }
                    _counter++;

                    break;
            }
        }
        void Update()
        {

            CallTest -= Time.deltaTime;
            if (CallTest - ConfigurationManager.FloatingPoint <= 0)
            {
                switch (Option)
                {
                    case 1:
                        if (Ship.transform.position == Vector3.zero)
                        {
                            _pass = true;
                        }
                        break;
                    case 2://fail case in fixed update
                        _pass = true;
                        break;
                    case 3:
                        _pass = Ship.Health == (_counter - ((Missile) SO).Damage);
                        break;
                }
                if (_pass)
                {
                    IntegrationTest.Pass();
                }
                else
                {
                    IntegrationTest.Fail();
                }
            }
        }

        private void ShipMovementTestInit()
        {
            Ship.SetDestination(Vector3.zero);
        }

    }
}
