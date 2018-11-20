using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActController_2_2 : ActCannonController {
    [Header("Cannon")]
    public M8.Animator.Animate cannonEnterAnimator;
    [M8.Animator.TakeSelector(animatorField = "cannonEnterAnimator")]
    public string cannonEnterTake;

    public GameObject launchReadyGO;
    public M8.Animator.Animate cannonAnimator;
    [M8.Animator.TakeSelector(animatorField = "cannonAnimator")]
    public string cannonTakeLaunch;

    public GameObject cannonAngleDragHelpGO;
    public GameObject graphReminderGO;

    [Header("Sequence")]
    public ModalDialogController seqDlgIntro;
    public ModalDialogController seqDlgPlay;

    private bool mIsAngleChanged;
    private bool mIsShowGraphReminder;

    protected override void OnInstanceDeinit() {
        //

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();
                
        //
        launchReadyGO.SetActive(false);

        cannonEnterAnimator.ResetTake(cannonEnterTake);

        cannonAngleDragHelpGO.SetActive(false);
        graphReminderGO.SetActive(false);
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //show targets
        ShowTargets();
        yield return new WaitForSeconds(1.5f);

        //intro part
        seqDlgIntro.Play();
        while(seqDlgIntro.isPlaying)
            yield return null;

        cannonEnterAnimator.Play(cannonEnterTake);
        while(cannonEnterAnimator.isPlaying)
            yield return null;
        cannonEnterAnimator.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.0f);

        seqDlgPlay.Play();
        while(seqDlgPlay.isPlaying)
            yield return null;

        yield return new WaitForSeconds(1.0f);

        //everything ready
        cannonInterfaceGO.SetActive(true);

        //enable force/angle input
        angleSlider.interactable = true;
        forceSlider.interactable = true;

        //enable cannon launch
        launchReadyGO.SetActive(true);
        cannonLaunch.interactable = true;

        if(trajectoryDisplayControl)
            trajectoryDisplayControl.show = true;
        //

        //first time shows
        mIsAngleChanged = false;
        cannonAngleDragHelpGO.SetActive(true);

        mIsShowGraphReminder = true;
        //

        //wait for launch
        mIsLaunchWait = true;
        while(mIsLaunchWait)
            yield return null;

        //wait for cannon launch ready
        while(!cannonLaunch.interactable)
            yield return null;

        //remind about the graph
    }

    protected override void OnLaunched() {
        base.OnLaunched();

        if(!mIsAngleChanged) {
            cannonAngleDragHelpGO.SetActive(false);
            mIsAngleChanged = true;
        }

        graphReminderGO.SetActive(false);

        launchReadyGO.SetActive(false);
        cannonLaunch.interactable = false;

        cannonAnimator.Play(cannonTakeLaunch);

        var ent = cannonballSpawner.Spawn();
        StartCoroutine(DoLaunch(ent));
    }

    protected override void OnAngleChanged(float val) {
        base.OnAngleChanged(val);

        if(!mIsAngleChanged) {            
            cannonAngleDragHelpGO.SetActive(false);
            mIsAngleChanged = true;
        }
    }

    protected override void OnShowGraph() {
        base.OnShowGraph();

        graphReminderGO.SetActive(false);
        mIsShowGraphReminder = false;
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

        launchReadyGO.SetActive(true);
        cannonLaunch.interactable = true;

        if(mIsShowGraphReminder) {
            graphReminderGO.SetActive(true);
            mIsShowGraphReminder = false;
        }
    }
}