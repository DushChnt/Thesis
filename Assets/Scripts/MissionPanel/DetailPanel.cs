using UnityEngine;
using System.Collections;

public class DetailPanel : MonoBehaviour {

    public dfButton TestButton;
    public dfLabel MissionTextLabel, TitleLabel;
    dfPanel panel;
    public dfPanel MainPanel;
    public TrainingDialogScript BrainsDialog;

    Player Player
    {
        get
        {
            return Parse.ParseUser.CurrentUser as Player;
        }
    }

    #region LONG_STRINGS

    string mission1Text = @"Train your robot to move around. 

Do so by adjusting the correct sliders for the brain that you are training.

In order to complete the mission you must create two brains; a brain that can approach the target and a brain that can flee from the target.";
    string mission2Text = @"Train your robot to use it's melee attack. 

In this mission you will need to be able to hit the opponent with melee attacks and beat the opponent in a battle.";

    string mission3Text = @"Train your robot to use it's ranged attack.

In this mission you have to be able to hit a moving target with your ranged weapon and then beat the opponent in a battle using both melee and ranged attacks.";

    string mission4Text = @"Train your robot to use it's mortar attack.

In this mission you must be able to hit the opponent with your mortar attack and then beat the opponent using all your combat skills.";

    string mission5Text = @"Boss battle!

By using your combined combat skills you must defeat the opponent before he defeats you.";

    #endregion

    // Use this for initialization
	void Start () {
        panel = GetComponent<dfPanel>();
        TestButton.Click += new MouseEventHandler(TestButton_Click);
        
	}

    void TestButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        if (TestBrains())
        {
            if (TestWeapons())
            {

                Application.LoadLevel(string.Format("Mission {0}", PlayerPrefs.GetInt(MissionPanel.CURRENT_MISSION, 1)));
            }
            else
            {
                MainPanel.Disable();
                BrainsDialog.ShowDialog("You have to choose a weapon before continuing. Click on the flashing slot to the right to assign.");
                BrainsDialog.panel.Click += new MouseEventHandler(panel_Click);
            }
        }
        else
        {
            MainPanel.Disable();
            BrainsDialog.ShowDialog("You haven't chosen any brains to use for battle! Drag the brains you wish to use down to the selected brains panel.");
            BrainsDialog.panel.Click += new MouseEventHandler(panel_Click);
        }
    }

    bool TestWeapons()
    {
        bool ok = true;
        // Test for selected weapons
        if (Player.Level > 1)
        {
            if (Player.MeleeWeapon == null || Player.MeleeWeapon.Equals(""))
            {
                ok = false;
            }
            if (Player.Level > 2)
            {
                if (Player.RangedWeapon == null || Player.RangedWeapon.Equals(""))
                {
                    ok = false;
                }
                if (Player.Level > 3)
                {
                    if (Player.MortarWeapon == null || Player.MortarWeapon.Equals(""))
                    {
                        ok = false;
                    }
                }
            }
        }
        return ok;
    }

    bool TestBrains()
    {
        bool ok = Player.Brain1 != null || Player.Brain2 != null || Player.Brain3 != null || Player.Brain4 != null;
        return ok;
    }

    void panel_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        MainPanel.Enable();
    }

   

    public void DisplayMission(int mission, string title)
    {
        switch (mission)
        {
            case 1:
                MissionTextLabel.Text = mission1Text;
                break;
            case 2:
                MissionTextLabel.Text = mission2Text;
                break;
            case 3:
                MissionTextLabel.Text = mission3Text;
                break;
            case 4:
                MissionTextLabel.Text = mission4Text;
                break; 
            case 5:
                MissionTextLabel.Text = mission5Text;
                break;
        }
     
        TitleLabel.Text = title;     
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
