using UnityEngine;
using System.Collections;
using Parse;
using System.Collections.Generic;

[ParseClassName("Match")]
public class Match : ParseObject
{
    [ParseFieldName("opponent")]
    public Player Opponent
    {
        get { return GetProperty<Player>("Opponent"); }
        set { SetProperty<Player>(value, "Opponent"); }
    }

    [ParseFieldName("won")]
    public bool Won
    {
        get { return GetProperty<bool>("Won"); }
        set { SetProperty<bool>(value, "Won"); }
    }

    [ParseFieldName("frames")]
    public IList<ParseObject> Frames
    {
        get { return GetProperty<IList<ParseObject>>("Frames"); }
        set { SetProperty<IList<ParseObject>>(value, "Frames"); }
    }

    [ParseFieldName("player")]
    public Player Player
    {
        get { return GetProperty<Player>("Player"); }
        set { SetProperty<Player>(value, "Player"); }
    }

    public Match()
    {
        this.Player = ParseUser.CurrentUser as Player;
    }
}
