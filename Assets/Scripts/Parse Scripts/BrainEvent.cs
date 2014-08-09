using UnityEngine;
using System.Collections;
using Parse;

[ParseClassName("BrainEvent")]
public class BrainEvent : ParseObject
{

    [ParseFieldName("player")]
    public Player Player
    {
        get { return GetProperty<Player>("Player"); }
        set { SetProperty<Player>(value, "Player"); }
    }

    [ParseFieldName("frame")]
    public Frame Frame
    {
        get { return GetProperty<Frame>("Frame"); }
        set { SetProperty<Frame>(value, "Frame"); }
    }

    [ParseFieldName("arena")]
    public string Arena
    {
        get { return GetProperty<string>("Arena"); }
        set { SetProperty<string>(value, "Arena"); }
    }

    [ParseFieldName("activeBrain")]
    public Brain ActiveBrain
    {
        get { return GetProperty<Brain>("ActiveBrain"); }
        set { SetProperty<Brain>(value, "ActiveBrain"); }
    }

    [ParseFieldName("meleeAttacks")]
    public int MeleeAttacks
    {
        get { return GetProperty<int>("MeleeAttacks"); }
        set { SetProperty<int>(value, "MeleeAttacks"); }
    }

    [ParseFieldName("meleeHits")]
    public int MeleeHits
    {
        get { return GetProperty<int>("MeleeHits"); }
        set { SetProperty<int>(value, "MeleeHits"); }
    }

    [ParseFieldName("meleeDamage")]
    public float MeleeDamage
    {
        get { return GetProperty<float>("MeleeDamage"); }
        set { SetProperty<float>(value, "MeleeDamage"); }
    }

    [ParseFieldName("rangedAttacks")]
    public int RangedAttacks
    {
        get { return GetProperty<int>("RangedAttacks"); }
        set { SetProperty<int>(value, "RangedAttacks"); }
    }

    [ParseFieldName("rangedHits")]
    public int RangedHits
    {
        get { return GetProperty<int>("RangedHits"); }
        set { SetProperty<int>(value, "RangedHits"); }
    }

    [ParseFieldName("rangedDamage")]
    public float RangedDamage
    {
        get { return GetProperty<float>("RangedDamage"); }
        set { SetProperty<float>(value, "RangedDamage"); }
    }

    [ParseFieldName("mortarAttacks")]
    public int MortarAttacks
    {
        get { return GetProperty<int>("MortarAttacks"); }
        set { SetProperty<int>(value, "MortarAttacks"); }
    }

    [ParseFieldName("mortarHits")]
    public int MortarHits
    {
        get { return GetProperty<int>("MortarHits"); }
        set { SetProperty<int>(value, "MortarHits"); }
    }

    [ParseFieldName("mortarDamage")]
    public float MortarDamage
    {
        get { return GetProperty<float>("MortarDamage"); }
        set { SetProperty<float>(value, "MortarDamage"); }
    }

    [ParseFieldName("mortarOwnDamage")]
    public float MortarOwnDamage
    {
        get { return GetProperty<float>("MortarOwnDamage"); }
        set { SetProperty<float>(value, "MortarOwnDamage"); }
    }

    [ParseFieldName("damageTaken")]
    public float DamageTaken
    {
        get { return GetProperty<float>("DamageTaken"); }
        set { SetProperty<float>(value, "DamageTaken"); }
    }

    [ParseFieldName("playerHealth")]
    public float PlayerHealth
    {
        get { return GetProperty<float>("PlayerHealth"); }
        set { SetProperty<float>(value, "PlayerHealth"); }
    }

    [ParseFieldName("opponentHealth")]
    public float OpponentHealth
    {
        get { return GetProperty<float>("OpponentHealth"); }
        set { SetProperty<float>(value, "OpponentHealth"); }
    }

    [ParseFieldName("time")]
    public float Time
    {
        get { return GetProperty<float>("Time"); }
        set { SetProperty<float>(value, "Time"); }
    }

    [ParseFieldName("duration")]
    public float Duration
    {
        get { return GetProperty<float>("Duration"); }
        set { SetProperty<float>(value, "Duration"); }
    }
}
