using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateForceApply : MonoBehaviour, M8.IPoolSpawn {
    public const string parmDir = "forceApply_d";
    public const string parmForce = "forceApply_f";
    public const string parmDuration = "forceApply_dur";

    [Header("Data")]
    public UnitEntity unit;
    public M8.EntityState state; //which state to apply force

    public float startDelay = 0f;
    public float interval = 0f;
    public float defaultDuration = 1f;
    public GameObject activeGO;
    public GameObject inactiveGO;

    private bool mIsPlaying;
    private float mCurTime;
    private float mCurTimeInterval;

    private Vector2 mDir;
    private Vector2 mForce;
    private float mDuration;
    
    public void Play() {
        if(startDelay > 0f)
            StartCoroutine(DoPlayDelay());
        else {
            mIsPlaying = true;
            ApplyPlayingState();
        }
    }

    public void Stop() {
        ResetPlay();
    }

    void OnDisable() {
        ResetPlay();
    }

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

    void FixedUpdate() {
        if(mIsPlaying) {
            if(mCurTimeInterval < interval) {                
                mCurTimeInterval += Time.fixedDeltaTime;
                return;
            }
            else
                mCurTimeInterval = 0f;

            unit.body.AddForce(mForce, ForceMode2D.Force);

            mCurTime += Time.fixedDeltaTime;
            if(mCurTime >= mDuration)
                ResetPlay();
        }
    }

    void M8.IPoolSpawn.OnSpawned(M8.GenericParams parms) {
        if(parms != null) {
            if(parms.ContainsKey(parmDir))
                mDir = parms.GetValue<Vector2>(parmDir);

            if(parms.ContainsKey(parmForce)) {
                float f = parms.GetValue<float>(parmForce);
                mForce = mDir * f;
            }

            if(parms.ContainsKey(parmDuration))
                mDuration = parms.GetValue<float>(parmDuration);
            else
                mDuration = defaultDuration;
        }
    }

    void OnEntityStateChanged(M8.EntityBase ent) {
        if(unit.state == state) {
            unit.physicsEnabled = true;
            unit.body.bodyType = RigidbodyType2D.Dynamic;
            Play();
        }
    }
    
    IEnumerator DoPlayDelay() {
        yield return new WaitForSeconds(startDelay);
        mIsPlaying = true;
        ApplyPlayingState();
    }

    private void ApplyPlayingState() {
        if(activeGO) activeGO.SetActive(mIsPlaying);
        if(inactiveGO) inactiveGO.SetActive(!mIsPlaying);
    }

    private void ResetPlay() {
        mIsPlaying = false;
        mCurTime = 0f;
        mCurTimeInterval = 0f;

        ApplyPlayingState();
    }
}
