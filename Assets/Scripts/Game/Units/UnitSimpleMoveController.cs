using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Just move at a direction, requires Rigidbody2DMoveController
/// </summary>
public class UnitSimpleMoveController : MonoBehaviour {
    public UnitEntity unit;

    [Header("Move Data")]
    public M8.EntityState moveState; //which state to do moving
    public float moveHorizontal; //unit scalar for which direction to move horizontally
    public bool moveHorizontalIsGrounded; //only move horizontal if grounded?
    public float moveVertical; //unit scalar for which direction to move vertically

    void OnDestroy() {
        if(unit)
            unit.setStateCallback -= OnEntityStateChanged;
    }

    void Awake() {
        if(!unit)
            unit = GetComponent<UnitEntity>();

        unit.setStateCallback += OnEntityStateChanged;
    }

    void FixedUpdate() {
        if(unit.state != moveState)
            return;

        if(!unit.physicsEnabled)
            return;

        if(!unit.bodyMoveCtrl)
            return;

        //horizontal
        if(!moveHorizontalIsGrounded || unit.bodyMoveCtrl.isGrounded)
            unit.bodyMoveCtrl.moveHorizontal = moveHorizontal;
        else
            unit.bodyMoveCtrl.moveHorizontal = 0f;

        //vertical
        unit.bodyMoveCtrl.moveVertical = moveVertical;
    }

    void OnEntityStateChanged(M8.EntityBase ent) {
        if(unit.prevState == moveState) {
            unit.physicsEnabled = true;
        }
    }
}
