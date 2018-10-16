using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActController_2_2 : ActCannonController {
    [Header("Sequence")]
    public GameObject header;

    protected override void OnInstanceDeinit() {
        //

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();
        
        //

    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //intro part

        cannonInterfaceGO.SetActive(true);

        //some other stuff?

        //enable cannon launch        
        cannonLaunch.interactable = true;

        //wait for launch
        mIsCannonballFree = true; //free shot
        mIsLaunchWait = true;
        while(mIsLaunchWait)
            yield return null;

        //explanations and such

        //enable force/angle input
        angleSlider.interactable = true;
        forceSlider.interactable = true;

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
}