using UnityEngine;

namespace Assets.Scripts
{
    public class ButtonScripts : MonoBehaviour {

        public void ResumeTime()
        {
            Time.timeScale = 1;
            TurnManager.Instance.ToNextTurn = ConfigurationManager.Instance.TurnDuration;  
        }
    }
}
