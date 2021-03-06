﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryManager : MonoBehaviour
{

    public WinnerUI winner;
    public CueEvent_MCBaloon baloon;
    public PointView pointView1P;
    public PointView pointView2P;
    public GameObject credit;
    public Animator Character1;
    public Animator Character2;

    int victory;

    // Use this for initialization
    void Start()
    {
        if(Commander.Players[0].point >= 2)
        {
            victory = 1;
            baloon.replaceList.Add("Victory", "1P");
            Character1.SetBool("Happy", true);
            Character2.SetBool("Sad", true);
        }
        else if (Commander.Players[1].point >= 2)
        {
            victory = 2;
            baloon.replaceList.Add("Victory", "2P");
            Character1.SetBool("Sad", true);
            Character2.SetBool("Happy", true);
        }
        else
        {
            baloon.replaceList.Add("Victory", "ウェイ");
        }

        pointView1P.Point = Commander.Players[0].point;
        pointView2P.Point = Commander.Players[1].point;

        winner.Winner = victory;

        foreach(var item in Commander.PlayedMiniGames)
        {
            Instantiate(item.creditUi, credit.transform);
        }
        credit.SetActive(false);
        credit.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void GotoNextScene()
    {
        SceneManager.LoadSceneAsync("Title");
    }
}
