using UnityEngine;
using System.Collections;
using Parse;
using System.Collections.Generic;

public class Player : ParseUser {

    [ParseFieldName("isOnline")]
    public bool IsOnline
    {
        get { return GetProperty<bool>("IsOnline"); }
        set { SetProperty<bool>(value, "IsOnline"); }
    }

    [ParseFieldName("pinnedPlayers")]
    public IList<Player> PinnedPlayers
    {
        get { return GetProperty<IList<Player>>("PinnedPlayers"); }
        set { SetProperty<IList<Player>>(value, "PinnedPlayers"); }
    }

    [ParseFieldName("brain1")]
    public Brain Brain1
    {
        get { return GetProperty<Brain>("Brain1"); }
        set { SetProperty<Brain>(value, "Brain1"); }
    }

    [ParseFieldName("brain2")]
    public Brain Brain2
    {
        get { return GetProperty<Brain>("Brain2"); }
        set { SetProperty<Brain>(value, "Brain2"); }
    }

    [ParseFieldName("brain3")]
    public Brain Brain3
    {
        get { return GetProperty<Brain>("Brain3"); }
        set { SetProperty<Brain>(value, "Brain3"); }
    }

    [ParseFieldName("brain4")]
    public Brain Brain4
    {
        get { return GetProperty<Brain>("Brain4"); }
        set { SetProperty<Brain>(value, "Brain4"); }
    }
}
