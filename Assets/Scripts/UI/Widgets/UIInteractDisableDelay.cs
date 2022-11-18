using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Disable interactive for a duration, then enables
/// </summary>
public class UIInteractDisableDelay : MonoBehaviour {
    public Selectable target;
    public float delay;

    public void Invoke() {
        StartCoroutine(DoDisable());
    }

    void Awake() {
        if(!target)
            target = GetComponent<Selectable>();
    }
        
    IEnumerator DoDisable() {
        target.interactable = false;

        yield return new WaitForSeconds(delay);

        target.interactable = true;
    }
}
