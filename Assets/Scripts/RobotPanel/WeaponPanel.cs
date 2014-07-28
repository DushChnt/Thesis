using UnityEngine;
using System.Collections;
using Parse;
using System.Collections.Generic;

public class WeaponPanel : MonoBehaviour {

    public dfLabel UnlockLabel, ReadyLabel;
    public SelectWeaponSlot WeaponButton;
    public WeaponType WeaponType;
    public Weapon Weapon;
    dfPanel panel;
    dfTweenColor32 colorTween;
    RobotPanel robotPanel;

    Player Player
    {
        get
        {
            return ParseUser.CurrentUser as Player;
        }
    }

	// Use this for initialization
	void Start () {
        Player.WeaponChanged += new global::Player.WeaponChangedEventHandler(Player_WeaponChanged);
        robotPanel = transform.parent.GetComponent<RobotPanel>();
        panel = this.GetComponent<dfPanel>();
        colorTween = this.GetComponent<dfTweenColor32>();
        Initialize();
	}

    void Player_WeaponChanged(object sender, System.EventArgs e)
    {
        Initialize();
    }

    void Initialize()
    {

        WeaponButton.weaponButton.RemoveAllEventHandlers();
        WeaponButton.weaponButton.Click += new MouseEventHandler(panel_Click);
        
        
        Deactivate();
        switch (WeaponType)
        {
            case WeaponType.Melee:
                if (Player.MeleeWeapon != null)
                {
                    Weapon = WeaponList.WeaponDict[Player.MeleeWeapon];
                    WeaponButton.SetWeapon(Weapon);
                }
                if (Player.Level > 1)
                {
                    Activate();
                }
                
                break;
            case WeaponType.Ranged:
                if (Player.RangedWeapon != null)
                {
                    Weapon = WeaponList.WeaponDict[Player.RangedWeapon];
                    WeaponButton.SetWeapon(Weapon);
                }
                if (Player.Level > 2)
                {
                    Activate();
                }
                
                break;
            case WeaponType.Mortar:
                if (Player.MortarWeapon != null)
                {
                    Weapon = WeaponList.WeaponDict[Player.MortarWeapon]; 
                    WeaponButton.SetWeapon(Weapon);
                }
                if (Player.Level > 3)
                {
                    Activate();
                }
               
                break;
        }
    }

    void panel_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        robotPanel.ShowSelectWeapon(WeaponType);
    }

    void Deactivate()
    {
        int lvl = 2;
        switch (WeaponType)
        {
            case WeaponType.Melee:
                lvl = 2;
                break;
            case WeaponType.Ranged:
                lvl = 3;
                break;
            case WeaponType.Mortar:
                lvl = 4;
                break;
        }
        UnlockLabel.Text = "Unlock in level " + lvl;
        UnlockLabel.Show();
        WeaponButton.weaponButton.Hide();
        ReadyLabel.Hide();
    }

    void Activate()
    {
        panel.Click += new MouseEventHandler(panel_Click);
        
        if (Weapon == null)
        {           
            UnlockLabel.Hide();
            WeaponButton.weaponButton.Hide();
            ReadyLabel.Show();
            colorTween.Play();
        }
        else
        {
            UnlockLabel.Hide();
            WeaponButton.weaponButton.Show();
            ReadyLabel.Hide();
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}

public enum WeaponType
{
    Melee, Ranged, Mortar
}
