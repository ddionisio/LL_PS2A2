using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceUnitDirTransformUp : MonoBehaviour {

    public Transform target;
    public Rigidbody2D body;

    private Vector2 mLastVel;

    void OnEnable() {
        mLastVel = body.simulated ? body.velocity : Vector2.zero;
    }

    void FixedUpdate() {
		if(body.simulated) {
            var vel = body.velocity;
            if(mLastVel != vel) {
                var dVel = vel - mLastVel;

                target.up = dVel.normalized;
                target.gameObject.SetActive(true);

                mLastVel = vel;
            }
            else
                target.gameObject.SetActive(false);
        }
        else {
            mLastVel = Vector2.zero;
            target.gameObject.SetActive(false);
        }
	}
}
