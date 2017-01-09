using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurnManager : UnitySingleton<TurnManager>
    {

        public float ToNextTurn;
        // Use this for initialization
        void Start ()
        {
            ToNextTurn = ConfigurationManager.Instance.FixedUpdateStep;
            ToNextTurn = 0.02f;
            print(ToNextTurn);
        }

        private int counter = 0;
        // Update is called once per frame
        void FixedUpdate ()
        {
            var step = ConfigurationManager.Instance.FixedUpdateStep;
            ToNextTurn -= step;
            if (ToNextTurn-step<0)
            {
                Time.timeScale = 0;
            }
            // float a = Mathf.Round(ToNextTurn*10000)/10000; //truncation of float in order to avoid rounding issue
        }
    }
}
