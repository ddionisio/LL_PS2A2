using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigidbody2DEnableSignal : MonoBehaviour {
    public Rigidbody2D body;
    public M8.Signal signalEnabled;

    void OnDestroy() {
        signalEnabled.callback -= OnSignalEnabled;
    }

    void Awake() {
        signalEnabled.callback += OnSignalEnabled;
    }

    void OnSignalEnabled() {
        body.simulated = true;
    }
}
