using UnityEngine;
using System.Collections;
using Parse;

public class Player : ParseUser {

    

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
