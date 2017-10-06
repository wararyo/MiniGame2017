using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MyButton : MonoBehaviour {

    [SerializeField]
    private bool _buttonEnabled = true;
    public bool buttonEnabled
    {
        get
        {
            return _buttonEnabled;
        }
        set
        {
            _buttonEnabled = value;
            if (value)
            {
                GetComponent<Image>().color = enabledTint;
            }
            else
            {
                GetComponent<Image>().color = disabledTint;
            }
        }

    }

    Color disabledTint = new Color(.8f, .8f, .8f, .6f);
    Color enabledTint = new Color(1, 1, 1, 1);

    public string inputButton;

	public Sprite imageDefault;
	public Sprite imagePressed;

	public UnityEvent onPressed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (buttonEnabled)
        {
            if (Input.GetButtonDown(inputButton))
            {
                StartCoroutine(buttonPressingWork());
            }
        }
	}

    IEnumerator buttonPressingWork()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Pressed");

        Image image = GetComponent<Image>();
        image.sprite = imagePressed;

        for(float i = 0;i < 1;i += Time.deltaTime)
        {
            if (Input.GetButtonUp(inputButton))
            {
                anim.SetTrigger("Up");
                image.sprite = imageDefault;
                GetComponent<CriAtomSource>().Play();
                onPressed.Invoke();
                yield break;
            }
            yield return null;
        }
        image.sprite = imageDefault;
        anim.SetTrigger("Canceled");
    }
}
