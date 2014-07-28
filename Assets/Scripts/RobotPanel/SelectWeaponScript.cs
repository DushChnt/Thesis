using UnityEngine;
using System.Collections;

public class SelectWeaponScript : MonoBehaviour {

    public SelectWeaponSlot Weapon1, Weapon2, Weapon3;
    dfPanel panel;

	// Use this for initialization
	void Start () {
        panel = this.GetComponent<dfPanel>();

        
	}

    void weaponButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        panel.Hide();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private void ShowMeleeWeapons()
    {
        Weapon1.SetWeapon(WeaponList.M_AXE);
        Weapon2.SetWeapon(WeaponList.M_HAMMER);
        Weapon3.SetWeapon(WeaponList.M_SWORD);
    }

    public void ShowSelectWeapons(WeaponType type)
    {
        Weapon1.weaponButton.Click += new MouseEventHandler(weaponButton_Click);
        Weapon2.weaponButton.Click += new MouseEventHandler(weaponButton_Click);
        Weapon3.weaponButton.Click += new MouseEventHandler(weaponButton_Click);
        panel.Show();
        switch (type)
        {
            case WeaponType.Melee:
                ShowMeleeWeapons();
                break;
            case WeaponType.Ranged:

                break;
            case WeaponType.Mortar:

                break;
        }
    }
}
