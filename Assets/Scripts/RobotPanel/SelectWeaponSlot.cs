using UnityEngine;
using System.Collections;
using Parse;

public class SelectWeaponSlot : MonoBehaviour {

    public dfSprite WeaponAvatar;
    public dfLabel NameLabel, DamageLabel, AttackSpeedLabel, SlowdownLabel;
    public dfButton weaponButton;
    Weapon Weapon;

    Player Player
    {
        get
        {
            return ParseUser.CurrentUser as Player;
        }
    }

	// Use this for initialization
	void Start () {
        this.weaponButton = this.GetComponent<dfButton>();
        weaponButton.Click += new MouseEventHandler(weaponButton_Click);
	}

    void weaponButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        if (Weapon != null)
        {
            switch (Weapon.Type)
            {
                case WeaponType.Melee:
                    Player.MeleeWeapon = Weapon.Name;
                    break;
                case WeaponType.Ranged:
                    Player.RangedWeapon = Weapon.Name;
                    break;
                case WeaponType.Mortar:
                    Player.MortarWeapon = Weapon.Name;
                    break;
            }
            Player.PushWeaponChange();
            Player.SaveAsync();
        }
    }

    public void DisableSlot()
    {
        DamageLabel.DisabledColor = new Color32(0, 0, 0, 254);
    }

    public void SetWeapon(Weapon weapon)
    {
        if (weaponButton == null)
        {
            // print("Weapon button is null");
        }
        if (weapon == null)
        {
            // print("NulL");
        }
        else
        {
            // print("Notnull");
        }
        this.Weapon = weapon;
        if (NameLabel == null)
        {
            // print("WTF?");
        }
        WeaponAvatar.SpriteName = weapon.AvatarPath;
        NameLabel.Text = weapon.Name;
        if (weapon.MinimumDamage < weapon.MaximumDamage)
        {
            DamageLabel.Text = string.Format("{0} - {1}", weapon.MinimumDamage, weapon.MaximumDamage);
        }
        else
        {
            DamageLabel.Text = weapon.MaximumDamage + "";
        }
        AttackSpeedLabel.Text = string.Format("{0:0.00} attacks per second", weapon.AttackSpeed);
        SlowdownLabel.Text = string.Format("{0:0} % slowdown", weapon.SlowDown);

        if (Player.MeleeWeapon != null && Player.MeleeWeapon.Equals(weapon.Name) || 
            Player.RangedWeapon != null && Player.RangedWeapon.Equals(weapon.Name) ||
            Player.MortarWeapon != null && Player.MortarWeapon.Equals(weapon.Name))
        {
            weaponButton.Focus();
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
