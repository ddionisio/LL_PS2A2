using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActCannonController : GameModeController<ActCannonController> {
    [Header("Force")]
    public Slider forceSlider;
    public float forceStart = 16f;
    public float forceMin = 10f;
    public float forceMax = 50f;

    [Header("Angle")]
    public SliderRadial angleSlider;
    public float angleStart;
    public float angleMin = 15f;
    public float angleMax = 80f;

    [Header("General Interface")]
    public GameObject cannonInterfaceGO;
    public Selectable cannonLaunch;
    public GameObject nextInterfaceGO;    

    [Header("Data")]
    public int cannonballCount = 10;

    [Header("Targets")]
    [M8.TagSelector]
    public string targetTag;
    public M8.EntityState targetStateDespawn;

    [Header("Signals")]
    public M8.Signal signalLaunched;
    public M8.Signal signalNext;

    public int cannonballLaunced { get { return mCannonballLaunched; } }

    public event System.Action cannonballLaunchedCallback;
        
    protected bool mIsCannonballFree = false; //set to true to not decrement cannonball count on launch

    protected bool mIsLaunchWait = false;
    protected bool mIsNextWait = false;

    private int mCannonballLaunched = 0;

    private M8.CacheList<M8.EntityBase> mActiveTargets;

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        if(forceSlider) {
            forceSlider.minValue = forceMin;
            forceSlider.maxValue = forceMax;
            forceSlider.value = forceStart;
        }

        if(angleSlider) {
            angleSlider.minValue = angleMin;
            angleSlider.maxValue = angleMax;
            angleSlider.startAngle = -angleMin;
            angleSlider.endAngle = -angleMax;
            angleSlider.value = angleStart;
        }

        var targetGOs = GameObject.FindGameObjectsWithTag(targetTag);

        var entList = new List<M8.EntityBase>();
        for(int i = 0; i < targetGOs.Length; i++) {
            var ent = targetGOs[i].GetComponent<M8.EntityBase>();
            if(ent)
                entList.Add(ent);
        }

        mActiveTargets = new M8.CacheList<M8.EntityBase>(entList.Count);
        for(int i = 0; i < entList.Count; i++) {
            var ent = entList[i];

            ent.setStateCallback += OnTargetChangeState;
            ent.releaseCallback += OnTargetReleased;

            mActiveTargets.Add(ent);
        }

        signalLaunched.callback += OnSignalLaunched;
        signalNext.callback += OnSignalNext;

        //start with interfaces hidden and disabled
        cannonInterfaceGO.SetActive(false);
        nextInterfaceGO.SetActive(false);

        SetInteractiveEnable(false);
    }

    protected override void OnInstanceDeinit() {
        signalLaunched.callback -= OnSignalLaunched;
        signalNext.callback -= OnSignalNext;

        if(mActiveTargets != null) {
            for(int i = 0; i < mActiveTargets.Count; i++) {
                if(mActiveTargets[i]) {
                    mActiveTargets[i].setStateCallback -= OnTargetChangeState;
                    mActiveTargets[i].releaseCallback -= OnTargetReleased;
                }
            }
            mActiveTargets.Clear();
        }

        base.OnInstanceDeinit();
    }

    protected virtual void OnFinish() {

    }

    protected virtual void OnSignalLaunched() {
        mIsLaunchWait = false;

        if(!mIsCannonballFree) {
            if(mCannonballLaunched < cannonballCount) {
                mCannonballLaunched++;
                if(mCannonballLaunched == cannonballCount)
                    OnFinish();
            }
        }
        else
            mIsCannonballFree = false;

        if(cannonballLaunchedCallback != null)
            cannonballLaunchedCallback();
    }

    protected virtual void OnSignalNext() {
        mIsNextWait = false;
    }

    protected void SetInteractiveEnable(bool interact) {
        if(forceSlider) forceSlider.interactable = interact;
        if(angleSlider) angleSlider.interactable = interact;
        if(cannonLaunch) cannonLaunch.interactable = interact;
    }

    void OnTargetChangeState(M8.EntityBase ent) {
        //targetStateDespawn
        if(ent.state == targetStateDespawn) {
            RemoveTarget(ent);
            if(mActiveTargets.Count == 0)
                OnFinish();
        }
    }

    void OnTargetReleased(M8.EntityBase ent) {
        //fail safe if not despawned by released somehow
        RemoveTarget(ent);
        if(mActiveTargets.Count == 0)
            OnFinish();
    }

    private void RemoveTarget(M8.EntityBase ent) {
        if(mActiveTargets.Remove(ent)) {
            ent.setStateCallback -= OnTargetChangeState;
            ent.releaseCallback -= OnTargetReleased;
        }
    }
}
