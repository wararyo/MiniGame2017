﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
            GotoNextScene();
		}
	}

	void GotoNextScene(){
		SceneManager.LoadSceneAsync ("Instruction");
	}
}
