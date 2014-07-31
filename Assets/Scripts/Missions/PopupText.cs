using UnityEngine;
using System.Collections;

public class PopupText : MonoBehaviour {

    dfLabel label;
    float timer, fadeTimer;
    bool show;
    dfTweenVector3 posTween;
    dfTweenFloat widthTween, heightTween;

	// Use this for initialization
	void Awake () {
        this.label = GetComponent<dfLabel>();
        this.posTween = GetComponent<dfTweenVector3>();
        dfTweenFloat[] tweens = GetComponents<dfTweenFloat>();
        foreach (dfTweenFloat tw in tweens)
        {
           
            if (tw.TweenName.Equals("Scale width"))
            {
                widthTween = tw;
            }
            else
            {
                heightTween = tw;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
     //   fadeTimer -= Time.deltaTime;
        if (show && timer < 0)
        {
            //label.Hide();
            show = false;
            posTween.Play();
            widthTween.Play();
            heightTween.Play();
        }
	}

    public void ShowText(string text, float duration)
    {
        posTween.Reset();
        widthTween.Reset();
        heightTween.Reset();
        label.RelativePosition = posTween.StartValue;
        label.Text = text;
        label.Show();
        show = true;
        timer = duration;
        fadeTimer = duration / 2f;
    }

    public void ShowText(string text)
    {
        ShowText(text, 2);
    }
}
