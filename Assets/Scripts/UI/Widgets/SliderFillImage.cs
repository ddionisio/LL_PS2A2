using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderFillImage : MonoBehaviour {
    public Slider slider;
    public Image fillImage;

    void OnEnable() {
        RefreshFillImage();
    }

    void Awake() {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float val) {
        RefreshFillImage();
    }

    private void RefreshFillImage() {
        fillImage.fillAmount = slider.normalizedValue;
    }
}
