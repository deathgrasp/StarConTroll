
using Assets.Scripts;
using UnityEngine;

namespace Assets.Test_suits 
{
    class SetTestSpeed : MonoBehaviour
    {
        void Start()
        {
            Time.timeScale = ConfigurationManager.TestTimeScale;
        }
    }
}
