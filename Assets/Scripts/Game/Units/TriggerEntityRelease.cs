using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEntityRelease : MonoBehaviour {
    [M8.TagSelector]
    public string tagFilter;

    void OnTriggerEnter2D(Collider2D collision) {
        if(!string.IsNullOrEmpty(tagFilter) && !collision.CompareTag(tagFilter))
            return;

        var ent = collision.GetComponent<M8.EntityBase>();
        if(ent)
            ent.Release();
    }
}
