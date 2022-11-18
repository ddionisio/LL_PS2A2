using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class ActIntroController : GameModeController<ActIntroController> {
    [System.Serializable]
    public struct LevelData {
        public int levelIndex;
        public GameObject rootGO;
        public string musicPath;
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
        int levelInd;

        if(GameData.instance.isGameStarted)
            levelInd = GameData.instance.curLevelIndex;
        else {
            GameData.instance.ApplyLevelIndex(debugLevelIndex);
            levelInd = debugLevelIndex;
        }

        LevelData curLevelData = new LevelData();

        for(int i = 0; i < levelMatches.Length; i++) {
            if(levelMatches[i].levelIndex == levelInd) {
                curLevelData = levelMatches[i];
                break;
            }
        }

        //play music
        if(!string.IsNullOrEmpty(curLevelData.musicPath))
            LoLManager.instance.PlaySound(curLevelData.musicPath, true, true);

        yield return base.Start();

        //show matching level
        if(curLevelData.rootGO)
            curLevelData.rootGO.SetActive(true);

        yield return new WaitForSeconds(proceedShowDelay);

        proceedGO.SetActive(true);
    }
}
