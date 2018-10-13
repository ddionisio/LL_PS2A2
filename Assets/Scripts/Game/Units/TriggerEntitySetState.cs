using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEntitySetState : MonoBehaviour {
    [M8.TagSelector]
    public string tagFilter;
    public M8.EntityState state;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(!string.IsNullOrEmpty(tagFilter) && !collision.CompareTag(tagFilter))
            return;

        var ent = collision.GetComponent<M8.EntityBase>();
        if(ent)
            ent.state = state;
    }
}
