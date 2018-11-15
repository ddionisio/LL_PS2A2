using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphLineWidget : MonoBehaviour {
    [Header("Templates")]
    public Transform dotTemplate;

    public RectTransform lineTemplate; //ensure pivot is at bottom. height is used as length, and up is oriented towards the end point

    public Text labelHorizontalTemplate;
    public Text labelVerticalTemplate;
    public string labelFormat = "{0}";

    [Header("Display")]
    public RectTransform barHorizontal; //add labels here
    public int barHorizontalCount = 10;

    public RectTransform barVertical; //add labels here
    public int barVerticalCount = 10;

    public RectTransform lineArea;
    public RectTransform dotArea;
    
    private Text[] mLabelNumberHorz;
    private Text[] mLabelNumberVert;

    private M8.CacheList<Transform> mDots;
    private M8.CacheList<RectTransform> mLines;

    private M8.CacheList<Transform> mDotsCache;
    private M8.CacheList<RectTransform> mLinesCache;

    private float mXMin, mXMax;
    private float mYMin, mYMax;

    private bool mIsInit;

    public void Setup(float xMin, float xMax, float yMin, float yMax) {
        Init();

        Clear();

        //setup horizontal
        float xStep = Mathf.Abs(xMax - xMin) / barHorizontalCount;

        float curX = xMin;
        for(int i = 0; i < mLabelNumberHorz.Length; i++) {
            mLabelNumberHorz[i].text = string.Format(labelFormat, curX);
            curX += xStep;
        }

        mXMin = xMin;
        mXMax = xMax;

        //setup vertical
        float yStart = Mathf.Floor(yMin);
        float yEnd = Mathf.Ceil(yMax);

        float yStep = Mathf.Abs(yEnd - yStart) / barVerticalCount;

        float curY = yStart;
        for(int i = 0; i < mLabelNumberVert.Length; i++) {
            mLabelNumberVert[i].text = string.Format(labelFormat, curY);
            curY += yStep;
        }

        mYMin = yStart;
        mYMax = yEnd;
    }

    public void Plot(float x, float y) {
        Init();

        if(mDots.IsFull)
            return;

        float xLen = Mathf.Abs(mXMax - mXMin);
        float nX = (x - mXMin) / xLen;

        float yLen = Mathf.Abs(mYMax - mYMin);
        float nY = (y - mYMin) / yLen;

        Transform lastDotTrans = mDots.Count > 0 ? mDots[mDots.Count - 1] : null;

        //add dot
        var dotAreaSize = dotArea.rect.size;
        var dotTrans = mDotsCache.RemoveLast();
        dotTrans.localPosition = new Vector2(nX * dotAreaSize.x, nY * dotAreaSize.y);
        dotTrans.gameObject.SetActive(true);
        mDots.Add(dotTrans);

        //add line
        if(!mLines.IsFull && lastDotTrans) {
            Vector2 dPos = dotTrans.localPosition - lastDotTrans.localPosition;
            float len = dPos.magnitude;
            if(len > 0f) {
                var dir = dPos / len;

                var lineAreaSize = lineArea.rect.size;
                var lineRectTrans = mLinesCache.RemoveLast();
                lineRectTrans.localPosition = lastDotTrans.localPosition;
                lineRectTrans.up = dir;

                var lineSize = lineRectTrans.sizeDelta;
                lineSize.y = len;
                lineRectTrans.sizeDelta = lineSize;

                lineRectTrans.gameObject.SetActive(true);
                mLines.Add(lineRectTrans);
            }
        }
    }

    public void Clear() {
        if(mDots == null || mLines == null)
            return;

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
        mDots = new M8.CacheList<Transform>(barHorizontalCount);
        mDotsCache = new M8.CacheList<Transform>(barHorizontalCount);

        for(int i = 0; i < barHorizontalCount; i++) {
            var newDot = Instantiate(dotTemplate);
            newDot.SetParent(dotArea);
            newDot.gameObject.SetActive(false);
            mDotsCache.Add(newDot);
        }

        dotTemplate.gameObject.SetActive(false);

        mLines = new M8.CacheList<RectTransform>(barHorizontalCount);
        mLinesCache = new M8.CacheList<RectTransform>(barHorizontalCount);

        for(int i = 0; i < barHorizontalCount; i++) {
            var newLine = Instantiate(lineTemplate);
            newLine.SetParent(lineArea);
            newLine.gameObject.SetActive(false);
            mLines.Add(newLine);
        }

        lineTemplate.gameObject.SetActive(false);

        mIsInit = true;
    }
}
