using UnityEngine;
using System.Collections;

public class Weapon  {
    public WeaponType Type { get; set; }
    public float MinimumDamage { get; set; }
    public float MaximumDamage { get; set; }
    public float AttackSpeed { get; set; }
    public float SlowDown { get; set; }
    public string Name { get; set; }
    public string AvatarPath { get; set; }

    public Weapon()
    {

    }

    public Weapon(string name, WeaponType type, float minDmg, float maxDmg, float coolDown, float slowDown, string avatar)
    {
        this.Name = name;
        this.Type = type;
        this.MinimumDamage = minDmg;
        this.MaximumDamage = maxDmg;
        this.AttackSpeed = coolDown;
        this.SlowDown = slowDown;
        this.AvatarPath = avatar;
    }
}
