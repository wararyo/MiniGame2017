using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewMiniGame.asset",menuName ="MiniGameDefinition",order = 369)]
public class MiniGame : ScriptableObject {

	public string name;
	public string path;
	public Sprite logo;
	public Sprite screenShot;
	public GameObject ruleTabUi;
	public GameObject howToPlayTabUi;
}
