using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragGOActivate : MonoBehaviour {
    [Header("Data")]
    public GameObject activeGO;
    public GameObject inactiveGO;

    [Header("Signals")]
    public SignalDragWidget signalDragBegin;
    public SignalDragWidget signalDragEnd;

    void OnDestroy() {
        if(signalDragBegin) signalDragBegin.callback -= OnSignalDragBegin;
        if(signalDragEnd) signalDragEnd.callback -= OnSignalDragEnd;
    }

    void Awake() {
        if(activeGO) activeGO.SetActive(false);
        if(inactiveGO) inactiveGO.SetActive(true);

        if(signalDragBegin) signalDragBegin.callback += OnSignalDragBegin;
        if(signalDragEnd) signalDragEnd.callback += OnSignalDragEnd;
    }

    void OnSignalDragBegin(DragWidgetSignalInfo inf) {
        if(activeGO) activeGO.SetActive(true);
        if(inactiveGO) inactiveGO.SetActive(false);
    }

    void OnSignalDragEnd(DragWidgetSignalInfo inf) {
        if(activeGO) activeGO.SetActive(false);
        if(inactiveGO) inactiveGO.SetActive(true);
    }
}
