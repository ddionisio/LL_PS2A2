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

    [Header("Signals")]
    public M8.Signal signalTreasureOpened;
    public M8.Signal signalShowNext;
    
    private DragRigidbody2D[] mDragBodies;
        
    protected override void OnInstanceDeinit() {
        signalTreasureOpened.callback -= OnSignalTreasureOpened;
        signalShowNext.callback -= OnSignalShowNext;

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        header.SetActive(false);

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
        
        itemBody.gameObject.SetActive(false);
        //

        signalTreasureOpened.callback += OnSignalTreasureOpened;
        signalShowNext.callback += OnSignalShowNext;
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //intro

        //show left side weight
        itemBody.gameObject.SetActive(true);

        //enable play
        SetInteractiveEnabled(true);

        //drag instruction
        
        //SetInteractiveEnabled(false);

        //GameData.instance.Progress();
    }
    
    private void SetInteractiveEnabled(bool aEnabled) {
        for(int i = 0; i < mDragBodies.Length; i++) {
            mDragBodies[i].isDragEnabled = aEnabled;
        }
    }

    void OnSignalTreasureOpened() {
        SetInteractiveEnabled(false);
    }

    void OnSignalShowNext() {
        Debug.Log("NEXT");
    }
}
