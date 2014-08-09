using UnityEngine;
using System.Collections;
using Parse;

[ParseClassName("BattleEvent")]
public class BattleEvent : ParseObject {

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

    [ParseFieldName("action")]
    public string Action
    {
        get { return GetProperty<string>("Action"); }
        set { SetProperty<string>(value, "Action"); }
    }

    [ParseFieldName("damageGiven")]
    public float DamageGiven
    {
        get { return GetProperty<float>("DamageGiven"); }
        set { SetProperty<float>(value, "DamageGiven"); }
    }

    [ParseFieldName("damageTaken")]
    public float DamageTaken
    {
        get { return GetProperty<float>("DamageTaken"); }
        set { SetProperty<float>(value, "DamageTaken"); }
    }

    [ParseFieldName("currentHealth")]
    public float CurrentHealth
    {
        get { return GetProperty<float>("CurrentHealth"); }
        set { SetProperty<float>(value, "CurrentHealth"); }
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
}
