using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnController : MonoBehaviour {
    [Header("Data")]
    public UnitEntity unit;
    public bool releaseAfterDespawn;

    [Header("States")]
    public M8.EntityState spawnState;
    public M8.EntityState spawnAfterState; //state after spawn finished

    public M8.EntityState despawnState;
    public M8.EntityState despawnAfterState; //state after despawn finish, if releaseAfterDespawn is false

    [Header("Animation")]
    public M8.Animator.Animate animator;
    public string takeSpawn;
    public string takeDespawn;

    private Coroutine mRout;

    void OnDestroy() {
        if(unit) {
            unit.setStateCallback -= OnEntityChangedState;
            unit.releaseCallback -= OnEntityRelease;
        }
    }

    void Awake() {
        if(!unit)
            unit = GetComponent<UnitEntity>();

        unit.setStateCallback += OnEntityChangedState;
        unit.releaseCallback += OnEntityRelease;
    }
        
    void OnEntityChangedState(M8.EntityBase ent) {
        StopRoutine();

        if(unit.state == spawnState) {
            unit.physicsEnabled = false;
            mRout = StartCoroutine(DoSpawn());
        }
        else if(unit.state == despawnState) {
            unit.physicsEnabled = false;
            mRout = StartCoroutine(DoDespawn());
        }
    }

    void OnEntityRelease(M8.EntityBase ent) {
        StopRoutine();
    }

    IEnumerator DoSpawn() {
        if(animator && !string.IsNullOrEmpty(takeSpawn)) {
            animator.Play(takeSpawn);
            while(animator.isPlaying)
                yield return null;
        }
        else
            yield return null;

        mRout = null;

        unit.state = spawnAfterState;
    }

    IEnumerator DoDespawn() {
        if(animator && !string.IsNullOrEmpty(takeDespawn)) {
            animator.Play(takeDespawn);
            while(animator.isPlaying)
                yield return null;
        }
        else
            yield return null;

        mRout = null;

        if(releaseAfterDespawn)
            unit.Release();
        else
            unit.state = despawnAfterState;
    }

    private void StopRoutine() {
        if(mRout != null) {
            StopCoroutine(mRout);
            mRout = null;
        }
    }
}
