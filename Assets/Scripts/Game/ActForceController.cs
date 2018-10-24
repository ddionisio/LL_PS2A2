using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is for Act 3 levels
/// </summary>
public class ActForceController : GameModeController<ActForceController> {
    [Header("Game")]
    public GameObject forceGroupGO;
    public GameObject playGroupGO;

    [Header("Signal")]
    public M8.Signal signalGoal;
}
