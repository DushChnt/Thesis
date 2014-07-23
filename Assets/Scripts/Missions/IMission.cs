using UnityEngine;
using System.Collections;
using System;

public interface IMission {

    event EventHandler MissionComplete;
    event EventHandler SubMissionComplete;

}
