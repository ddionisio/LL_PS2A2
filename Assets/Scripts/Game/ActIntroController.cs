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

    [Header("Proceed")]
    public GameObject proceedGO;
    public float proceedShowDelay = 1.0f;

    [Header("Debug")]
    public int debugLevelIndex;

    protected override void OnInstanceInit() {
        base.OnInstanceInit();
                
        for(int i = 0; i < levelMatches.Length; i++) {
            if(levelMatches[i].rootGO)
                levelMatches[i].rootGO.SetActive(false);
        }

        proceedGO.SetActive(false);
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

        yield return new WaitForSeconds(proceedShowDelay);

        proceedGO.SetActive(true);
    }
}
