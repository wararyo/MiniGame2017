using System.Collections;
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
			} else if (value == 1) {
				ruleContent.SetActive (false);
				howToPlayContent.SetActive (true);
			}
		}
	}

	public Image logoImage;
	GameObject ruleContent;
	GameObject howToPlayContent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		int down = InputUtil.GetHorizontalDown ();
		if (down < 0) {//left
			Tab = 0;
		}
		if (down > 0) {//right
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
