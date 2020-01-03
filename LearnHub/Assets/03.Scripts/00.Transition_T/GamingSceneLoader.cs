using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GamingSceneLoader : ScenesLoader {

    public static bool CanStart { get; set; }
    public Text playerNumText;

    private bool LoaderLock;

    protected override void Start() {
        LoaderLock = false;
    }

    private void Update() {
        playerNumText.text = Setup.RefRoom.RoomMembers + "/" + Setup.RefRoom.Members;

        if (CanStart && !LoaderLock) {
            Loadlevel(3);
            LoaderLock = true;
        }
    }

    private IEnumerator WaitingServerToStartGame() {

        while (!CanStart) {
            playerNumText.text = Setup.RefRoom.RoomMembers + "/" + Setup.RefRoom.Members;
            Thread.Sleep(100);
            //### 未結局等待超時
            yield return null;
        }

        Debug.Log($"Test: {CanStart}");
    }

}
