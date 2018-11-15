using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphBarWidget : MonoBehaviour {
    [Header("Templates")]
    public RectTransform boxTemplate; //pivot-y is set based on the zero line
    public Text boxLabelTemplate; //set as child of box

    public Text labelHorizontalTemplate;
    public Text labelVerticalTemplate;
    public string labelFormat = "{0}";

    [Header("Display")]
    public RectTransform barHorizontal; //add labels here
    public int barHorizontalCount = 10;

    public RectTransform barVertical; //add labels here
    public int barVerticalCount = 10;

    public RectTransform boxArea;

    public Transform lineZero;

    private Text[] mLabelNumberHorz;
    private Text[] mLabelNumberVert;

    private bool mIsInit;

    void Init() {
        if(mIsInit)
            return;

        //generate labels
        mLabelNumberHorz = new Text[barHorizontalCount];

        //horizontal
        float x = 0f;
        float xOfs = barHorizontal.rect.width / (barHorizontalCount - 1);

        for(int i = 0; i < barHorizontalCount; i++) {
            var newLabel = Instantiate(labelHorizontalTemplate);
            var newLabelRT = newLabel.rectTransform;

            newLabelRT.SetParent(barHorizontal);
            newLabelRT.anchoredPosition = new Vector2(x, 0f);

            mLabelNumberHorz[i] = newLabel;

            x += xOfs;
        }

        labelHorizontalTemplate.gameObject.SetActive(false);
        //

        //vertical
        mLabelNumberVert = new Text[barVerticalCount];

        float y = 0f;
        float yOfs = barVertical.rect.height / (barVerticalCount - 1);

        for(int i = 0; i < barVerticalCount; i++) {
            var newLabel = Instantiate(labelVerticalTemplate);
            var newLabelRT = newLabel.rectTransform;

            newLabelRT.SetParent(barVertical);
            newLabelRT.anchoredPosition = new Vector2(0f, y);

            mLabelNumberVert[i] = newLabel;

            y += yOfs;
        }

        labelVerticalTemplate.gameObject.SetActive(false);
        //

        //setup caches
        

        mIsInit = true;
    }
}
