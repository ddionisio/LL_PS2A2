using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEntity : M8.EntityBase {
    public const string parmPosition = "pos";
    public const string parmNormal = "norm";

    [Header("States")]
    public M8.EntityState stateOnSpawn;

    public Vector2 position {
        get {
            return mPhysicsEnabled && body ? body.position : (Vector2)transform.position;
        }

        set {
            if(mPhysicsEnabled && body)
                body.position = value;
            else
                transform.position = value;
        }
    }

    public Rigidbody2D body { get; private set; }
    public Rigidbody2DMoveController bodyMoveCtrl { get; private set; }

    public Collider2D coll { get; private set; }    

    public bool physicsEnabled {
        get { return mPhysicsEnabled; }
        set {
            if(mPhysicsEnabled != value) {
                mPhysicsEnabled = value;
                ApplyPhysicsEnabled();
            }
        }
    }

    protected Coroutine mRout;

    private bool mPhysicsEnabled;

    protected override void StateChanged() {
        StopCurrentRoutine();
    }

    protected override void OnDespawned() {
        //reset stuff here
        StopCurrentRoutine();

        physicsEnabled = false;

        transform.rotation = Quaternion.identity;        
    }

    protected override void OnSpawned(M8.GenericParams parms) {
        //populate data/state for ai, player control, etc.

        //start ai, player control, etc

        if(parms != null) {
            if(parms.ContainsKey(parmPosition)) {
                Vector2 pt;

                object ptObj = parms.GetValue<object>(parmPosition);
                if(ptObj is Vector2)
                    pt = (Vector2)ptObj;
                else if(ptObj is Vector3)
                    pt = (Vector3)ptObj;
                else
                    pt = position;

                position = pt;
            }

            if(parms.ContainsKey(parmNormal)) {
                Vector2 norm = parms.GetValue<Vector2>(parmNormal);
                transform.rotation = Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, norm), Vector3.forward);
            }
        }

        if(stateOnSpawn) state = stateOnSpawn;
    }

    protected override void OnDestroy() {
        //dealloc here

        base.OnDestroy();
    }

    protected override void Awake() {
        base.Awake();

        //initialize data/variables
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        bodyMoveCtrl = GetComponent<Rigidbody2DMoveController>();

        mPhysicsEnabled = false;
        ApplyPhysicsEnabled();
    }

    // Use this for one-time initialization
    protected override void Start() {
        base.Start();

        //initialize variables from other sources (for communicating with managers, etc.)
    }

    protected void StopCurrentRoutine() {
        if(mRout != null) {
            StopCoroutine(mRout);
            mRout = null;
        }
    }

    private void ApplyPhysicsEnabled() {
        if(mPhysicsEnabled) {
            if(body) body.simulated = true;
            if(coll) coll.enabled = true;
        }
        else {
            if(body) {
                body.velocity = Vector2.zero;
                body.angularVelocity = 0f;
                body.simulated = false;
            }

            if(coll) coll.enabled = false;

            if(bodyMoveCtrl) {
                bodyMoveCtrl.ResetCollision();
                bodyMoveCtrl.RevertSpeedCap();
            }
        }
    }
}
