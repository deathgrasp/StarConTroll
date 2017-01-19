using Assets.Scripts;
using UnityEngine;
using UnityTest;

namespace Assets.Test_suits
{
    public class ShipTest : MonoBehaviour
    {
        public int Option;
        public float CallTest;
        public Ship Ship;
        private bool _pass;
        void Start()
        {
            CallTest = ConfigurationManager.Instance.TurnDuration;
            Time.timeScale = ConfigurationManager.Instance.TestTimeScale;
            switch (Option)
            {
                case 1:
                    MovementTest1Init();
                    break;
            }
        }

        void Update()
        {

            CallTest -= Time.deltaTime;
            if (CallTest - ConfigurationManager.Instance.FloatingPoint <= 0)
            {
                switch (Option)
                {
                    case 1:
                        MovementTest1();
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

        private void MovementTest1Init()
        {
            Ship.SetDestination(Vector3.zero);
        }

        private void MovementTest1()
        {
            if (Ship.transform.position == Vector3.zero)
            {
                _pass = true;
            }
        }

    }
}
