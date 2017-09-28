using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointView : MonoBehaviour {

	public Image point1;
	public Image point2;

	public Texture pointTexture;
	public Texture pointEmptyTexture;

	private int point;
	public int Point{
		get{
			return point;
		}
		set{
			point = value;
			point1.material.mainTexture = (point >= 1) ? pointTexture : pointEmptyTexture;
			point2.material.mainTexture = (point >= 2) ? pointTexture : pointEmptyTexture;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
