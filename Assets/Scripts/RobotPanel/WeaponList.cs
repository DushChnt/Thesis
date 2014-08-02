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

    public static Weapon R_CROSSBOW = new Weapon
    {
        Name = "Crossbow",
        MinimumDamage = 2,
        MaximumDamage = 4,
        AttackSpeed = 2.00f,
        SlowDown = 10,
        Type = WeaponType.Ranged
    };

    public static Weapon R_RIFLE = new Weapon
    {
        Name = "Rifle",
        MinimumDamage = 2,
        MaximumDamage = 3,
        AttackSpeed = 2.40f,
        SlowDown = 8,
        Type = WeaponType.Ranged
    };

    public static Weapon R_PHASERGUN = new Weapon
    {
        Name = "Phase gun",
        MinimumDamage = 1,
        MaximumDamage = 1,
        AttackSpeed = 6.00f,
        SlowDown = 3,
        Type = WeaponType.Ranged
    };

    public static Weapon C_BOMB = new Weapon
    {
        Name = "Bomb",
        MinimumDamage = 1,
        MaximumDamage = 30,
        AttackSpeed = 0.5f,
        SlowDown = 90,
        Type = WeaponType.Mortar
    };

    public static Weapon C_FIREBALL = new Weapon
    {
        Name = "Fireball",
        MinimumDamage = 1,
        MaximumDamage = 20,
        AttackSpeed = 0.75f,
        SlowDown = 80,
        Type = WeaponType.Mortar
    };

    public static Weapon C_DESTROYER = new Weapon
    {
        Name = "Destroyer",
        MinimumDamage = 1,
        MaximumDamage = 60,
        AttackSpeed = 0.25f,
        SlowDown = 100,
        Type = WeaponType.Mortar
    };

    public static Dictionary<string, Weapon> WeaponDict = new Dictionary<string, Weapon>()
    {
        { M_AXE.Name, M_AXE },
        { M_HAMMER.Name, M_HAMMER},
        {  M_SWORD.Name, M_SWORD },
        { R_CROSSBOW.Name, R_CROSSBOW },
        { R_RIFLE.Name, R_RIFLE },
        { R_PHASERGUN.Name, R_PHASERGUN },
        { C_BOMB.Name, C_BOMB },
        { C_FIREBALL.Name, C_FIREBALL },
        { C_DESTROYER.Name, C_DESTROYER }
    };

}
