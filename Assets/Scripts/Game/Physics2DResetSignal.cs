using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics2DResetSignal : MonoBehaviour {
    public Rigidbody2D body;
    public M8.Signal signalReset;

    private Vector2 mStartPos;
    private bool mStartBodySimulated;

    void OnDestroy() {
        signalReset.callback -= OnSignalReset;
    }

    void Awake() {
        mStartPos = transform.position;
        mStartBodySimulated = body.simulated;

        signalReset.callback += OnSignalReset;
    }

    void OnSignalReset() {
        transform.position = mStartPos;
        transform.rotation = Quaternion.identity;

        body.velocity = Vector2.zero;
        body.angularVelocity = 0f;
        body.simulated = mStartBodySimulated;
    }
}
