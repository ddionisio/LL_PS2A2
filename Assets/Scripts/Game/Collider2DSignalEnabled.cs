using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider2DSignalEnabled : MonoBehaviour {
    public Collider2D coll;
    public bool defaultEnabled;

    [Header("Signals")]
    public M8.Signal signalEnable;
    public M8.Signal signalDisable;

    void OnDisable() {
        if(signalEnable) signalEnable.callback -= OnSignalEnable;
        if(signalDisable) signalDisable.callback -= OnSignalDisable;
    }

    void OnEnable() {
        coll.enabled = defaultEnabled;

        if(signalEnable) signalEnable.callback += OnSignalEnable;
        if(signalDisable) signalDisable.callback += OnSignalDisable;
    }

    void OnSignalEnable() {
        coll.enabled = true;
    }

    void OnSignalDisable() {
        coll.enabled = false;
    }
}
