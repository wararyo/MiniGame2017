﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionUI : MonoBehaviour {

	int _tab = 0;
	int Tab{
		get{
			return _tab;
		}
		set{
			_tab = value;
			if (value == 0) {
				ruleContent.SetActive (true);
				howToPlayContent.SetActive (false);
				currentTab.anchoredPosition = tab0.anchoredPosition;
			} else if (value == 1) {
				ruleContent.SetActive (false);
				howToPlayContent.SetActive (true);
				currentTab.anchoredPosition = tab1.anchoredPosition;
			}
		}
	}

	public Image logoImage;
	public RectTransform currentTab;
	public RectTransform tab0;
	public RectTransform tab1;
	GameObject ruleContent;
	GameObject howToPlayContent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		int down = InputUtil.GetHorizontalDown ();
		if (down < 0 || Input.GetButtonDown("5")) {//left
			Tab = 0;
		}
		else if (down > 0 || Input.GetButtonDown("6")) {//right
			Tab = 1;
		}
	}

	public void Initialize(MiniGame mg){
		ruleContent = Instantiate (mg.ruleTabUi, transform);
		howToPlayContent = Instantiate (mg.howToPlayTabUi, transform);
		howToPlayContent.SetActive(false);
		logoImage.sprite = mg.logo;
		Tab = 0;
	}
}
