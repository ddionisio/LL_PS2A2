using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActController_1_1 : GameModeController<ActController_1_1> {
    [Header("Sequence")]
    public GameObject headerRootGO;

    public float startDelay = 1f;

    public ModalDialogController introDialog;

    public GameObject interfaceRootGO;

    public DragWidget unitSpawnerWidget;

    public Rigidbody2DMoveController block1;
    public Transform block1StartPt;
    [M8.TagSelector]
    public string block1GoalTag;

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

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        headerRootGO.SetActive(false);

        interfaceRootGO.SetActive(false);

        princessGO.SetActive(false);

        victoryGO.SetActive(false);

        block1.transform.position = block1StartPt.position;

        block2.transform.position = block2StartPt.position;
    }

    protected override IEnumerator Start() {        
        yield return base.Start();

        headerRootGO.SetActive(true);

        yield return new WaitForSeconds(startDelay);

        //describe inertia
        introDialog.Play();
        while(introDialog.isPlaying)
            yield return null;
                
        //get block 1 ready
        block1.body.simulated = true;

        //wait for block 1 to hit ground
        while(!block1.isGrounded)
            yield return null;

        //fancy camera shake

        //some more dialog

        //princess
        StartCoroutine(DoPrincessDistress());

        //show interaction
        interfaceRootGO.SetActive(true);

        //game ready
        unitSpawnerWidget.active = true;
        
        //drag instruction

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

        //block 2 ready
        block2.body.simulated = true;

        //wait for block 2 to hit ground
        while(!block2.isGrounded)
            yield return null;

        //fancy camera shake

        //more dialog

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
        M8.UIModal.Manager.instance.ModalOpen(victoryModal);
    }

    IEnumerator DoPrincessDistress() {
        princessGO.SetActive(true);

        princessAnim.Play(princessTakeEnter);

        while(princessAnim.isPlaying)
            yield return null;

        princessAnim.Play(princessTakeHelp);
    }
}
