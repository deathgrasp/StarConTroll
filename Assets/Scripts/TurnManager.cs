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
            ToNextTurn = ConfigurationManager.Instance.TurnDuration;
        }
	
        // Update is called once per frame
        void FixedUpdate ()
        {
            ToNextTurn -= ConfigurationManager.Instance.FixedUpdateStep;
            if (ToNextTurn<=0)
            {
                Time.timeScale = 0;
            }
        }
    }
}
