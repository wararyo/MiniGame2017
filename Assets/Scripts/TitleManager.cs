using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TitleManager : MonoBehaviour {

    public List<VideoClip> videos;
    public GameObject videoUI;
    public float videoWaitTime = 60;

    VideoPlayer videoPlayer;

	// Use this for initialization
	void Start () {
        Commander.Initialize();
        videoPlayer = GetComponent<VideoPlayer>();
        StartCoroutine(playVideoWork());
	}
	
	// Update is called once per frame
	void Update () {

	}

    IEnumerator playVideoWork()
    {
        yield return new WaitForSecondsRealtime(videoWaitTime);
        videoPlayer.clip = videos[Random.Range(0,videos.Count - 1)];
        videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
        videoPlayer.Play();
        videoUI.SetActive(true);
    }

    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        videoPlayer.clip = null;
        StartCoroutine(playVideoWork());
        videoUI.SetActive(false);
    }

    public void GotoNextScene(){
        if (videoPlayer.clip) VideoPlayer_loopPointReached(videoPlayer);
        else
        {
            GetComponent<CriAtomSource>().Stop();
            SceneNavigator.Instance.Change("Instruction", 2);
        }
	}
}
