using UnityEngine;
using System.Collections;
using Parse;
using System.Threading.Tasks;
using System;

public class BrainPanelState : MonoBehaviour {

    dfButton _removeButton;
    dfPanel _panel;
    dfLabel _label;
    dfSprite _indicator;
    string name;
    private Color32 grey = new Color32(100, 100, 100, 255);
    private Color32 color;
    Brain _brain;
    public bool InUse;
    public int SlotNo;
    public bool RemoveRemove;
    public dfSprite Indicator;

    Player Player
    {
        get
        {
            return ParseUser.CurrentUser as Player;
        }
    }

	// Use this for initialization
	void Start () {
        this._panel = GetComponent<dfPanel>();
        this._indicator = Indicator;
        try
        {
            this._removeButton = transform.Find("Remove").GetComponent<dfButton>();
            if (RemoveRemove)
            {
                this._removeButton.Hide();
            }
            _removeButton.Click += RemoveButton_Click;
        }
        catch (Exception)
        {

        }

        try
        {
            this._indicator = transform.Find("Indicator").GetComponent<dfSprite>();
        }
        catch (Exception)
        {

        }
        this._label = transform.Find("Label").GetComponent<dfLabel>();
        this.name = gameObject.name;
        
        color = _panel.BackgroundColor;
        _panel.BackgroundColor = grey;

        // print("Init");
        StartCoroutine(Initialize());
	}

    public void ActivatePanel()
    {
       // if (InUse)
        {
            _indicator.Show();
        }
    }

    public dfPanel GetPanel()
    {
        return _panel;
    }

    public void DeactivatePanel()
    {
        _indicator.Hide();
    }

    void RemoveButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        _panel.BackgroundColor = grey;
        _label.Text = "Drag a brain here...";
      //  ParseUser.CurrentUser[GetParseName(name)] = null;
        _brain = null;
        switch (SlotNo)
        {
            case 1:
                Player.Brain1 = _brain;
                break;
            case 2:
                Player.Brain2 = _brain;
                break;
            case 3:
                Player.Brain3 = _brain;
                break;
            case 4:
                Player.Brain4 = _brain;
                break;
        }
        Player.SaveAsync();
        InUse = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Refresh()
    {
        if (InUse)
        {
            _panel.BackgroundColor = color;
        }
        else
        {
            _panel.BackgroundColor = grey;
        }
    }

    public IEnumerator Initialize()
    {
        // print("Init2");
        switch (SlotNo)
        {
            case 1:
                _brain = Player.Brain1;
                break;
            case 2:
                _brain = Player.Brain2;
                break;
            case 3:
                _brain = Player.Brain3;
                break;
            case 4:
                _brain = Player.Brain4;
                break;
        }
        if (_brain == null)
        {
            InUse = false;
    
            Refresh();

        }
        else
        {
            InUse = true;
            _panel.BackgroundColor = color;

            Task fetchTask = _brain.FetchIfNeededAsync();
            // print("Fetching");
            while (!fetchTask.IsCompleted)
            {
                yield return null;
            }
            // print("Brain id: " + _brain.ObjectId);
            // print("Brain name: " + _brain.Name);

            _label.Text = _brain.Name;
        }
    }

    public void AddBrain(Brain brain)
    {
        InUse = true;
        _brain = brain;
        _panel.BackgroundColor = color;
        _label.Text = brain.Name;

        switch (SlotNo)
        {
            case 1:
                Player.Brain1 = _brain;
                break;
            case 2:
                Player.Brain2 = _brain;
                break;
            case 3:
                Player.Brain3 = _brain;
                break;
            case 4:
                Player.Brain4 = _brain;
                break;
        }

     //   print("brain" + SlotNo + " adding " + brain.ObjectId);
      //  ParseUser.CurrentUser["brain" + SlotNo] = brain;
        Player.SaveAsync();
    }

    private string GetParseName(string name)
    {
        return name.ToLower().Replace(" ", "");
    }
}
