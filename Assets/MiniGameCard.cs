using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameCard : MonoBehaviour {

	public Image thumbImage;
	public Text nameText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Initialize(Texture thumb,string name){
		thumbImage.material.mainTexture = thumb;
		nameText.text = name;
	}
}
