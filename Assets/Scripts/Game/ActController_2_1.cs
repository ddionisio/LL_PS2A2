using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActController_2_1 : GameModeController<ActController_2_1> {
    [Header("Sequence")]
    public GameObject cannonInterfaceGO;
    public Selectable cannonForceInputUI;
    public Selectable cannonLaunchUI;

    public ProjectileEntitySpawner projSpawner;

    public GameObject nextInterfaceGO;

    [Header("Target")]
    public M8.SignalEntity targetSignalStateChanged;
    public M8.EntityState targetEndState;
    public int targetHitQuota = 3;
    public GameObject targetHitActiveGO; //when target has been hit

    [Header("Signals")]
    public SignalFloat signalLaunch;
    public M8.Signal signalNext;

    private Vector2 mDir = new Vector2(1f, 0f);
    private bool mIsForceProceedWait;
    private bool mIsProceedWait;
    private int mLaunchCount;
    private bool mIsTargetHit;
    private int mScore;

    protected override void OnInstanceDeinit() {
        //
        signalLaunch.callback -= OnSignalLaunch;
        signalNext.callback -= OnSignalNext;
        targetSignalStateChanged.callback -= OnTargetStateChanged;

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        cannonInterfaceGO.SetActive(false);
        nextInterfaceGO.SetActive(false);

        targetHitActiveGO.SetActive(false);

        SetInteractiveEnable(false);

        //
        signalLaunch.callback += OnSignalLaunch;
        signalNext.callback += OnSignalNext;
        targetSignalStateChanged.callback += OnTargetStateChanged;
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //intro part

        cannonInterfaceGO.SetActive(true);

        //some other stuff?

        //enable cannon launch
        cannonLaunchUI.interactable = true;

        //wait for launch
        mIsForceProceedWait = true;
        while(mIsForceProceedWait)
            yield return null;

        //explanations and such

        //enable force input
        cannonForceInputUI.interactable = true;

        //start checking for target
        mLaunchCount = 0;
        mIsTargetHit = false;

        //enable next
        nextInterfaceGO.SetActive(true);

        //wait for proceed
        mIsProceedWait = true;
        while(mIsProceedWait)
            yield return null;

        //progress
        GameData.instance.Progress();
    }

    void OnSignalLaunch(float force) {
        mIsForceProceedWait = false;

        mLaunchCount++;

        //spawn
        projSpawner.Spawn(mDir, force);
    }

    void OnSignalNext() {
        mIsProceedWait = false;
    }

    void OnTargetStateChanged(M8.EntityBase ent) {
        if(ent.state == targetEndState) {
            if(mIsTargetHit)
                return;

            mIsTargetHit = true;

            //apply score
            mScore = GameData.instance.ComputeHitScore(targetHitQuota, mLaunchCount);
            LoLManager.instance.curScore += mScore;

            targetHitActiveGO.SetActive(true);
        }
    }

    private void SetInteractiveEnable(bool interact) {
        cannonForceInputUI.interactable = interact;
        cannonLaunchUI.interactable = interact;
    }
}