using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateOnContact : MonoBehaviour {
    public M8.EntityBase entity;
    [M8.TagSelector]
    public string tagFilter;
    public M8.EntityState state;
    public int counter = 0; //number of times before state is activated, set to <= 0 for indefinite state change

    private int mCurCounter;

    void OnCollisionEnter2D(Collision2D collision) {
        if(mCurCounter > 0 && mCurCounter >= counter)
            return;

        if(!string.IsNullOrEmpty(tagFilter) && !collision.collider.CompareTag(tagFilter))
            return;

        mCurCounter++;

        if(mCurCounter >= counter)
            entity.state = state;
    }

    void OnEnable() {
        mCurCounter = 0;
    }

    void Awake() {
        if(!entity)
            entity = GetComponent<M8.EntityBase>();
    }
}