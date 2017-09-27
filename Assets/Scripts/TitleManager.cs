using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Commander.Players[0].point = 0;
        Commander.Players[1].point = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
            GotoNextScene();
		}
	}

	void GotoNextScene(){
		SceneManager.LoadSceneAsync ("Shuffle");
	}
}
