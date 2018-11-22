using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is for Act 3 levels
/// </summary>
public class ActForceController : GameModeController<ActForceController> {
    [Header("Game")]
    [M8.TagSelector]
    public string playerTag;
    [M8.TagSelector]
    public string forceGroupUITag;
    [M8.TagSelector]
    public string playGroupUITag;

    private GameObject mPlayerGO;
    private GameObject mForceGroupUIGO;
    private GameObject mPlayGroupUIGO;

    protected override void OnInstanceDeinit() {
        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        mPlayerGO = GameObject.FindGameObjectWithTag(playerTag);
        mPlayerGO.SetActive(false);

        mForceGroupUIGO = GameObject.FindGameObjectWithTag(forceGroupUITag);
        mForceGroupUIGO.SetActive(false);

        mPlayGroupUIGO = GameObject.FindGameObjectWithTag(playGroupUITag);
        mPlayGroupUIGO.SetActive(false);
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //spawn player
        mPlayerGO.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        //

        //first time detail

        //activate force selection
        mForceGroupUIGO.SetActive(true);

        //first time - show drags

        //activate play
        mPlayGroupUIGO.SetActive(true);

        //first time - point at play
    }
}
