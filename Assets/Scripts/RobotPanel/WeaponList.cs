using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class WeaponList {

    public static Weapon M_AXE = new Weapon
    {
        Name = "Axe",
        MinimumDamage = 9,
        MaximumDamage = 11,
        AttackSpeed = 1.12f,
        SlowDown = 50,
        Type = WeaponType.Melee
    };

    public static Weapon M_HAMMER = new Weapon
    {
        Name = "Hammer",
        MinimumDamage = 14,
        MaximumDamage = 18,
        AttackSpeed = 0.7f,
        SlowDown = 60,
        Type = WeaponType.Melee
    };

    public static Weapon M_SWORD = new Weapon
    {
        Name = "Sword",
        MinimumDamage = 6,
        MaximumDamage = 10,
        AttackSpeed = 1.40f,
        SlowDown = 40,
        Type = WeaponType.Melee
    };

    public static Dictionary<string, Weapon> WeaponDict = new Dictionary<string, Weapon>()
    {
        { M_AXE.Name, M_AXE },
        { M_HAMMER.Name, M_HAMMER},
        {  M_SWORD.Name, M_SWORD }
    };

}
