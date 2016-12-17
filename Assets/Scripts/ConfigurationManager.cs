using Assets.Utils;

namespace Assets.Scripts
{
    public class ConfigurationManager : UnitySingleton<ConfigurationManager>
    {
        public float FixedUpdateStep=0.02f;
        public float TurnDuration = 3f;
    }
}
