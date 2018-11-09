using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountTweenWidget : MonoBehaviour {
    public Text label;
    public string labelFormat = "{0:G0}";

    public float startValue;
    public float endValue;
    public bool isWholeNumber = true;

    public DG.Tweening.Ease easeType;
    public float easeDelay = 3f;

    public void Play() {
        StopAllCoroutines();
        StartCoroutine(DoPlay());
    }

    void OnEnable() {
        ApplyValue(startValue);
    }

    private void ApplyValue(float val) {
        if(isWholeNumber)
            val = Mathf.Round(val);

        label.text = string.Format(labelFormat, val);
    }

    IEnumerator DoPlay() {
        var easeFunc = DG.Tweening.Core.Easing.EaseManager.ToEaseFunction(easeType);

        float curTime = 0f;
        while(curTime <= easeDelay) {
            curTime += Time.deltaTime;

            float t = easeFunc(curTime, easeDelay, 0f, 0f);

            var val = Mathf.Lerp(startValue, endValue, t);
            ApplyValue(val);

            yield return null;
        }
    }
}
