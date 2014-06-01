using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrainButton : dfButton {

    public bool IsNewBrain = true;
    List<BrainButton> children = new List<BrainButton>();
    public string Id;
    public string ParentId;
    public float OriginalWidth;
    public Brain Brain;

	
}
