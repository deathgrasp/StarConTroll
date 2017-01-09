using UnityEngine;

namespace Assets.Scripts
{
    public class EndButtonScripts : MonoBehaviour {

        public void ResumeTime()
        {
            Time.timeScale = 1;
            TurnManager.Instance.ToNextTurn = ConfigurationManager.Instance.TurnDuration;  
            print("look at me    "+TurnManager.Instance.ToNextTurn);
        }

        public void OnMouseEnter()
        {
            InputManager.Instance.LineRenderer.enabled = false;
        }

        public void OnMouseExit()
        {
            InputManager.Instance.LineRenderer.enabled = true;
        }
    }
}
