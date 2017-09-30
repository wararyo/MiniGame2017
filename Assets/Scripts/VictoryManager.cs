using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryManager : MonoBehaviour
{

    public Text text;

    // Use this for initialization
    void Start()
    {
        if(Commander.Players[0].point >= 2)
        {
            text.text = "1Pの優勝！";
        }
        else if (Commander.Players[1].point >= 2)
        {
            text.text = "2Pの優勝！";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("4"))
        {
            GotoNextScene();
        }
    }

    void GotoNextScene()
    {
        SceneManager.LoadSceneAsync("Title");
    }
}
