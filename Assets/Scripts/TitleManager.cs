using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void GotoNextScene(){
        GetComponent<CriAtomSource>().Stop();
		SceneNavigator.Instance.Change ("Instruction",1);
	}
}
