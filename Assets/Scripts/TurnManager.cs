using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurnManager : UnitySingleton<TurnManager>
    {
        // TODO: merge this class into GameWorld?
        public int ToNextTurn=0;
        // Use this for initialization
        void Awake ()
        {
            ToNextTurn = 0;
            GameWorld.Instance.OnExecutionPhaseEnd();
        }

        // Update is called once per frame
        void FixedUpdate ()
        {
            print("update");
            ToNextTurn -= 1;
            if (ToNextTurn < 1)
            {
                GameWorld.Instance.OnExecutionPhaseEnd();
            }
            // float a = Mathf.Round(ToNextTurn*10000)/10000; //truncation of float in order to avoid rounding issue
        }
    }
}
