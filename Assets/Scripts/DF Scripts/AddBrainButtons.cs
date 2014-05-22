using UnityEngine;
using System.Collections;
using Parse;
using System.Linq;
using System.Collections.Generic;

public class AddBrainButtons : MonoBehaviour {

    private dfPanel _panel;
    private IEnumerable<ParseObject> brains;
    private bool DataLoaded = false;

    private List<Brain> rootBrains = new List<Brain>();

	// Use this for initialization
    void Start()
    {
        this._panel = GetComponent<dfPanel>();

        //   _panel.transform.Find("NewBrainButton").GetComponent<dfButton>().Click += button_Click;

        var q = new ParseQuery<Brain>().WhereEqualTo("userId", ParseUser.CurrentUser.ObjectId);
        q.FindAsync().ContinueWith(t =>
            {
                IEnumerable<Brain> result = t.Result;
                Dictionary<string, Brain> allBrains = new Dictionary<string, Brain>();

                foreach (Brain brain in result)
                {
                    if (brain.ParentId != null)
                    {
                        result.Where(b => b.ObjectId == brain.ParentId).First().Children.Add(brain);
                    }
                    else
                    {
                        rootBrains.Add(brain);
                    }
                }

                DataLoaded = true;
            });
    }

    private void AddButton(Brain b, int level, float ratio, float idx)
    {
        var spacing = 90;
        var count = _panel.Controls.Count;
        var button = _panel.AddControl<BrainButton>();
        button.BackgroundSprite = "listitem-selected";
        button.HoverSprite = "listitem-hover";
        button.PressedSprite = "listitem-selected";
        button.Text = level + ". " + b.Name;
        button.Width = _panel.Width * ratio - 10;
        button.Height = 48;
        button.RelativePosition = new Vector3(_panel.Width * idx + 5, level * spacing);
        button.IsNewBrain = b.IsNewBrain;
        button.Brain = b;

        button.Click += button_Click;
     //   button.MouseHover += button_MouseHover;
      //  button.MouseLeave += button_MouseLeave;

        if (level > 0)
        {
            var line = _panel.AddControl<dfSprite>();
            line.SpriteName = "vslider-track-normal";
            line.Width = 12;
            line.Height = spacing - button.Height;
            line.RelativePosition = new Vector3(_panel.Width * idx + button.Width / 2, level * spacing - line.Height);
        }

        var plus = button.AddControl<BrainButton>();
        plus.BackgroundSprite = "button-normal";
        plus.HoverSprite = "button-hover";
        plus.DisabledSprite = "button-disabled";
        plus.PressedSprite = "button-normal";
        plus.Text = "New";
        plus.Width = 40;
        plus.Height = 25;
        plus.RelativePosition = new Vector3(button.Width - plus.Width - 5, 10);
        plus.Brain = b;
        plus.Click += new_button_Click;
    }

    void button_MouseLeave(dfControl control, dfMouseEventArgs mouseEvent)
    {
        BrainButton but = control as BrainButton;
        but.Width = but.OriginalWidth;
        
        but.ZOrder = 10;
    }

    void button_MouseHover(dfControl control, dfMouseEventArgs mouseEvent)
    {
        BrainButton but = control as BrainButton;
        but.OriginalWidth = but.Width;
        but.Width = 400;
        but.ZOrder = 50;
    }

    void button_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        //print("Button clicked");
        BrainButton bb = control as BrainButton;
        Settings.Brain = bb.Brain;
    //    Settings.IsNewBrain = bb.IsNewBrain;
        Application.LoadLevel("Training Overview");
        
    }

    void new_button_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        BrainButton bb = control as BrainButton;
        
        Settings.Brain = bb.Brain.Branch();
      
        Application.LoadLevel("Training Overview");
        
        //    Application.LoadLevel("Training Overview");
            mouseEvent.Use();
    }
	
	// Update is called once per frame
	void Update () {
        if (DataLoaded)
        {
            AddBrains();
        }
	}

    void AddBrains()
    {
        if (rootBrains.Count == 0)
        {
            Brain b = new Brain();
            b.Name = "New brain";
            b.UserId = ParseUser.CurrentUser.ObjectId;
            b.IsNewBrain = true;
            rootBrains.Add(b);
        }
        GameObject.Find("MainPanel").GetComponent<dfPanel>().Enable();
        GameObject.Find("LoadingLabel").GetComponent<dfLabel>().IsVisible = false;
        DataLoaded = false;
        int level = 0;
        int idx = 0;
        float ratio = 1.0f / rootBrains.Count;
       
        foreach (Brain b in rootBrains)
        {
            recurseAdd(b, level, ratio, idx, idx * ratio);
            idx++;
        }
    }

    void recurseAdd(Brain b, int level, float ratio, int idx, float startRatio)
    {

        print(level + ". " + b.Name + ", Startratio: " + startRatio);
        AddButton(b, level, ratio, startRatio);
        if (b.Children.Count > 0)
        {
            int count = b.Children.Count;
            float r = ratio / count;
            int i = 0;
            foreach (Brain child in b.Children)
            {
                recurseAdd(child, level + 1, r, i, startRatio + i * r);
                i++;
            }
        }
        
        
    }
}