using UnityEngine;
using System.Collections;

public class OptimizerParameters {

    public static int NumInputs = 9;
    public static int NumOutputs = 6;

    // Fitness weights
    public static float WMeleeAttack = 1f;
    public static float WMortarAttack = 0;
    public static float WMortarHits = 0;
    public static float WMortarPrecision = 0f;
    public static float WMortarDamage = 0f;
    public static float WMortarDamagePerHit = 0f;
    public static float WApproach = 5f;
    public static float WRifleAttack = 0f;
    public static float WRifleHits = 0f;
    public static float WAngleTowards = 0f;
    public static float WRiflePrecision = 0f;

    public static TargetMove TargetMoveStrategy = TargetMove.Simple;
    public static float DistanceToKeep = 0f;

    public static bool MultipleTargets = false;

    public static string Name = "SimpleTest";
    public static string ConfigFile = @"Assets\Scripts\phototaxis.config.xml";
    public static float WTurretAngleTowards = 0f;

    public static void Reset()
    {
        WMeleeAttack = 0;
        WMortarAttack = 0;
        WMortarHits = 0;
        WMortarPrecision = 0f;
        WMortarDamagePerHit = 0f;
        WMortarDamage = 0f;
        WApproach = 0;
        WRifleAttack = 0f;
        WRifleHits = 0f;
        WAngleTowards = 0f;
        WTurretAngleTowards = 0f;
        WRiflePrecision = 0f;
        DistanceToKeep = 0f;
        MultipleTargets = false;

        TargetMoveStrategy = TargetMove.Stationary;
        Name = "Test";
    }
}

public enum TargetMove
{
    Stationary, Random, Simple, Advanced
}
