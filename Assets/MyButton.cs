﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyButton : MonoBehaviour {

	public string inputButton;

	public Sprite imageDefault;
	public Sprite imagePressed;

	public UnityEvent onPressed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown(inputButton)){
			onPressed.Invoke();
		}
	}
}
