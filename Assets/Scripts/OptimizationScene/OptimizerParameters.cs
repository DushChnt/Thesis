using UnityEngine;
using System.Collections;

public class OptimizerParameters {

    public static int NumInputs = 6;
    public static int NumOutputs = 3;

    // Fitness weights
    public static float WMeleeAttack = 1f;
    public static float WRangedAttack = 0;
    public static float WApproach = 5f;

    public static TargetMove TargetMoveStrategy = TargetMove.Simple;

    public static string Name = "SimpleTest";
    public static string ConfigFile = @"Assets\Scripts\phototaxis.config.xml";

    public static void Reset()
    {
        WMeleeAttack = 0;
        WRangedAttack = 0;
        WApproach = 0;

        TargetMoveStrategy = TargetMove.Stationary;
        Name = "Test";
    }
}

public enum TargetMove
{
    Stationary, Random, Simple, Advanced
}
