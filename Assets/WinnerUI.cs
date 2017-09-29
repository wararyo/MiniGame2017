using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerUI : MonoBehaviour {

    public UnityEngine.UI.Image image;
    public Sprite player1;
    public Sprite player2;

    private int winner;
    public int Winner
    {
        get { return winner; }
        set { winner = value;
            if (winner == 1) image.sprite = player1;
            else if (winner == 2) image.sprite = player2;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
