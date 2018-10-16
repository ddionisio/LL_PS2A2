using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTrajectoryDisplay : MonoBehaviour {    
    [Header("Display")]
    public GameObject template;
    public Transform templateRoot; //where to place displays
    public int capacity = 50;
    public float timeInterval = 0.1f;

    [Header("Collision")]
    public float collisionRadius;

    public LayerMask collisionLayerMask; //for grounds and wall
    public bool collisionEndOnContact; //end trajectory on any contact?
    [M8.TagSelector]
    public string collisionEndTagFilter; //for ending the trajectory

    private M8.CacheList<GameObject> mDisplayActives;
    private M8.CacheList<GameObject> mDisplayInactives;

    public void Clear() {
        while(mDisplayActives.Count > 0) {
            var go = mDisplayActives.RemoveLast();
            go.SetActive(false);
            mDisplayInactives.Add(go);
        }
    }

    public void Evaluate(float mass, Vector2 force, float forceDuration, float duration) {
        Clear();


    }

    void OnDisable() {
        Clear();
    }

    void Awake() {
        mDisplayActives = new M8.CacheList<GameObject>(capacity);
        mDisplayInactives = new M8.CacheList<GameObject>(capacity);

        for(int i = 0; i < capacity; i++) {
            var newGO = Object.Instantiate<GameObject>(template);
            newGO.transform.SetParent(templateRoot);
            newGO.SetActive(false);

            mDisplayInactives.Add(newGO);
        }
    }
}
