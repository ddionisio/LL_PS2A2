using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActController_2_1 : ActCannonController {
    
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

        //enable force input
        forceSlider.interactable = true;
    }

    protected override void OnFinish() {
        //victory
    }
}