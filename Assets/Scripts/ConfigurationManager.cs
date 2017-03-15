using System;
using Assets.Utils;

namespace Assets.Scripts
{
    public class ConfigurationManager : UnitySingleton<ConfigurationManager>
    {
        public Boolean VersusAI = false;
        public const float ScrollZoomSpeed = 2f;//mouse wheel zoom speed

        public const float FixedUpdateStep = 0.02f;
        public const float TurnDuration = 3f;
        public const float FloatingPoint = 0.00001f;
        public const float TestTimeScale = 100f;
        public const int UpdatesInTurn = (int)(TurnDuration / FixedUpdateStep); // TODO: change to "public  int UpdatesInTurn {get {return (int)(TurnDuration/FixedUpdateStep};" and remove const from the rest of the vars here
    }
}
