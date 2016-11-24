using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class TurnTimer : MonoBehaviour
    {
        public Text Text;
        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update ()
        {
            Text.text = TurnManager.Instance.ToNextTurn.ToString();
        }
    }
}
