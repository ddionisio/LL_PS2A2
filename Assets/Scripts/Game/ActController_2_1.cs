using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActController_2_1 : ActCannonController {
    [Header("Sequence")]
    public GameObject cannonInterfaceGO;
    public Selectable cannonForceInputUI;
    public Selectable cannonLaunchUI;

    public GameObject nextInterfaceGO;

    protected override void OnInstanceDeinit() {
        //
        
        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        cannonInterfaceGO.SetActive(false);
        nextInterfaceGO.SetActive(false);

        SetInteractiveEnable(false);

        //
        
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //intro part

        cannonInterfaceGO.SetActive(true);

        //some other stuff?

        //enable cannon launch
        cannonLaunchUI.interactable = true;

        //wait for launch
        mIsCannonballFree = true; //free shot
        mIsLaunchWait = true;
        while(mIsLaunchWait)
            yield return null;

        //explanations and such

        //enable force input
        cannonForceInputUI.interactable = true;
                        
        //wait for proceed
        mIsNextWait = true;
        while(mIsNextWait)
            yield return null;

        //progress
        GameData.instance.Progress();
    }

    protected override void OnFinish() {
        //enable next
        nextInterfaceGO.SetActive(true);
    }

    private void SetInteractiveEnable(bool interact) {
        cannonForceInputUI.interactable = interact;
        cannonLaunchUI.interactable = interact;
    }
}