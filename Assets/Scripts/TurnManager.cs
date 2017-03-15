using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurnManager : UnitySingleton<TurnManager>
    {
        // TODO: merge this class into GameWorld?
        public int CurrentUpdate = -1; // TODO: add getter/setter
        // Use this for initialization
        void Start ()
        {
            CurrentUpdate = -1;
        }

        // Update is called once per frame
        void FixedUpdate ()
        {
            CurrentUpdate += 1;
            if (CurrentUpdate > ConfigurationManager.UpdatesInTurn-1) // TODO: should it be UpdatesInTurn-1? or just UpdatesInTurn?
            {
                GameWorld.Instance.OnExecutionPhaseEnd();
            }
            // float a = Mathf.Round(CurrentUpdate*10000)/10000; //truncation of float in order to avoid rounding issue
        }
    }
}
