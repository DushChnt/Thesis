using UnityEngine;
using System.Collections;

public class Weapon  {
    public WeaponType Type { get; set; }
    public float MinimumDamage { get; set; }
    public float MaximumDamage { get; set; }
    public float CoolDown { get; set; }
    public float SlowDown { get; set; }

    public Weapon(WeaponType type, float minDmg, float maxDmg, float coolDown, float slowDown)
    {
        this.Type = type;
        this.MinimumDamage = minDmg;
        this.MaximumDamage = maxDmg;
        this.CoolDown = coolDown;
        this.SlowDown = slowDown;
    }
}
