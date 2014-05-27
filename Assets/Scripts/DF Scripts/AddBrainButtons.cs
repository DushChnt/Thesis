using UnityEngine;
using System.Collections;
using Parse;
using System.Linq;
using System.Collections.Generic;

public class AddBrainButtons : MonoBehaviour {

    private dfPanel _panel;
    private IEnumerable<ParseObject> brains;
    private bool DataLoaded = false;
    private bool ExpandedState = false;

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

        GameObject.Find("Back Button").GetComponent<dfButton>().Click += AddBrainButtons_Click;
    }

    void AddBrainButtons_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        if (ExpandedState)
        {
            AddBrains(rootBrains);
            ExpandedState = false;
        }
    }

    private float HypotenuseLength(float pc, float c, float B)
    {
        float A = pc - c;
        return Mathf.Sqrt(A * A + B * B);
    }

    private float Angle(float a, float b)
    {
        float angle = Mathf.Atan(b / a) * Mathf.Rad2Deg;
        return angle;
    }

    private float AddButton(Brain b, int level, float ratio, float idx, float prev_center)
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
        button.ZOrder = 10;
        button.Click += button_Click;
     //   button.MouseHover += button_MouseHover;
      //  button.MouseLeave += button_MouseLeave;

        float center = _panel.Width * idx + button.Width / 2;

        if (level > 0)
        {
            var line = _panel.AddControl<dfSprite>();
            line.SpriteName = "vslider-track-normal";
            line.Width = 12;
            //line.Height = spacing - button.Height;
            float B = spacing - button.Height;
            line.Height = HypotenuseLength(prev_center, center, B);
            Vector3 rot = Vector3.forward;
            float angle = Angle(B, center - prev_center);
            print("Angle: " + angle);
            line.transform.Rotate(rot, angle);
            float padding = 0;
            if (angle < 0)
            {
                padding = -5;
            }
            if (angle > 0)
            {
                padding = 5;
            }
            line.RelativePosition = new Vector3(prev_center, level * spacing - B + padding);
            line.ZOrder = 0;

            var expand = button.AddControl<BrainButton>();
            expand.BackgroundSprite = "dropdown-btn-normal";
            expand.HoverSprite = "dropdown-btn-hover";
            expand.DisabledSprite = "button-disabled";
            expand.PressedSprite = "dropdown-btn-normal";
            //expand.Text = "+";
            expand.Width = 25;
            expand.Height = 25;
            expand.RelativePosition = new Vector3(5, 10);
            expand.Brain = b;
            expand.Click += expand_click;
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

        

        return center;
    }

    private void expand_click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        BrainButton bb = control as BrainButton;
        List<Brain> brains = new List<Brain>() { bb.Brain };
        mouseEvent.Use();
        ExpandedState = true;
        AddBrains(brains);
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
            if (rootBrains.Count == 0)
            {
                Brain b = new Brain();
                b.Name = "New brain";
                b.UserId = ParseUser.CurrentUser.ObjectId;
                b.IsNewBrain = true;
                rootBrains.Add(b);
            }
            AddBrains(rootBrains);
        }
	}

    void AddBrains(List<Brain> brains)
    {
        var children = new List<GameObject>();

        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));

        GameObject.Find("MainPanel").GetComponent<dfPanel>().Enable();

     //   GameObject.Find("LoadingLabel").GetComponent<dfLabel>().IsVisible = false;
        DataLoaded = false;
        int level = 0;
        int idx = 0;
        float ratio = 1.0f / brains.Count;

        foreach (Brain b in brains)
        {
            recurseAdd(b, level, ratio, idx, idx * ratio, 0);
            idx++;
        }
    }

    void recurseAdd(Brain b, int level, float ratio, int idx, float startRatio, float prev_center)
    {
        float center = AddButton(b, level, ratio, startRatio, prev_center);
        print(level + ". " + b.Name + ", Startratio: " + startRatio + ", center: " + center);
        
        if (b.Children.Count > 0)
        {
            int count = b.Children.Count;
            float r = ratio / count;
            int i = 0;
            foreach (Brain child in b.Children)
            {
                recurseAdd(child, level + 1, r, i, startRatio + i * r, center);
                i++;
            }
        }
        
        
    }
}