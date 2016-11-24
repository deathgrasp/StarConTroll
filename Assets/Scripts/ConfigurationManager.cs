using UnityEngine;
using System.Collections;
using Assets.Utils;

public class ConfigurationManager : UnitySingleton<ConfigurationManager>
{
    public float FixedUpdateStep=0.02f;
    public float TurnDuration = 3f;
}
