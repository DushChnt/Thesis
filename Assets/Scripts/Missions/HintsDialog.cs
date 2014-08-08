using UnityEngine;
using System.Collections;

public class HintsDialog : MonoBehaviour {

    public dfButton CloseButton;
    public dfLabel TitleLabel;
    public dfRichTextLabel InfoLabel;
    public dfPanel Panel;

    #region LONG_STRINGS
    string mission1 = @"<b>Welcome to your first mission. In order to complete the mission you must teach your robot to approach a target and to flee from a target.</b>
<br><br>
In the <span style=""color: #99ff99"">Focus areas</span> panel you can choose which behaviours your robot should learn by either rewarding or punishing the robot for doing them. Mouse over the names to find out what they do. 
<br><br>
In the <span style=""color: #0099ff"">Target behaviour</span> panel you can select how the target should move around. It is easiest to start with a stationary target. If you want the target to follow your robot, select Multiple targets.
<br><br>
Click on ? to bring up this dialog again.";

    string mission2 = @"<b>You have now unlocked the Melee focus areas. These all relate to your robot's close combat skills.</b>

<ul>
<li>Attacks: Counts attacks no matter where the target is.</li>
<li>Hits: Only attacks that hit the target counts.</li>
<li>Precision: Don't do attacks that does not hit the target!</li>
</ul>
<br><br>
Remember that each attack slows down the robot according to the slowdown of the selected weapon.
<br><br>
<span style=""color: green""><b>Tip:</b></span> It is a good idea to base your melee attack brains on brains that can approach already. Generally it is a good idea to base advanced brains on simple brains.
<br><br>
Click on ? to bring up this dialog again.";

    string mission3 = @"<b>You have now unlocked the Ranged focus areas. These all relate to your robot's ranged combat skills.</b>
<br>
<ul>
<li>Attacks: Counts shots no matter where the target is.</li>
<li>Hits: Only shots that hit the target counts.</li>
<li>Precision: Don't shoot if you do not hit the target!</li>
</ul>
<br><br>
<span style=""color: green""><b>Tip:</b></span> Don't select too many focus areas at once. Gradually change the focus areas, e.g. first train movement, then train attacks and then train hits and precision.
<br><br>
<span style=""color: green""><b>Tip 2:</b></span> Bigger targets are easier to hit, smaller targets give more precision.
<br><br>
Click on ? to bring up this dialog again.";

    string mission4 = @"<b>You have now unlocked the Mortar focus areas. These all relate to your robot's mortar combat skills.</b>
<br>
<ul>
<li>Aim turret: Aim the turret towards the target.</li>
<li>Attacks: Counts shots no matter where the target is.</li>
<li>Hits: Only shots that hit the target counts.</li>
<li>Precision: Don't shoot if you do not hit the target!</li>
<li>Damage per attack: Closer impacts means more damage.</li>
</ul>
<br><br>
<span style=""color: green""><b>Tip:</b></span> Don't select too many focus areas at once. Gradually change the focus areas, e.g. first train aiming, then train attacks, then train hits and precision and finally train damage per attack.
<br><br>
Click on ? to bring up this dialog again.";

    string mission5 = @"<b>Your robot is fully upgraded and has learned all the different combat skills. Now you must fine tune your robot to be able to beat the final mission.</b>
<br><br>
<span style=""color: green""><b>Tip:</b></span> It is a good idea to base your melee attack brains on brains that can approach already. Generally it is a good idea to base advanced brains on simple brains.
<br><br>
<span color=""green""><b>Tip 2:</b></span> Don't select too many focus areas at once. Gradually change the focus areas, e.g. first train aiming, then train attacks, then train hits and precision and finally train damage per attack.
<br><br>
<span style=""color: green""><b>Tip 3:</b></span> Bigger targets are easier to hit, smaller targets give more precision.
<br><br>.";
    #endregion

    // Use this for initialization
	void Start () {
        Panel = this.GetComponent<dfPanel>();
        int mission = PlayerPrefs.GetInt(MissionPanel.CURRENT_MISSION, 1);
        CloseButton.Click += new MouseEventHandler(CloseButton_Click);
        bool seen = PlayerPrefs.GetInt(string.Format("Mission{0}Seen", mission), 0) == 1;
        if (seen)
        {
            Panel.Hide();         
        }
        else
        {
            AssignText();
        }
	}

    public void ShowDialog()
    {
        AssignText();
        Panel.Show();
    }

    void AssignText()
    {
        int mission = PlayerPrefs.GetInt(MissionPanel.CURRENT_MISSION, 1);
        
        

        PlayerPrefs.SetInt(string.Format("Mission{0}Seen", mission), 1);
        InfoLabel.ResetLayout();
        switch (mission)
        {
            case 1:
                TitleLabel.Text = "Mission 1: Movement";
                InfoLabel.Text = mission1;
                break;
            case 2:
                TitleLabel.Text = "Mission 2: Close combat";
                InfoLabel.Text = mission2;
                break;
            case 3:
                TitleLabel.Text = "Mission 3: Ranged attacks";
                InfoLabel.Text = mission3;
                break;
            case 4:
                TitleLabel.Text = "Mission 4: Mortar attacks";
                InfoLabel.Text = mission4;
                break;
            case 5:
                TitleLabel.Text = "Mission 5: Final test";
                InfoLabel.Text = mission5;
                break;
        }
    }

    void CloseButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Panel.Hide();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
