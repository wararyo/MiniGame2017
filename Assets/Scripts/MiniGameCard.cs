using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameCard : MonoBehaviour {

	public Image thumbImage;
	public Text nameText;

	public bool isVisible{
		set{
			foreach (MaskableGraphic m in GetComponentsInChildren<MaskableGraphic>()) {
				m.enabled = value;
			}
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Initialize(Sprite thumb,string name){
		thumbImage.sprite = thumb;
		nameText.text = name;
	}
}
