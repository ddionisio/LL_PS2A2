using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySticky : MonoBehaviour {
    public struct AttachData {
        public UnitEntity ent;
        public Transform lastParent;
    }

    public UnitEntity ownerEntity;

    [M8.TagSelector]
    public string tagFilter;
    public Transform holder;

    public int capacity = 10;

    private M8.CacheList<AttachData> mAttached;

    void Awake() {
        mAttached = new M8.CacheList<AttachData>(capacity);

        if(!ownerEntity)
            ownerEntity = GetComponentInParent<UnitEntity>();

        ownerEntity.despawnEvent.AddListener(OnDespawn);
    }

    void OnDespawn() {
        //detach everything
        for(int i = 0; i < mAttached.Count; i++) {
            var dat = mAttached[i];
            dat.ent.transform.SetParent(dat.lastParent, true);
            dat.ent.physicsEnabled = true;
        }

        mAttached.Clear();
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(mAttached.IsFull)
            return;

        //attach unit
        if(collision.CompareTag(tagFilter)) {
            var ent = collision.GetComponent<UnitEntity>();
            if(ent) {
                ent.physicsEnabled = false;

                mAttached.Add(new AttachData { ent = ent, lastParent = ent.transform.parent });

                ent.transform.SetParent(holder, true);
            }
        }
    }
}
