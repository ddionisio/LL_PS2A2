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
    public float[] itemBodyMasses;
    public Transform itemStartPoint;
    
    [Header("Sequence")]
    public ModalDialogController introDialog;
    public GameObject illustrateGO;
    public ModalDialogController illustrateDialog;
    public float treasureDialogStartDelay = 2f;
    public ModalDialogController treasureDialog;
    public string modalVictory;

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

        illustrateGO.gameObject.SetActive(false);

        //setup item
        itemBody.transform.position = itemStartPoint.position;
        itemBody.mass = itemBodyMasses[Random.Range(0, itemBodyMasses.Length)];
        itemBody.gameObject.SetActive(false);        
        //

        signalTreasureOpened.callback += OnSignalTreasureOpened;
        signalShowNext.callback += OnSignalShowNext;
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //intro
        yield return new WaitForSeconds(0.5f);

        introDialog.Play();
        while(introDialog.isPlaying)
            yield return null;

        //illustrate
        illustrateGO.SetActive(true);

        illustrateDialog.Play();
        while(illustrateDialog.isPlaying)
            yield return null;

        illustrateGO.SetActive(false);

        //show left side weight
        itemBody.gameObject.SetActive(true);

        yield return new WaitForSeconds(treasureDialogStartDelay);

        treasureDialog.Play();
        while(treasureDialog.isPlaying)
            yield return null;

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
        M8.UIModal.Manager.instance.ModalOpen(modalVictory);
    }
}
