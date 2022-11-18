using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Make sure to create this in Resources with name: gameData
/// </summary>
[CreateAssetMenu(fileName = "gameData", menuName = "Game/Game Data", order = 0)]
public class GameData : M8.SingletonScriptableObject<GameData> {
    [System.Serializable]
    public struct LevelData {
        public M8.SceneAssetPath introScene;
        public M8.SceneAssetPath levelScene;
    }

    [Header("Scenes")]
    public M8.SceneAssetPath introScene;
    public M8.SceneAssetPath endScene;

    [Header("Levels")]
    public LevelData[] levels;

    [Header("Scoring")]
    public int hitScore = 500;

    [Header("Signals")]
    public SignalGameFlag signalGameFlagChanged;

    public bool isGameStarted { get; private set; } //true: we got through start normally, false: debug
    public int curLevelIndex { get; private set; }
    public LevelData curLevelData { get { return levels[curLevelIndex]; } }

    private Dictionary<string, int> mFlags;

    public void SetFlag(string key, int flag) {
        if(mFlags == null)
            mFlags = new Dictionary<string, int>();

        if(mFlags.ContainsKey(key)) {
            if(mFlags[key] != flag) {
                mFlags[key] = flag;

                if(signalGameFlagChanged) signalGameFlagChanged.Invoke(key, flag);
            }
        }
        else {
            mFlags.Add(key, flag);

            if(signalGameFlagChanged) signalGameFlagChanged.Invoke(key, flag);
        }
    }

    public int GetFlag(string key) {
        if(mFlags == null)
            return 0;

        int flag = 0;
        mFlags.TryGetValue(key, out flag);
        return flag;
    }

    public int ComputeHitScore(int hitQuota, int numHit) {
        float scale = ((float)hitQuota) / numHit;
        return Mathf.RoundToInt(scale * hitScore);
    }

    /// <summary>
    /// Called in start scene
    /// </summary>
    public void Begin() {
        isGameStarted = true;

        if(LoLManager.instance.curProgress == 0 && introScene.isValid)
            introScene.Load();
        else {
            //LoLMusicPlaylist.instance.Play();
            Current();
        }
    }

    /// <summary>
    /// Update level index based on current progress, and load scene
    /// </summary>
    public void Current() {
        int progress = LoLManager.instance.curProgress;

        if(progress == LoLManager.instance.progressMax)
            endScene.Load();
        else if(progress < levels.Length) {
            UpdateLevelIndexFromProgress(progress);

            if(curLevelIndex < levels.Length) {
                //play intro scene, otherwise proceed to level
                if(levels[curLevelIndex].introScene.isValid)
                    levels[curLevelIndex].introScene.Load();
                else
                    levels[curLevelIndex].levelScene.Load();
            }
        }
    }

    /// <summary>
    /// Update progress, go to next level-scene
    /// </summary>
    public void Progress() {
        var curScene = M8.SceneManager.instance.curScene;

        if(isGameStarted) {
            //we are in intro, proceed
            if(curScene.name == introScene.name) {
                Current();
            }
            //ending if we are already at max
            else if(LoLManager.instance.curProgress == LoLManager.instance.progressMax)
                endScene.Load();
            //proceed to next progress
            else {
                //we are in intro, proceed to level
                if(curLevelData.introScene.isValid && curLevelData.introScene == curScene) {
                    //load level if valid, otherwise, proceed to next progress
                    if(curLevelData.levelScene.isValid) {
                        curLevelData.levelScene.Load();
                        return;
                    }
                }

                LoLManager.instance.ApplyProgress(LoLManager.instance.curProgress + 1);
                Current();
            }
        }
        else { //debug
            if(curScene.name == introScene.name) {
                isGameStarted = true;
                LoLManager.instance.ApplyProgress(0, 0);
                Current();
            }
            else if(curScene.name == endScene.name) {
                LoLManager.instance.Complete();
            }
            else {
                //check which level we are on
                for(int i = 0; i < levels.Length; i++) {
                    var level = levels[i];

                    if(level.introScene == curScene) {
                        isGameStarted = true;

                        if(level.levelScene.isValid) {
                            LoLManager.instance.ApplyProgress(i);
                            level.levelScene.Load();
                        }
                        else {
                            LoLManager.instance.ApplyProgress(i + 1);
                            Current();
                        }

                        return;
                    }
                    else if(level.levelScene == curScene) {
                        isGameStarted = true;

                        LoLManager.instance.ApplyProgress(i + 1);
                        Current();

                        return;
                    }
                }

                M8.SceneManager.instance.Reload(); //not found, just reload current
            }
        }
    }

    public void ApplyLevelIndex(int ind) {
        isGameStarted = true;
        curLevelIndex = ind;
        LoLManager.instance.ApplyProgress(ind);
    }

    protected override void OnInstanceInit() {
        //compute max progress
        if(LoLManager.isInstantiated) {
            LoLManager.instance.progressMax = levels.Length;
        }

        isGameStarted = false;
        curLevelIndex = 0;
    }

    private void UpdateLevelIndexFromProgress(int progress) {
        curLevelIndex = progress;
    }
}
