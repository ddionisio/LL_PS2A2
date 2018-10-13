using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simply turn on physics on specific states
/// </summary>
public class UnitStatePhysicsEnable : MonoBehaviour {
    [Header("Data")]
    public UnitEntity unit;
    public M8.EntityState[] states;

    void OnDestroy() {
        if(unit) {
            unit.setStateCallback -= OnEntityStateChanged;
        }
    }

    void Awake() {
        if(!unit)
            unit = GetComponent<UnitEntity>();

        unit.setStateCallback += OnEntityStateChanged;
    }

    void OnEntityStateChanged(M8.EntityBase ent) {
        for(int i = 0; i < states.Length; i++) {
            if(unit.state == states[i]) {
                unit.physicsEnabled = true;
                break;
            }
        }
    }
}
