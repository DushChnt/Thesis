using UnityEngine;
using System.Collections;
using Parse;
using System.Collections.Generic;

public class WeaponPanel : MonoBehaviour {

    public dfLabel UnlockLabel, ReadyLabel;
    public dfButton WeaponButton;
    public WeaponType WeaponType;
    public Weapon Weapon;
    dfPanel panel;
    dfTweenColor32 colorTween;
    RobotPanel robotPanel;
    SelectWeaponSlot weaponSlot;

    bool Clicked;

    Player Player
    {
        get
        {
            return ParseUser.CurrentUser as Player;
        }
    }

	// Use this for initialization
	void Start () {
        weaponSlot = WeaponButton.GetComponent<SelectWeaponSlot>();
        Player.WeaponChanged += new global::Player.WeaponChangedEventHandler(Player_WeaponChanged);
        robotPanel = transform.parent.GetComponent<RobotPanel>();
        panel = this.GetComponent<dfPanel>();
        colorTween = this.GetComponent<dfTweenColor32>();
        
        Initialize();
	}

    void Player_WeaponChanged(object sender, System.EventArgs e)
    {
        if (Clicked)
        {
            UpdateWeapon();
            Clicked = false; 
        }
    }

    void UpdateWeapon()
    {
        switch (WeaponType)
        {
            case WeaponType.Melee:
                if (Player.MeleeWeapon != null)
                {
                    Weapon = WeaponList.WeaponDict[Player.MeleeWeapon];
                    print("Weapon: " + Weapon.Name);
                    weaponSlot.SetWeapon(Weapon);
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
                    weaponSlot.SetWeapon(Weapon);
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
                    weaponSlot.SetWeapon(Weapon);
                }
                if (Player.Level > 3)
                {
                    Activate();
                }

                break;
        }
    }

    void Initialize()
    {

        WeaponButton.RemoveAllEventHandlers();
        WeaponButton.Click += new MouseEventHandler(panel_Click);
        
        
        Deactivate();
        switch (WeaponType)
        {
            case WeaponType.Melee:
                if (Player.MeleeWeapon != null)
                {
                    Weapon = WeaponList.WeaponDict[Player.MeleeWeapon];
                    weaponSlot.SetWeapon(Weapon);
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
                    weaponSlot.SetWeapon(Weapon);
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
                    weaponSlot.SetWeapon(Weapon);
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
        Clicked = true;
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
        WeaponButton.Hide();
        ReadyLabel.Hide();
    }

    void Activate()
    {
        panel.Click += new MouseEventHandler(panel_Click);
        
        if (Weapon == null)
        {           
            UnlockLabel.Hide();
            WeaponButton.Hide();
            ReadyLabel.Show();
            colorTween.Play();
        }
        else
        {
            UnlockLabel.Hide();
            WeaponButton.Show();
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
