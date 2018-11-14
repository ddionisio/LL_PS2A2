using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphLineWidget : MonoBehaviour {
    [Header("Templates")]
    public Transform dotTemplate;

    public RectTransform lineTemplate;

    public Text labelTemplate;
    public string labelFormat = "{0}";

    [Header("Display")]
    public RectTransform barHorizontal; //add labels here
    public int barHorizontalCount = 10;

    public RectTransform barVertical; //add labels here
    public int barVerticalCount = 10;

    public RectTransform plotArea; //add lines and dots here
    
    private Text[] mLabelNumberHorz;
    private Text[] mLabelNumberVert;

    private M8.CacheList<Transform> mDots;
    private M8.CacheList<RectTransform> mLines;

    private M8.CacheList<Transform> mDotsCache;
    private M8.CacheList<RectTransform> mLinesCache;

    public void Setup(float xMin, float xMax, float yMin, float yMax) {
        Clear();

        //setup horizontal

        //setup vertical
    }

    public void Plot(float x, float y) {
        if(mDots.IsFull)
            return;


    }

    public void Clear() {
        for(int i = 0; i < mDots.Count; i++) {
            mDots[i].gameObject.SetActive(false);
            mDotsCache.Add(mDots[i]);
        }

        mDots.Clear();

        for(int i = 0; i < mLines.Count; i++) {
            mLines[i].gameObject.SetActive(false);
            mLinesCache.Add(mLines[i]);
        }

        mLines.Clear();
    }

    void Awake() {
        //generate labels
        mLabelNumberHorz = new Text[barHorizontalCount];

        //horizontal
        float x = 0f;
        float xOfs = barHorizontal.rect.width / barHorizontalCount;

        for(int i = 0; i < barHorizontalCount; i++) {
            var newLabel = Instantiate(labelTemplate);
            var newLabelRT = newLabel.rectTransform;

            newLabelRT.SetParent(barHorizontal);
            newLabelRT.anchoredPosition = new Vector2(x, 0f);

            mLabelNumberHorz[i] = newLabel;

            x += xOfs;
        }

        //vertical
        mLabelNumberVert = new Text[barVerticalCount];

        float y = 0f;
        float yOfs = barHorizontal.rect.width / barHorizontalCount;

        for(int i = 0; i < barHorizontalCount; i++) {
            var newLabel = Instantiate(labelTemplate);
            var newLabelRT = newLabel.rectTransform;

            newLabelRT.SetParent(barVertical);
            newLabelRT.anchoredPosition = new Vector2(0f, y);

            mLabelNumberVert[i] = newLabel;

            y += yOfs;
        }

        labelTemplate.gameObject.SetActive(false);
        //

        //setup caches
        mDots = new M8.CacheList<Transform>(barHorizontalCount);
        mDotsCache = new M8.CacheList<Transform>(barHorizontalCount);

        for(int i = 0; i < barHorizontalCount; i++) {
            var newDot = Instantiate(dotTemplate);
            newDot.gameObject.SetActive(false);
            mDotsCache.Add(newDot);
        }

        dotTemplate.gameObject.SetActive(false);

        mLines = new M8.CacheList<RectTransform>(barHorizontalCount);
        mLinesCache = new M8.CacheList<RectTransform>(barHorizontalCount);

        for(int i = 0; i < barHorizontalCount; i++) {
            var newLine = Instantiate(lineTemplate);
            newLine.gameObject.SetActive(false);
            mLines.Add(newLine);
        }

        lineTemplate.gameObject.SetActive(false);
    }
}
