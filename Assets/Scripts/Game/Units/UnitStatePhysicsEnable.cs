using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simply turn on physics on specific states
/// </summary>
public class UnitStatePhysicsEnable : MonoBehaviour {
    [System.Serializable]
    public struct Data {
        public M8.EntityState state;
        public RigidbodyType2D bodyType;
    }

    [Header("Data")]
    public UnitEntity unit;
    public Data[] states;

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
            var dat = states[i];
            if(unit.state == dat.state) {
                unit.physicsEnabled = true;
                unit.body.bodyType = dat.bodyType;
                break;
            }
        }
    }
}
