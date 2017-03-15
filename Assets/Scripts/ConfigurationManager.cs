using System;
using Assets.Utils;

namespace Assets.Scripts
{
    public class ConfigurationManager : UnitySingleton<ConfigurationManager>
    {
        public const float FixedUpdateStep=0.02f;
        public const float FloatingPoint = 0.00001f;
        public const float TestTimeScale = 100f;
        public const Boolean VersusAI = true;
        public const float ScrollZoomSpeed = 2f;//mouse wheel zoom speed
    }
}
