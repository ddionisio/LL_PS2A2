using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateOnContact : MonoBehaviour {
    public M8.EntityBase entity;
    [M8.TagSelector]
    public string tagFilter;
    public M8.EntityState state;
    public bool onlyOnce;

    private bool mIsContacted;

    void OnCollisionEnter2D(Collision2D collision) {
        if(onlyOnce && mIsContacted)
            return;

        if(!string.IsNullOrEmpty(tagFilter) && !collision.collider.CompareTag(tagFilter))
            return;

        entity.state = state;
    }

    void OnDisable() {
        mIsContacted = false;
    }

    void Awake() {
        if(!entity)
            entity = GetComponent<M8.EntityBase>();
    }
}