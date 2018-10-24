using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySignalRelease : MonoBehaviour, M8.IPoolSpawn, M8.IPoolDespawn {
    public M8.EntityBase entity;
    public M8.Signal signal;

    void M8.IPoolSpawn.OnSpawned(M8.GenericParams parms) {
        signal.callback += entity.Release;
    }

    void M8.IPoolDespawn.OnDespawned() {
        signal.callback -= entity.Release;
    }

    void Awake() {
        if(!entity)
            entity = GetComponent<M8.EntityBase>();
    }
}
