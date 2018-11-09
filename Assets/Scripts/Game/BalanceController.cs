using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceController : MonoBehaviour {
    [Header("Beam")]
    public Transform beamRoot;
    public Rigidbody2D beamBody;
    public float beamRestAngularSpeed;
    public float beamRestDelay = 1f;
    public float beamRestAngularDrag;

    [Header("Unlock")]
    public float unlockBeamMinRotate = 5f;
    public GameObject[] unlockGOActives;

    [Header("Signals")]
    public M8.Signal signalUnlocked;
    public M8.Signal signalLocked;

    private bool mIsLocked;
    private float mBeamDefaultAngularDrag;
    private Coroutine mBeamRestRout;
    private bool mIsRested;

    void Awake() {
        mBeamDefaultAngularDrag = beamBody.angularDrag;

        mIsLocked = true;
        ApplyLocked();
    }

    void Update() {
        float curAngleSpeed = Mathf.Abs(beamBody.angularVelocity);

        if(curAngleSpeed <= beamRestAngularSpeed) {
            if(!mIsRested) {
                //rest a bit
                if(mBeamRestRout == null)
                    mBeamRestRout = StartCoroutine(DoBeamRest());

                mIsRested = true;
            }
            else {
                UpdateLocked();
            }
        }
        else {
            mIsRested = false;

            if(!mIsLocked) {
                mIsLocked = true;
                ApplyLocked();

                if(signalLocked)
                    signalLocked.Invoke();
            }
        }
	}

    void ApplyLocked() {
        for(int i = 0; i < unlockGOActives.Length; i++) {
            if(unlockGOActives[i])
                unlockGOActives[i].SetActive(!mIsLocked);
        }
    }

    IEnumerator DoBeamRest() {
        beamBody.angularDrag = beamRestAngularDrag;

        yield return new WaitForSeconds(beamRestDelay);

        beamBody.angularDrag = mBeamDefaultAngularDrag;

        mBeamRestRout = null;

        //check if unlocked
        UpdateLocked();
    }

    void UpdateLocked() {
        float angle = Vector2.Angle(beamRoot.up, Vector2.up);

        bool isLocked = angle > unlockBeamMinRotate;

        if(mIsLocked != isLocked) {
            mIsLocked = isLocked;
            ApplyLocked();

            if(mIsLocked) {
                if(signalLocked)
                    signalLocked.Invoke();
            }
            else {
                if(signalUnlocked)
                    signalUnlocked.Invoke();
            }
        }
    }
}
