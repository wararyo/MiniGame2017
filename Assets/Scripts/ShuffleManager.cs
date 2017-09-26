﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ShuffleManager : MonoBehaviour
{

    public Text nextMiniGameText;

    int nextMiniGameID = 0;

    [NonSerialized]
    public int winner = 0;

    bool isGameStartWaiting = false;

    // Use this for initialization
    void Start()
    {
        nextMiniGameID = UnityEngine.Random.Range(0, Commander.Minigames.Length);

        nextMiniGameText.text = "次のゲームは\n" + Commander.Minigames[nextMiniGameID].name + "\nAを押してスタート";

        isGameStartWaiting = true;

    }

    // Update is called once per frame
    void Update()
    {
        if(isGameStartWaiting && Input.GetButtonDown("Fire1"))
        {
            isGameStartWaiting = false;
            StartCoroutine(coroutine());
        }
    }

    IEnumerator coroutine()
    {
        yield return new WaitForSeconds(0.1f);
        ExecuteMiniGame();
    }

    void ExecuteMiniGame()
    {
        string FilePath = getPath() + Commander.Minigames[nextMiniGameID].path;

        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = FilePath;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        //process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(OutputHandler);
        process.StartInfo.CreateNoWindow = false;
        process.EnableRaisingEvents = true;
        process.Exited += new System.EventHandler(Process_Exit);
        process.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(FilePath);
        process.StartInfo.Arguments =
            Commander.Players[0].name + " " +
            Commander.Players[0].characterId + " " +
            Commander.Players[1].name + " " +
            Commander.Players[1].characterId;

        process.Start();
        //process.BeginOutputReadLine();
        process.WaitForExit();
		OnEndMiniGame(process.StandardOutput.ReadToEnd());
    }

    // プロセス終了時.
    private void Process_Exit(object sender, System.EventArgs e)
    {
        System.Diagnostics.Process proc = (System.Diagnostics.Process)sender;
        // プロセスを閉じる.
        proc.Kill();
    }

	private void OnEndMiniGame(string stdout){
		if (!string.IsNullOrEmpty(stdout))
		{
			Debug.Log(stdout);
			if (stdout.StartsWith("1"))//1Pの勝ち
			{
				nextMiniGameText.text = "1Pの勝ち！";
				Commander.Players [0].point++;
				winner = 1;
				if (Commander.Players [0].point >= Commander.VICTORY_COUNT) {//1Pの優勝
					GotoVictoryScene();
				}
			}
			else if (stdout.StartsWith("2"))//2Pの勝ち
			{
				nextMiniGameText.text = "2Pの勝ち！";
				Commander.Players [1].point++;
				winner = 2;
				if (Commander.Players [1].point >= Commander.VICTORY_COUNT) {//2Pの優勝
					GotoVictoryScene();
				}
			}
		}
		if (winner == 0) nextMiniGameText.text = "勝敗情報が送信されずに終了しました";
		StartCoroutine(GotoNextGame());
	}

    private IEnumerator GotoNextGame()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadSceneAsync("Shuffle");
    }

	private IEnumerator GotoVictoryScene(){
		yield return new WaitForSeconds (4);
		SceneManager.LoadSceneAsync ("Victory");
	}

    public string getPath()
    {
        string result = Application.dataPath;
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            // MacOSでビルドした場合のパス(.appファイルの同階層のパス)
            result = result + "/../../";
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            // Windowsでビルドした場合のパス(.exeファイルの同階層のパス)
            result = result + "/../";
        }
        else
        {
            // それ以外の場合(Assets/Resourcesフォルダのパス)
            result = result + "/../";
        }
        return result;
    }
}