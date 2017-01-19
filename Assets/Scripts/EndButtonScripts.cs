using UnityEngine;

namespace Assets.Scripts
{
    public class EndButtonScripts : MonoBehaviour {

        public void ResumeTime()
        {
            Time.timeScale = 1;
            TurnManager.Instance.ToNextTurn = (int)(ConfigurationManager.Instance.TurnDuration/ConfigurationManager.Instance.FixedUpdateStep);  
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
