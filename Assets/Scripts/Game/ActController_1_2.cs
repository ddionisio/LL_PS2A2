using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActController_1_2 : GameModeController<ActController_1_2> {
    [Header("Data")]
    [M8.TagSelector]
    public string interactTag;
    public DragCursorWorld dragCursor;

    //TODO: figure out randomization of left side object
    public Rigidbody2D itemBody;

    [Header("Display")]
    public Text resultAnswerLabel;
    public Text resultInputLabel;

    [Header("Sequence")]
    public GameObject header;
    public GameObject question;
    public GameObject result;

    [Header("Signal")]
    public SignalFloat signalNumberProceed;
    public M8.Signal signalProceed;

    private DragRigidbody2D[] mDragBodies;

    private bool mIsProceedWait;
    private float mProceedNumber;
    private float mAnswerNumber;
    
    protected override void OnInstanceDeinit() {
        signalNumberProceed.callback -= OnSignalNumberProceed;
        signalProceed.callback -= OnSignalProceed;

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        header.SetActive(false);
        question.SetActive(false);
        result.SetActive(false);

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

        //setup left side weight
        //determine what item to show, what the weight is
        
        //apply answer
        mAnswerNumber = itemBody.mass;
        resultAnswerLabel.text = mAnswerNumber.ToString();

        itemBody.gameObject.SetActive(false);
        //

        signalNumberProceed.callback += OnSignalNumberProceed;
        signalProceed.callback += OnSignalProceed;
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //intro

        //show left side weight
        itemBody.gameObject.SetActive(true);

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

        //apply input
        resultInputLabel.text = mProceedNumber.ToString();

        //show result
        result.SetActive(true);

        //other stuff

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
