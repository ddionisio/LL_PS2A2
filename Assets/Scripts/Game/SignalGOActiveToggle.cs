using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class SignalGOActiveToggle : MonoBehaviour {
    public enum State {
        None,
        Activate,
        Deactivate
    }

    public GameObject targetGO;
    public bool defaultActive;
    public float delay;
    public string activeSfxPath;
    public Rigidbody2D activeLockBody;

    [Header("Signal")]
    public M8.Signal signalActive;
    public M8.Signal signalInactive;

    private State mState = State.None;
    private float mCurTime;

    void OnDisable() {
        if(signalActive) signalActive.callback -= OnSignalActive;
        if(signalInactive) signalInactive.callback -= OnSignalInactive;

        mState = State.None;
    }

    void OnEnable() {
        if(signalActive) signalActive.callback += OnSignalActive;
        if(signalInactive) signalInactive.callback += OnSignalInactive;

        targetGO.SetActive(defaultActive);
    }

    void Update() {
        switch(mState) {
            case State.Activate:
            case State.Deactivate:
                mCurTime += Time.deltaTime;
                if(mCurTime >= delay) {
                    if(mState == State.Activate) {
                        targetGO.SetActive(true);

                        if(!string.IsNullOrEmpty(activeSfxPath))
                            LoLManager.instance.PlaySound(activeSfxPath, false, false);

                        if(activeLockBody) {
                            activeLockBody.bodyType = RigidbodyType2D.Static;
                        }
                    }
                    else
                        targetGO.SetActive(false);

                    mState = State.None;
                }
                break;
        }
    }

    void OnSignalActive() {
        mState = State.Activate;
        mCurTime = 0f;
    }

    void OnSignalInactive() {
        mState = State.Deactivate;
        mCurTime = 0f;
    }
}
