using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using wararyo.EclairCueMaker;

public class CueEvent_MCBaloon : CueEventBase {

	public Image baloon;
	public Text text;
	public Image nextIcon;
    public Animator anim;

    public string content;

    public Coroutine textPoppingCoroutine;

    float textPopInterval = 0.05f;

    bool _isAnimating = false;
	public bool isAnimating {
		get {
			return _isAnimating;
		}
	}

	bool isShowing{
		set{
			baloon.enabled = value;
			text.enabled = value;
			nextIcon.enabled = value;
            if (value)
            {
                nextIcon.GetComponent<Animator>().SetBool("Visible", false);
            }
        }
	}

	void Start () {
		isShowing = false;
	}

	/// <summary>
	/// CueSceneEditorのCue編集画面においてCueEventリストに表示される文字列です。
	/// </summary>
	public override string EventName
	{
		get
		{
			return "Show MCBaloon";
		}
	}

	/// <summary>
	/// CueEvent同士を区別するときに用いる識別子です。
	/// 他と被らない一意的な文字列である必要があります。
	/// 適当にキーボードをガチャガチャしてください。
	/// このCueEventが何らかのCueから参照されているときにこの値を変更した場合、動作は保証されません。
	/// </summary>
	public override string EventID
	{
		get
		{
			return "4j56789gken765k789sd07jh78";
		}
	}

	/// <summary>
	/// CueSceneEditorのCue編集画面でこのCueEventを指定したとき、ParamTypeで指定した型に応じて引数編集GUIが表示されます。
	/// CueScenePlayerがCueメソッドを実行するとき、引数paramにはParamTypeで指定した型の変数が入ります。
	/// 現在サポートしているParamTypeは以下の通りです。
	/// string
	/// int
	/// float
	/// GameObject
	/// </summary>
	public override Type ParamType
	{
		get
		{
			return typeof(string);
		}
	}


	public override void Cue(object param)
	{
		//ここに任意の処理を記述
		if (string.IsNullOrEmpty ((string)param)) {
			isShowing = false;
		} else {
			isShowing = true;
			content = ((string)param).Replace("¥n","\n");
            textPoppingCoroutine = StartCoroutine(textPoppingWork());
            anim.SetTrigger("Trigger");
            _isAnimating = true;
		}
	}

    IEnumerator textPoppingWork()
    {
        for (int i = 0; i <= content.Length; i++)
        {
            text.text = content.Substring(0, i);
            yield return new WaitForSeconds(textPopInterval);
        }
        OnEndAnimation();
    }

	public void Skip(){
        StopCoroutine(textPoppingCoroutine);
        text.text = content;
        OnEndAnimation();
	}

    void OnEndAnimation()
    {
        _isAnimating = false;
        nextIcon.GetComponent<Animator>().SetBool("Visible", true);
    }

}
