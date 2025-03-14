using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModeType
{
    singleMode,
    multiMode
}

public class JSYModeManager : SingletonManager<JSYModeManager>
{
    public ModeType currentGameMode;
    
}
