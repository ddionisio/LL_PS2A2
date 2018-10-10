using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActForceBalanceController : GameModeController<ActForceBalanceController> {
    [Header("Data")]
    [M8.TagSelector]
    public string interactTag;
    public DragCursorWorld dragCursor;

    [Header("Sequence")]
    public GameObject header;
    public GameObject question;

    [Header("Signal")]
    public SignalFloat signalNumberProceed;
    public M8.Signal signalProceed;

    private DragRigidbody2D[] mDragBodies;

    private bool mIsProceedWait;
    private float mProceedNumber;

    protected override void OnInstanceDeinit() {
        signalNumberProceed.callback -= OnSignalNumberProceed;
        signalProceed.callback -= OnSignalProceed;

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        header.SetActive(false);
        question.SetActive(false);

        //setup interactives
        var interactGOs = GameObject.FindGameObjectsWithTag(interactTag);

        //grab dragable bodies and initialize
        var dragBodyList = new List<DragRigidbody2D>();
        for(int i = 0; i < interactGOs.Length; i++) {
            var dragBodyComp = interactGOs[i].GetComponent<DragRigidbody2D>();
            if(dragBodyComp) {
                dragBodyComp.SetDragCursor(dragCursor);
                dragBodyList.Add(dragBodyComp);
            }
        }

        mDragBodies = dragBodyList.ToArray();

        dragCursor.gameObject.SetActive(false);
        //

        signalNumberProceed.callback += OnSignalNumberProceed;
        signalProceed.callback += OnSignalProceed;
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //intro

        //setup left side weight

        //enable play
        SetInteractiveEnabled(true);

        //drag instruction

        //show question
        question.SetActive(true);

        //wait for button proceed
        mIsProceedWait = true;
        while(mIsProceedWait)
            yield return null;

        SetInteractiveEnabled(false);

        //check answer
        Debug.Log("Answer: " + mProceedNumber);

        //show result

        //wait for button proceed
        mIsProceedWait = true;
        while(mIsProceedWait)
            yield return null;

        GameData.instance.Progress();
    }

    void OnSignalNumberProceed(float val) {
        mProceedNumber = val;
        mIsProceedWait = false;
    }

    void OnSignalProceed() {
        mIsProceedWait = false;
    }

    private void SetInteractiveEnabled(bool aEnabled) {
        for(int i = 0; i < mDragBodies.Length; i++) {
            mDragBodies[i].isDragEnabled = aEnabled;
        }
    }
}
