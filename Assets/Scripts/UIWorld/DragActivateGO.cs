using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragActivateGO : MonoBehaviour {
    public GameObject goActivate;

    [Header("Signals")]
    public SignalDragWidget signalDragBegin;
    public SignalDragWidget signalDragEnd;

    void OnDisable() {
        if(signalDragBegin)
            signalDragBegin.callback -= OnDragBegin;

        if(signalDragEnd)
            signalDragEnd.callback -= OnDragEnd;

        if(goActivate)
            goActivate.SetActive(false);
    }

    void OnEnable() {
        if(signalDragBegin)
            signalDragBegin.callback += OnDragBegin;

        if(signalDragEnd)
            signalDragEnd.callback += OnDragEnd;

        if(goActivate)
            goActivate.SetActive(false);
    }

    void OnDragBegin(DragWidgetSignalInfo inf) {
        if(goActivate)
            goActivate.SetActive(true);
    }

    void OnDragEnd(DragWidgetSignalInfo inf) {
        if(goActivate)
            goActivate.SetActive(false);
    }
}
