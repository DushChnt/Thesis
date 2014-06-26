using UnityEngine;
using System.Collections;
using Parse;

[ParseClassName("Frame")]
public class Frame : ParseObject 
{
    [ParseFieldName("damageTaken")]
    public float DamageTaken
    {
        get { return GetProperty<float>("DamageTaken"); }
        set { SetProperty<float>(value, "DamageTaken"); }
    }

    [ParseFieldName("damageGiven")]
    public float DamageGiven
    {
        get { return GetProperty<float>("DamageGiven"); }
        set { SetProperty<float>(value, "DamageGiven"); }
    }

    [ParseFieldName("time")]
    public float Time
    {
        get { return GetProperty<float>("Time"); }
        set { SetProperty<float>(value, "Time"); }
    }

    [ParseFieldName("outcome")]
    public string Outcome
    {
        get { return GetProperty<string>("Outcome"); }
        set { SetProperty<string>(value, "Outcome"); }
    }

    [ParseFieldName("match")]
    public ParseObject Match
    {
        get { return GetProperty<ParseObject>("Match"); }
        set { SetProperty<ParseObject>(value, "Match"); }
    }

    [ParseFieldName("player")]
    public Player Player
    {
        get { return GetProperty<Player>("Player"); }
        set { SetProperty<Player>(value, "Player"); }
    }

    public Frame()
    {
        this.Player = ParseUser.CurrentUser as Player;
    }
}
