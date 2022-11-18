using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUpOrient : MonoBehaviour {
    public float ofs;
    public float scale = 1f;

    public void Apply(float a) {
        transform.up = M8.MathUtil.RotateAngle(Vector2.up, ofs + (a * scale));
    }
}
