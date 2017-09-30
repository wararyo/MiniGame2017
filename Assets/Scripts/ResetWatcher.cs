using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetWatcher : SingletonMonoBehaviour<ResetWatcher> {

    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetButton("5") &&
            Input.GetButton("6") &&
            Input.GetButton("7") &&
            Input.GetButton("8") &&
            Input.GetButton("9") &&
            Input.GetButton("10"))
        {
            ResetGame();
        }
    }

    public void ResetGame()
    {
        SceneNavigator.Instance.Change("Title");
    }
}
