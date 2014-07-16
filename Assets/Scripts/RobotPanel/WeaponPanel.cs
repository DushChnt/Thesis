using UnityEngine;
using System.Collections;
using Parse;

public class WeaponPanel : MonoBehaviour {

    public dfLabel UnlockLabel, ReadyLabel;
    public dfSprite WeaponSprite;
    public WeaponType WeaponType;
    public Weapon Weapon;

    Player Player
    {
        get
        {
            return ParseUser.CurrentUser as Player;
        }
    }

	// Use this for initialization
	void Start () {
        Initialize();
	}

    void Initialize()
    {
        
        Deactivate();
        switch (WeaponType)
        {
            case WeaponType.Melee:
                if (Player.Level > 1)
                {
                    Activate();
                }
                break;
            case WeaponType.Ranged:
                if (Player.Level > 2)
                {
                    Activate();
                }
                break;
            case WeaponType.Mortar:
                if (Player.Level > 3)
                {
                    Activate();
                }
                break;
        }
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
        WeaponSprite.Hide();
        ReadyLabel.Hide();
    }

    void Activate()
    {
        
        if (Weapon == null)
        {
            print("Joe?");
            UnlockLabel.Hide();
            WeaponSprite.Hide();
            ReadyLabel.Show();
        }
        else
        {
            print("Joe!");
            UnlockLabel.Hide();
            WeaponSprite.Show();
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
