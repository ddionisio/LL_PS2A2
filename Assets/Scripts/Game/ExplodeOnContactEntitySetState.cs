using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnContactEntitySetState : ExplodeOnContact {
    [Header("Entity")]
    public M8.EntityState toState;

    protected override void OnExplode(Rigidbody2D body) {
        var ent = body.GetComponent<M8.EntityBase>();
        if(ent)
            ent.state = toState;
    }
}
