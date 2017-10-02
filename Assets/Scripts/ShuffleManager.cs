using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class ShuffleManager : MonoBehaviour
{
    public Text titleText;
	public PointView pointView1P;
	public PointView pointView2P;
	public MiniGameCard miniGameCard;
	public InstructionUI instruction;
    public WinnerUI winnerUi;
    public CriAtomSource audioSource;
    public GameObject anten;
    public CueEvent_MCBaloon baloon;
    public MyButton nextBtn;
    public MyButton practiceBtn;

    public List<MiniGame> miniGames;
    public List<bool> PlayedGames;

    int nextMiniGameID = 0;
	MiniGame nextMiniGame;

    [NonSerialized]
    public int winner = 0;

    // Use this for initialization
    void Start()
    {
        while (true)
        {
            nextMiniGameID = UnityEngine.Random.Range(0, miniGames.Count);
            nextMiniGame = miniGames[nextMiniGameID];
            if (Commander.PlayedMiniGames.Contains(nextMiniGame)) continue;
            else break;
        }

		int gameCount = Commander.Players [0].point + Commander.Players [1].point + 1;
		titleText.text = string.Format ("第{0}回戦", gameCount);
        baloon.replaceList.Add("Round", gameCount.ToString());

		miniGameCard.Initialize (nextMiniGame.screenShot, nextMiniGame.name);

		pointView1P.Point = Commander.Players [0].point;
		pointView2P.Point = Commander.Players [1].point;

		instruction.Initialize (nextMiniGame);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartPractice()
    {
        StartCoroutine(ExecutePracticeWork());
    }

    IEnumerator ExecutePractice()
    {
        string FilePath = getPath() + nextMiniGame.path;

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
        yield return null;
    }

    IEnumerator ExecutePracticeWork()
    {
        nextBtn.buttonEnabled = false;
        practiceBtn.buttonEnabled = false;
        anten.GetComponent<CueEvent_UIInAndOut>().Cue(null);
        audioSource.Stop();
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(ExecutePractice());
        yield return new WaitForSeconds(2);
        anten.GetComponent<CueEvent_UIInAndOut>().Cue(null);
        audioSource.Play();
        nextBtn.buttonEnabled = true;
    }

    public void StartMiniGame()
    {
        StartCoroutine(ExecuteMiniGameWork());
    }

    void ExecuteMiniGame()
    {
        string FilePath = getPath() + nextMiniGame.path;

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

    IEnumerator ExecuteMiniGameWork()
    {
        nextBtn.buttonEnabled = false;
        practiceBtn.buttonEnabled = false;
        anten.GetComponent<CueEvent_UIInAndOut>().Cue(null);
        audioSource.Stop();
        yield return new WaitForSeconds(2);
        ExecuteMiniGame();
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
				Commander.Players [0].point++;
                baloon.replaceList.Add("Winner", "1P");
				winner = 1;
			}
			else if (stdout.StartsWith("2"))//2Pの勝ち
			{
				Commander.Players [1].point++;
                baloon.replaceList.Add("Winner", "2P");
				winner = 2;
            }
            else if (stdout.ToLower().StartsWith("esc"))
            {
                ResetWatcher.Instance.ResetGame();
            }
		}
		if (winner == 0) titleText.text = "勝敗情報が送信されずに終了しました";
        else { Commander.PlayedMiniGames.Add(nextMiniGame); }
        winnerUi.Winner = winner;

        GetComponent<MyCueScenePlayer>().Invoke();

        if (Commander.Players[0].point >= Commander.VICTORY_COUNT)
        {//1Pの優勝
            StartCoroutine(GotoVictoryScene());
        }
        else if (Commander.Players[1].point >= Commander.VICTORY_COUNT)
        {//2Pの優勝
            StartCoroutine(GotoVictoryScene());
        }
        else 
            StartCoroutine(GotoNextGame());
	}

    private IEnumerator GotoNextGame()
    {
        yield return new WaitForSeconds(6);
        SceneNavigator.Instance.Change("Shuffle");
    }

	private IEnumerator GotoVictoryScene(){
		yield return new WaitForSeconds (6);
        SceneNavigator.Instance.Change("Victory");
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
