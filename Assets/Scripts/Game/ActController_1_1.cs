using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActController_1_1 : GameModeController<ActController_1_1> {
    [Header("Widgets")]
    [M8.TagSelector]
    public string tagDragGuide;
    public Transform dragGuideTo;

    [Header("Sequence")]
    public float startDelay = 1f;

    public CameraShakeControl cameraShaker;

    public AnimatorEnterExit titleAnim;
    public AnimatorEnterExit motionIllustrationAnim;
    public ModalDialogController introDialog;    
    public ModalDialogController landingDialog;
    public GameObject sumForceIllustrationGO;
    public ModalDialogController sumForceDialog;
    public ModalDialogController beginGameDialog;

    public GameObject interfaceRootGO;

    public DragWidget unitSpawnerWidget;

    public Rigidbody2DMoveController block1;
    public Transform block1StartPt;
    [M8.TagSelector]
    public string block1GoalTag;

    public ModalDialogController inertiaDialog1;

    public AnimatorEnterExit inertiaIllustrationAnim;

    public ModalDialogController inertiaDialog2;

    public Rigidbody2DMoveController block2;
    public Transform block2StartPt;

    public GameObject victoryGO;
    public float victoryStartDelay = 0.5f;
    public float victoryDelay; //delay to show victory modal

    public string victoryModal = "victory_act_1_1";

    [Header("Princess")]
    public GameObject princessGO;
    public M8.Animator.Animate princessAnim;
    [M8.Animator.TakeSelector(animatorField = "princessAnim")]
    public string princessTakeEnter;
    [M8.Animator.TakeSelector(animatorField = "princessAnim")]
    public string princessTakeHelp;

    [Header("Signals")]
    public M8.Signal signalUnitsClear;
    public SignalDragWidget signalUnitDragEnd;

    private DragToGuideWidget mDragGuide;
    private bool mIsDragGuideShown;

    protected override void OnInstanceDeinit() {
        signalUnitDragEnd.callback -= OnSignalUnitDragEnd;

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        var dragGuideGO = GameObject.FindGameObjectWithTag(tagDragGuide);
        mDragGuide = dragGuideGO.GetComponent<DragToGuideWidget>();

        titleAnim.gameObject.SetActive(false);
        motionIllustrationAnim.gameObject.SetActive(false);
        inertiaIllustrationAnim.gameObject.SetActive(false);

        sumForceIllustrationGO.SetActive(false);

        interfaceRootGO.SetActive(false);

        princessGO.SetActive(false);

        victoryGO.SetActive(false);
        
        block1.transform.position = block1StartPt.position;

        block2.transform.position = block2StartPt.position;

        signalUnitDragEnd.callback += OnSignalUnitDragEnd;
    }

    protected override IEnumerator Start() {        
        yield return base.Start();

        titleAnim.gameObject.SetActive(true);
        yield return titleAnim.PlayEnterWait();

        yield return new WaitForSeconds(startDelay);

        motionIllustrationAnim.gameObject.SetActive(true);
        yield return motionIllustrationAnim.PlayEnterWait();

        introDialog.Play();
        while(introDialog.isPlaying)
            yield return null;

        yield return titleAnim.PlayExitWait();
        titleAnim.gameObject.SetActive(false);

        yield return motionIllustrationAnim.PlayExitWait();
        motionIllustrationAnim.gameObject.SetActive(false);

        //get block 1 ready
        block1.body.simulated = true;

        //wait for block 1 to hit ground
        while(!block1.isGrounded)
            yield return null;

        //fancy camera shake
        cameraShaker.Shake();

        //some more dialog
        landingDialog.Play();
        while(landingDialog.isPlaying)
            yield return null;

        sumForceIllustrationGO.SetActive(true);

        sumForceDialog.Play();
        while(sumForceDialog.isPlaying)
            yield return null;

        sumForceIllustrationGO.SetActive(false);

        //princess
        StartCoroutine(DoPrincessDistress());

        //some more dialog
        beginGameDialog.Play();
        while(beginGameDialog.isPlaying)
            yield return null;

        //show interaction
        interfaceRootGO.SetActive(true);

        //game ready
        unitSpawnerWidget.active = true;

        //drag instruction
        yield return new WaitForSeconds(0.35f);
        mDragGuide.Show(false, unitSpawnerWidget.icon.transform.position, dragGuideTo.position);
        mIsDragGuideShown = true;

        //wait for block1 to contact goal
        bool isBlockFinish = false;

        while(!isBlockFinish) {
            var blockContacts = block1.collisionData;
            for(int i = 0; i < blockContacts.Count; i++) {
                var coll = blockContacts[i].collider;
                if(coll && coll.CompareTag(block1GoalTag)) {
                    isBlockFinish = true;
                    break;
                }
            }

            yield return null;
        }

        unitSpawnerWidget.active = false;

        //animation

        //clear out units
        signalUnitsClear.Invoke();

        //more dialog
        yield return new WaitForSeconds(0.5f);
                
        inertiaDialog1.Play();
        while(inertiaDialog1.isPlaying)
            yield return null;

        inertiaIllustrationAnim.gameObject.SetActive(true);
        yield return inertiaIllustrationAnim.PlayEnterWait();

        inertiaDialog2.Play();
        while(inertiaDialog2.isPlaying)
            yield return null;
                
        yield return inertiaIllustrationAnim.PlayExitWait();
        inertiaIllustrationAnim.gameObject.SetActive(false);
        //

        //block 2 ready
        block2.body.simulated = true;

        //wait for block 2 to hit ground
        while(!block2.isGrounded)
            yield return null;

        //fancy camera shake
        cameraShaker.Shake();

        //game ready
        unitSpawnerWidget.active = true;

        //wait for block2 to contact block 1
        isBlockFinish = false;

        while(!isBlockFinish) {
            var blockContacts = block2.collisionData;
            for(int i = 0; i < blockContacts.Count; i++) {
                var coll = blockContacts[i].collider;
                if(coll == block1.coll) {
                    isBlockFinish = true;
                    break;
                }
            }

            yield return null;
        }

        unitSpawnerWidget.active = false;

        //clear out units
        signalUnitsClear.Invoke();

        //victory
        yield return new WaitForSeconds(victoryStartDelay);

        victoryGO.SetActive(true);

        yield return new WaitForSeconds(victoryDelay);

        //next level
        //GameData.instance.Progress();
        M8.UIModal.Manager.instance.ModalCloseAll();
        M8.UIModal.Manager.instance.ModalOpen(victoryModal);
    }

    IEnumerator DoPrincessDistress() {
        princessGO.SetActive(true);

        princessAnim.Play(princessTakeEnter);

        while(princessAnim.isPlaying)
            yield return null;

        princessAnim.Play(princessTakeHelp);
    }

    void OnSignalUnitDragEnd(DragWidgetSignalInfo inf) {
        if(inf.dragWidget.isDropValid) {
            if(mIsDragGuideShown) {
                mDragGuide.Hide();
                mIsDragGuideShown = false;
            }
        }
    }
}
