using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateGOActive : MonoBehaviour {
    [System.Serializable]
    public struct Data {
        public M8.EntityState state;
        public GameObject go;

        public void Hide() {
            if(go) go.SetActive(false);
        }

        public void Apply(M8.EntityState curState) {
            if(go) go.SetActive(curState == state);
        }
    }

    public M8.EntityBase entity;
    public Data[] data;

    void OnDestroy() {
        if(entity) {
            entity.spawnCallback -= OnEntitySpawned;
            entity.setStateCallback -= OnEntityStateChanged;
        }
    }

    void Awake() {
        if(!entity)
            entity = GetComponent<M8.EntityBase>();

        entity.spawnCallback += OnEntitySpawned;
        entity.setStateCallback += OnEntityStateChanged;
    }

    void OnEntitySpawned(M8.EntityBase ent) {
        for(int i = 0; i < data.Length; i++)
            data[i].Hide();
    }

    void OnEntityStateChanged(M8.EntityBase ent) {
        for(int i = 0; i < data.Length; i++)
            data[i].Apply(ent.state);
    }
}
