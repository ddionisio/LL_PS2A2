using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LoLExt;

public class ActController_1_2 : GameModeController<ActController_1_2> {
    [Header("Data")]
    [M8.TagSelector]
    public string interactTag;
    [M8.TagSelector]
    public string tagDragGuide;
    public DragCursorWorld dragCursor;
    public GameObject dragActiveGO;

    //TODO: figure out randomization of left side object
    public Rigidbody2D itemBody;
    public float[] itemBodyMasses;
    public Transform itemStartPoint;
    public GameObject itemHintActiveGO;
    public Text itemHintLabel;
    public float itemHintShowDelay = 240f;
    
    [Header("Sequence")]
    public string musicPath;

    public ModalDialogController introDialog;
    public AnimatorEnterExit inertiaIllustrationAnim;
    public ModalDialogController inertiaDialog;
    public GameObject illustrateGO;
    public ModalDialogController illustrateDialog;
    public float treasureDialogStartDelay = 2f;
    public ModalDialogController treasureDialog;
    public GameObject dragWeightHelpGO;
    public Transform dragWeightStartAnchor;
    public Transform dragWeightEndAnchor;
    public string modalVictory;

    [Header("Signals")]
    public M8.Signal signalTreasureOpened;
    public M8.Signal signalShowNext;
    
    private DragRigidbody2D[] mDragBodies;

    private DragToGuideWidget mDragGuide;
    private bool mIsDragGuideShown;

    private Coroutine mItemHintRout;

    protected override void OnInstanceDeinit() {
        mItemHintRout = null;

        signalTreasureOpened.callback -= OnSignalTreasureOpened;
        signalShowNext.callback -= OnSignalShowNext;

        if(mIsDragGuideShown && mDragGuide)
            mDragGuide.Hide();

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

                dragBodyComp.dragBeginCallback += OnBodyDragBegin;
                dragBodyComp.dragEndCallback += OnBodyDragEnd;

                dragBodyList.Add(dragBodyComp);
            }
        }

        mDragBodies = dragBodyList.ToArray();

        dragCursor.gameObject.SetActive(false);
        dragActiveGO.SetActive(false);
        //

        inertiaIllustrationAnim.gameObject.SetActive(false);

        illustrateGO.gameObject.SetActive(false);

        //setup item
        itemBody.transform.position = itemStartPoint.position;
        itemBody.mass = itemBodyMasses[Random.Range(0, itemBodyMasses.Length)];
        itemBody.gameObject.SetActive(false);

        itemHintActiveGO.SetActive(false);
        itemHintLabel.text = Mathf.RoundToInt(itemBody.mass).ToString();
        //

        //drag instructs
        var dragGuideGO = GameObject.FindGameObjectWithTag(tagDragGuide);
        mDragGuide = dragGuideGO.GetComponent<DragToGuideWidget>();

        dragWeightHelpGO.SetActive(false);

        signalTreasureOpened.callback += OnSignalTreasureOpened;
        signalShowNext.callback += OnSignalShowNext;
    }

    protected override IEnumerator Start() {
        if(!string.IsNullOrEmpty(musicPath))
            LoLManager.instance.PlaySound(musicPath, true, true);

        yield return base.Start();

        //intro
        yield return new WaitForSeconds(0.5f);

        introDialog.Play();
        while(introDialog.isPlaying)
            yield return null;
        //

        //inertia
        inertiaIllustrationAnim.gameObject.SetActive(true);
        yield return inertiaIllustrationAnim.PlayEnterWait();

        inertiaDialog.Play();
        while(inertiaDialog.isPlaying)
            yield return null;

        yield return inertiaIllustrationAnim.PlayExitWait();
        inertiaIllustrationAnim.gameObject.SetActive(false);
        //

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
        dragWeightHelpGO.SetActive(true);

        if(mDragGuide) {
            var cam = Camera.main;
            Vector2 sPos = cam.WorldToScreenPoint(dragWeightStartAnchor.position);
            Vector2 ePos = cam.WorldToScreenPoint(dragWeightEndAnchor.position);

            mDragGuide.Show(false, sPos, ePos);
            mIsDragGuideShown = true;
        }

        mItemHintRout = StartCoroutine(DoShowHint());

        //SetInteractiveEnabled(false);

        //GameData.instance.Progress();
    }

    private void SetInteractiveEnabled(bool aEnabled) {
        for(int i = 0; i < mDragBodies.Length; i++) {
            mDragBodies[i].isDragEnabled = aEnabled;
        }
    }

    IEnumerator DoShowHint() {
        yield return new WaitForSeconds(itemHintShowDelay);

        itemHintActiveGO.SetActive(true);

        mItemHintRout = null;
    }

    void OnSignalTreasureOpened() {
        if(mItemHintRout != null) {
            StopCoroutine(mItemHintRout);
            mItemHintRout = null;
        }

        SetInteractiveEnabled(false);
    }

    void OnSignalShowNext() {
        M8.UIModal.Manager.instance.ModalOpen(modalVictory);
    }

    void OnBodyDragBegin() {
        dragActiveGO.SetActive(true);
    }

    void OnBodyDragEnd() {
        if(mIsDragGuideShown) {
            mDragGuide.Hide();
            mIsDragGuideShown = false;
        }

        dragActiveGO.SetActive(false);
    }
}
