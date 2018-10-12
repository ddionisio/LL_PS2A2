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

    [Header("Signals")]
    public SignalFloat signalLaunch;
    public M8.Signal signalNext;

    private Vector2 mDir = new Vector2(1f, 0f);
    private bool mIsForceProceedWait;
    private bool mIsProceedWait;

    protected override void OnInstanceDeinit() {
        //
        signalLaunch.callback -= OnSignalLaunch;
        signalNext.callback -= OnSignalNext;

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        cannonInterfaceGO.SetActive(false);
        nextInterfaceGO.SetActive(false);

        SetInteractiveEnable(false);

        //
        signalLaunch.callback += OnSignalLaunch;
        signalNext.callback += OnSignalNext;
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

        //spawn
        projSpawner.Spawn(mDir, force);
    }

    void OnSignalNext() {
        mIsProceedWait = false;
    }

    private void SetInteractiveEnable(bool interact) {
        cannonForceInputUI.interactable = interact;
        cannonLaunchUI.interactable = interact;
    }
}