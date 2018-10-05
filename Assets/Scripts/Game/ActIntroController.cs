using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActIntroController : GameModeController<ActIntroController> {
    [System.Serializable]
    public struct LevelData {
        public int levelIndex;
        public GameObject rootGO;
    }

    [Header("Data")]
    public LevelData[] levelMatches;

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField="animator")]
    public string takeEnter;
    public float enterStartDelay = 1.0f;

    [Header("Debug")]
    public int debugLevelIndex;

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        for(int i = 0; i < levelMatches.Length; i++) {
            if(levelMatches[i].rootGO)
                levelMatches[i].rootGO.SetActive(false);
        }
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        int levelInd;

        if(GameData.instance.isGameStarted)
            levelInd = GameData.instance.curLevelIndex;
        else {
            GameData.instance.ApplyLevelIndex(debugLevelIndex);
            levelInd = debugLevelIndex;
        }

        //show matching level
        for(int i = 0; i < levelMatches.Length; i++) {
            if(levelMatches[i].levelIndex == levelInd) {
                if(levelMatches[i].rootGO)
                    levelMatches[i].rootGO.SetActive(true);
                break;
            }
        }

        yield return new WaitForSeconds(enterStartDelay);

        if(animator && !string.IsNullOrEmpty(takeEnter))
            animator.Play(takeEnter);
    }
}
