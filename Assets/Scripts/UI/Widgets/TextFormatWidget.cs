using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFormatWidget : MonoBehaviour {
    [Header("Display")]
    public Text textLabel;
    public string stringFormat;

    public void SetText(int i) {
        textLabel.text = string.Format(stringFormat, i);
    }

    public void SetText(float f) {
        textLabel.text = string.Format(stringFormat, f);
    }

    public void SetText(string text) {
        textLabel.text = string.Format(stringFormat, text);
    }
}
