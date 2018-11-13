using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActController_2_1 : ActCannonController {

    [Header("Cannon Control")]
    public M8.Animator.Animate cannonAnimator;
    [M8.Animator.TakeSelector(animatorField = "cannonAnimator")]
    public string cannonTakeEnter;

    public ProjectileEntitySpawner cannonballSpawner;

    public Transform knightRoot;
    public SpriteRenderer knightSpriteRender;
    public GameObject knightWheelGO;    
    public M8.Animator.Animate knightAnimator;
    [M8.Animator.TakeSelector(animatorField = "knightAnimator")]
    public string knightTakePush;
    [M8.Animator.TakeSelector(animatorField = "knightAnimator")]
    public string knightTakeMove;
    public float knightReturnDelay;
    public Transform knightReturnPoint;

    protected override void OnInstanceDeinit() {
        //
        if(cannonballSpawner)
            cannonballSpawner.spawnCallback -= OnCannonballSpawn;

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        //
        cannonballSpawner.spawnCallback += OnCannonballSpawn;
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //intro part

        cannonInterfaceGO.SetActive(true);

        //some other stuff?
        
        //enable cannon launch
        cannonLaunch.interactable = true;

        //wait for launch
        mIsCannonballFree = true; //free shot
        mIsLaunchWait = true;
        while(mIsLaunchWait)
            yield return null;

        //explanations and such

        //show targets
        ShowTargets();

        //enable force input
        forceSlider.interactable = true;
    }

    protected override void OnFinish() {
        //victory
    }

    void OnCannonballSpawn(M8.EntityBase ent) {

    }

    IEnumerator DoKnightReturn() {
        yield return null;
    }
}