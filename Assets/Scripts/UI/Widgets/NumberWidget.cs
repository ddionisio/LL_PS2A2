using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberWidget : MonoBehaviour {
    public Text label;
    public string format = "{0}";

    public float number {
        get { return mNumber; }
        set {
            if(mNumber != value) {
                mNumber = value;
                if(label)
                    label.text = string.Format(format, mNumber);
            }
        }
    }

    private float mNumber = 0f;

    void Awake() {
        if(!label)
            label = GetComponent<Text>();
    }
}
