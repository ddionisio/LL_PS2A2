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

        //show targets
        ShowTargets();

        if(trajectoryDisplayControl)
            trajectoryDisplayControl.show = true;

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

        
    }

    protected override void OnLaunched() {
        base.OnLaunched();

        cannonLaunch.interactable = false;

        var ent = cannonballSpawner.Spawn();
        StartCoroutine(DoLaunch(ent));
    }

    IEnumerator DoLaunch(M8.EntityBase cannonEnt) {
        var unitForceApply = cannonEnt.GetComponent<UnitStateForceApply>();

        while(!unitForceApply.unit.physicsEnabled)
            yield return null;

        graphControl.tracer.body = unitForceApply.unit.body;
        graphControl.tracer.Record();

        //wait for tracer to finish
        while(graphControl.tracer.isRecording)
            yield return null;

        GraphPopulate(true);

        cannonLaunch.interactable = true;
    }
}