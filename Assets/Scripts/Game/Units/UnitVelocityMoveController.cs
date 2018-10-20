using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitVelocityMoveController : MonoBehaviour, M8.IPoolSpawn {
    public const string parmDir = "unitVelocityMoveCtrl_d";
    public const string parmSpeed = "unitVelocityMoveCtrl_s";
    public const string parmAccel = "unitVelocityMoveCtrl_a";

    public UnitEntity unit;
    public M8.EntityState state;

    [Header("Data")]
    public Vector2 dir;
    public float accel;
    public float initialSpeed;
    public float maxSpeed;

    [Header("Physics")]
    public RigidbodyType2D rigidbodyMode = RigidbodyType2D.Kinematic;
    public bool applyRigidbodyMode = true;

    private Vector2 mDir;
    private float mInitSpeed;
    private float mAccel;

    private RigidbodyType2D mPreviousRigidbodyMode;
    private Vector2 mCurVelocity;

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
        if(unit.state == state) {
            if(unit.body.bodyType == RigidbodyType2D.Dynamic) { //apply velocity to body and let physics deal with it
                if(mAccel != 0f) {
                    if(unit.body.velocity.sqrMagnitude < maxSpeed * maxSpeed)
                        unit.body.velocity += dir * mAccel * Time.fixedDeltaTime;
                }
            }
            else { //apply position manually
                if(mAccel != 0f) {
                    if(mCurVelocity.sqrMagnitude < maxSpeed * maxSpeed)
                        mCurVelocity += dir * mAccel * Time.fixedDeltaTime;
                }

                var curPos = unit.body.position;
                curPos += mCurVelocity * Time.fixedDeltaTime;

                unit.body.position = curPos;
            }
        }
    }

    void M8.IPoolSpawn.OnSpawned(M8.GenericParams parms) {
        if(parms != null) {
            if(parms.ContainsKey(parmDir))
                mDir = parms.GetValue<Vector2>(parmDir);
            else
                mDir = dir;

            if(parms.ContainsKey(parmSpeed))
                mInitSpeed = parms.GetValue<float>(parmSpeed);
            else
                mInitSpeed = initialSpeed;

            if(parms.ContainsKey(parmAccel))
                mAccel = parms.GetValue<float>(parmAccel);
            else
                mAccel = accel;
        }
    }

    void OnEntityStateChanged(M8.EntityBase ent) {
        if(unit.prevState == state) {
            if(applyRigidbodyMode)
                unit.body.bodyType = mPreviousRigidbodyMode;
        }

        if(unit.state == state) {
            unit.physicsEnabled = true;

            mCurVelocity = mDir * mInitSpeed;

            if(applyRigidbodyMode) {
                mPreviousRigidbodyMode = unit.body.bodyType;
                unit.body.bodyType = rigidbodyMode;
            }

            if(unit.body.bodyType == RigidbodyType2D.Dynamic)
                unit.body.velocity = mCurVelocity;
        }
    }
}
