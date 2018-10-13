using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawnToStateDelay : MonoBehaviour, M8.IPoolSpawn {
    public M8.EntityBase entity;
    public M8.EntityState state;
    public float delay;

    void Awake() {
        if(!entity)
            entity = GetComponent<M8.EntityBase>();
    }

    void M8.IPoolSpawn.OnSpawned(M8.GenericParams parms) {
        StartCoroutine(DoToState());
    }

    IEnumerator DoToState() {
        yield return new WaitForSeconds(delay);

        entity.state = state;
    }
}
