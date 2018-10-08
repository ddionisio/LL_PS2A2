using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActInertiaController : GameModeController<ActInertiaController> {
    [Header("Sequence")]
    public GameObject headerRootGO;

    public float startDelay = 1f;

    public ModalDialogController introDialog;

    public GameObject interfaceRootGO;

    public DragWidget unitSpawnerWidget;

    public Rigidbody2DMoveController block1;
    [M8.TagSelector]
    public string block1GoalTag;

    public Rigidbody2DMoveController block2;

    [Header("Signals")]
    public M8.Signal signalUnitsClear;

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        headerRootGO.SetActive(false);

        interfaceRootGO.SetActive(false);
    }

    protected override IEnumerator Start() {        
        yield return base.Start();

        headerRootGO.SetActive(true);

        yield return new WaitForSeconds(startDelay);

        //describe inertia
        introDialog.Play();
        while(introDialog.isPlaying)
            yield return null;

        //show interaction
        interfaceRootGO.SetActive(true);

        //get block 1 ready
        block1.body.simulated = true;

        //wait for block 1 to hit ground
        while(!block1.isGrounded)
            yield return null;

        //fancy camera shake

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

        //animation

        //clear out units
        signalUnitsClear.Invoke();

        //more stuff

        //next level
        GameData.instance.Progress();
    }
}
