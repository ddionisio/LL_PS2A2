using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LoLExt;

public class ActController_2_2 : ActCannonController {
    [Header("Cannon")]
    public M8.Animator.Animate cannonEnterAnimator;
    [M8.Animator.TakeSelector(animatorField = "cannonEnterAnimator")]
    public string cannonEnterTake;

    public GameObject launchReadyGO;
    public M8.Animator.Animate cannonAnimator;
    [M8.Animator.TakeSelector(animatorField = "cannonAnimator")]
    public string cannonTakeLaunch;
    public string cannonLaunchSfxPath;

    public GameObject cannonAngleDragHelpGO;
    public GameObject cannonForceHelpGO;
    public GameObject cannonLaunchHelpGO;

    public GameObject graphReminderGO;

    public CameraShakeControl cameraShaker;

    [Header("Sequence")]
    public ModalDialogController seqDlgIntro;
    public ModalDialogController seqDlgPlay;

    private bool mIsHintFinish;
    private bool mIsShowGraphReminder;

    private float mCurAngle = 0f;
    private float mCurForce = 0f;

    private const float angleHint = 70f;
    private const float forceHint = 310f;

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
        cannonForceHelpGO.SetActive(false);
        cannonLaunchHelpGO.SetActive(false);

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

        //enable cannon launch
        launchReadyGO.SetActive(true);

        if(trajectoryDisplayControl)
            trajectoryDisplayControl.show = true;
        //

        //forceSlider.interactable = true;
        //cannonLaunch.interactable = true;

        //first time shows
        mIsHintFinish = false;

        //wait for correct angle
        cannonAngleDragHelpGO.SetActive(true);
        angleSlider.interactable = true;

        while(mCurAngle != angleHint)
            yield return null;

        //wait for correct force
        cannonForceHelpGO.SetActive(true);
        forceSlider.interactable = true;

        while(mCurForce != forceHint)
            yield return null;

        //ready to launch
        cannonLaunchHelpGO.SetActive(true);
        //cannonLaunch.interactable = true;

        //remind about the graph
        mIsShowGraphReminder = true;

        //wait for launch
        mIsLaunchWait = true;
        while(mIsLaunchWait) {
            cannonLaunch.interactable = mCurAngle == angleHint && mCurForce == forceHint;
            yield return null;
        }

        //wait for cannon launch ready
        while(!cannonLaunch.interactable)
            yield return null;
    }

    protected override void OnLaunched() {
        base.OnLaunched();

        if(mIsLaunchWait)
            mIsLaunchWait = false;

        if(!mIsHintFinish) {
            cannonAngleDragHelpGO.SetActive(false);
            cannonForceHelpGO.SetActive(false);
            cannonLaunchHelpGO.SetActive(false);
            mIsHintFinish = true;
        }

        graphReminderGO.SetActive(false);

        launchReadyGO.SetActive(false);
        cannonLaunch.interactable = false;

        cannonAnimator.Play(cannonTakeLaunch);

        if(!string.IsNullOrEmpty(cannonLaunchSfxPath))
            LoLManager.instance.PlaySound(cannonLaunchSfxPath, false, false);

        var ent = SpawnCannonball();
        StartCoroutine(DoLaunch(ent));
    }

    protected override void OnAngleChanged(float val) {
        if(!mIsHintFinish && val > angleHint) {
            angleSlider.value = angleHint;
            return;
        }

        base.OnAngleChanged(val);

        mCurAngle = val;
    }

    protected override void OnForceValueChanged(float val) {
        if(!mIsHintFinish && val > forceHint) {
            forceSlider.normalizedValue = (forceHint - forceMin) / (forceMax - forceMin);
            return;
        }

        base.OnForceValueChanged(val);

        mCurForce = val;
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

        //wait for cannon to end
        while(unitForceApply.unit.physicsEnabled)
            yield return null;

        cameraShaker.Shake();

        graphControl.tracer.Stop();

        GraphPopulate(true);

        launchReadyGO.SetActive(true);
        cannonLaunch.interactable = true;

        if(mIsShowGraphReminder) {
            graphReminderGO.SetActive(true);
            mIsShowGraphReminder = false;
        }
    }
}