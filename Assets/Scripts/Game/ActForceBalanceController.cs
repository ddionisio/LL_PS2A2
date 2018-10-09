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

    [Header("Signal")]
    public M8.Signal signalProceed;

    private DragRigidbody2D[] mDragBodies;

    private bool mIsProceedWait;

    protected override void OnInstanceDeinit() {
        signalProceed.callback -= OnSignalProceed;

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

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

        signalProceed.callback += OnSignalProceed;
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //intro

        //setup left side weight

        //enable play
        SetInteractiveEnabled(true);

        //drag instruction

        //wait for button proceed
        mIsProceedWait = true;
        while(mIsProceedWait)
            yield return null;

        SetInteractiveEnabled(false);

        //check answer

        //show result

        //wait for button proceed
        mIsProceedWait = true;
        while(mIsProceedWait)
            yield return null;

        GameData.instance.Progress();
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
