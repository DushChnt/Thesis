using UnityEngine;
using System.Collections;
using System.Xml;
using Parse;
using System;
using System.IO;

public class OptimizerParameters {
    const string __OptimizerParameters = "OptimizerParameters";
    const string __NumInputs = "NumInputs";
    const string __NumOutputs = "NumOutputs";
    const string __WMeleeAttack = "WMeleeAttack";
    const string __WMortarAttack = "WMortarAttack";
    const string __WMortarHits = "WMortarHits";
    const string __WMortarPrecision = "WMortarPrecision";
    const string __WMortarDamage = "WMortarDamage";
    const string __WMortarDamagePerHit = "WMortarDamagePerHit";
    const string __WApproach = "WApproach";
    const string __WRifleAttack = "WRifleAttack";
    const string __WRifleHits = "WRifleHits";
    const string __WAngleTowards = "WAngleTowards";
    const string __WRiflePrecision = "WRiflePrecision";
    const string __TargetMoveStrategy = "TargetMoveStrategy";
    const string __DistanceToKeep = "DistanceToKeep";
    const string __MultipleTargets = "MultipleTargets";
    const string __Name = "Name";
    const string __Description = "Description";
    const string __WTurretAngleTowards = "WTurretAngleTowards";

    public static int NumInputs = 11;
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
    public static string Description = "Description...";
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

    public static void WriteXML()
    {
        XmlWriterSettings _xwSettings = new XmlWriterSettings();
        _xwSettings.Indent = true;
        string user = "Joe";
        try
        {
            if (ParseUser.CurrentUser != null)
            {
                user = ParseUser.CurrentUser.Username;
                
            }
        }
        catch (Exception e)
        {
            Utility.Log(e.StackTrace);
        }
        string folderPath = Application.persistentDataPath + string.Format("/{0}", user);
        DirectoryInfo dirInf = new DirectoryInfo(folderPath);
        if (!dirInf.Exists)
        {
            Debug.Log("Creating subdirectory");
            dirInf.Create();
        }
        string filePath = folderPath + string.Format("/{0}.settings.xml", Name);

        using (XmlWriter writer = XmlWriter.Create(filePath, _xwSettings))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement(__OptimizerParameters);

            writer.WriteElementString(__NumInputs, "" + NumInputs);
            writer.WriteElementString(__NumOutputs, "" + NumOutputs);
            writer.WriteElementString(__WMeleeAttack, "" + WMeleeAttack);
            writer.WriteElementString(__WMortarAttack, "" + WMortarAttack);
            writer.WriteElementString(__WMortarHits, "" + WMortarHits);
            writer.WriteElementString(__WMortarPrecision, "" + WMortarPrecision);
            writer.WriteElementString(__WMortarDamage, "" + WMortarDamage);
            writer.WriteElementString(__WMortarDamagePerHit, "" + WMortarDamagePerHit);
            writer.WriteElementString(__WApproach, "" + WApproach);
            writer.WriteElementString(__WRifleAttack, "" + WRifleAttack);
            writer.WriteElementString(__WRifleHits, "" + WRifleHits);
            writer.WriteElementString(__WAngleTowards, "" + WAngleTowards);
            writer.WriteElementString(__WRiflePrecision, "" + WRiflePrecision);
            writer.WriteElementString(__TargetMoveStrategy, "" + TargetMoveStrategy);
            writer.WriteElementString(__DistanceToKeep, "" + DistanceToKeep);
            writer.WriteElementString(__MultipleTargets, "" + MultipleTargets);
            writer.WriteElementString(__Name, "" + Name);
            writer.WriteElementString(__Description, "" + Description);
            writer.WriteElementString(__WTurretAngleTowards, "" + WTurretAngleTowards);

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
    }
}

public enum TargetMove
{
    Stationary, Random, Simple, Advanced
}
