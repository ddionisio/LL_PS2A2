using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCrossoutCounterItemWidget : MonoBehaviour {
    [Header("Root")]
    public Transform root;
    public Vector2 rootOfsMin;
    public Vector2 rootOfsMax;
    public float rootOfsRotMin;
    public float rootOfsRotMax;

    [Header("Cross")]
    public GameObject crossGO;

    void OnEnable() {
        root.localPosition = new Vector2(Mathf.Round(Random.Range(rootOfsMin.x, rootOfsMax.x)), Mathf.Round(Random.Range(rootOfsMin.y, rootOfsMax.y)));
        root.localEulerAngles = new Vector3(0f, 0f, Random.Range(rootOfsRotMin, rootOfsRotMax));

        crossGO.SetActive(false);
    }
}
