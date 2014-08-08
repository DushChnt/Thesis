using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;

[ParseClassName("Brain")]
public class Brain : ParseObject {

    public const string ADVANCED = "advanced";
    public const string SIMPLE = "simple";
    public const string BATTLE = "battle";

    [ParseFieldName("name")]
    public string Name
    {
        get { return GetProperty<string>("Name"); }
        set { SetProperty<string>(value, "Name"); }
    }

    [ParseFieldName("parentId")]
    public string ParentId
    {
        get { return GetProperty<string>("ParentId"); }
        set { SetProperty<string>(value, "ParentId"); }
    }

    [ParseFieldName("description")]
    public string Description
    {
        get { return GetProperty<string>("Description"); }
        set { SetProperty<string>(value, "Description"); }
    }

    [ParseFieldName("userId")]
    public string UserId
    {
        get { return GetProperty<string>("UserId"); }
        set { SetProperty<string>(value, "UserId"); }
    }

    [ParseFieldName("numInputs")]
    public int NumInputs
    {
        get { return GetProperty<int>("NumInputs"); }
        set { SetProperty<int>(value, "NumInputs"); }
    }

    [ParseFieldName("numOutputs")]
    public int NumOutputs
    {
        get { return GetProperty<int>("NumOutputs"); }
        set { SetProperty<int>(value, "NumOutputs"); }
    }

    [ParseFieldName("moveAround")]
    public float MoveAround
    {
        get { return GetProperty<float>("MoveAround"); }
        set { SetProperty<float>(value, "MoveAround"); }
    }

    [ParseFieldName("reachDistance")]
    public float ReachDistance
    {
        get { return GetProperty<float>("ReachDistance"); }
        set { SetProperty<float>(value, "ReachDistance"); }
    }

    [ParseFieldName("keepDistance")]
    public float KeepDistance
    {
        get { return GetProperty<float>("KeepDistance"); }
        set { SetProperty<float>(value, "KeepDistance"); }
    }

    [ParseFieldName("distanceToKeep")]
    public float DistanceToKeep
    {
        get { return GetProperty<float>("DistanceToKeep"); }
        set { SetProperty<float>(value, "DistanceToKeep"); }
    }

    [ParseFieldName("faceTarget")]
    public float FaceTarget
    {
        get { return GetProperty<float>("FaceTarget"); }
        set { SetProperty<float>(value, "FaceTarget"); }
    }

    [ParseFieldName("turretFaceTarget")]
    public float TurretFaceTarget
    {
        get { return GetProperty<float>("TurretFaceTarget"); }
        set { SetProperty<float>(value, "TurretFaceTarget"); }
    }

    [ParseFieldName("meleeAttacks")]
    public float MeleeAttacks
    {
        get { return GetProperty<float>("MeleeAttacks"); }
        set { SetProperty<float>(value, "MeleeAttacks"); }
    }

    [ParseFieldName("meleeHits")]
    public float MeleeHits
    {
        get { return GetProperty<float>("MeleeHits"); }
        set { SetProperty<float>(value, "MeleeHits"); }
    }

    [ParseFieldName("meleePrecision")]
    public float MeleePrecision
    {
        get { return GetProperty<float>("MeleePrecision"); }
        set { SetProperty<float>(value, "MeleePrecision"); }
    }

    [ParseFieldName("rifleAttacks")]
    public float RifleAttacks
    {
        get { return GetProperty<float>("RifleAttacks"); }
        set { SetProperty<float>(value, "RifleAttacks"); }
    }

    [ParseFieldName("rifleHits")]
    public float RifleHits
    {
        get { return GetProperty<float>("RifleHits"); }
        set { SetProperty<float>(value, "RifleHits"); }
    }

    [ParseFieldName("riflePrecision")]
    public float RiflePrecision
    {
        get { return GetProperty<float>("RiflePrecision"); }
        set { SetProperty<float>(value, "RiflePrecision"); }
    }

    [ParseFieldName("mortarAttacks")]
    public float MortarAttacks
    {
        get { return GetProperty<float>("MortarAttacks"); }
        set { SetProperty<float>(value, "MortarAttacks"); }
    }

    [ParseFieldName("mortarHits")]
    public float MortarHits
    {
        get { return GetProperty<float>("MortarHits"); }
        set { SetProperty<float>(value, "MortarHits"); }
    }

    [ParseFieldName("mortarPrecision")]
    public float MortarPrecision
    {
        get { return GetProperty<float>("MortarPrecision"); }
        set { SetProperty<float>(value, "MortarPrecision"); }
    }

    [ParseFieldName("mortarDamagePerHit")]
    public float MortarDamagePerHit
    {
        get { return GetProperty<float>("MortarDamagePerHit"); }
        set { SetProperty<float>(value, "MortarDamagePerHit"); }
    }

    [ParseFieldName("targetBehaviorMovement")]
    public string TargetBehaviorMovement
    {
        get { return GetProperty<string>("TargetBehaviorMovement"); }
        set { SetProperty<string>(value, "TargetBehaviorMovement"); }
    }

    [ParseFieldName("fitnessMode")]
    public string FitnessMode
    {
        get { return GetProperty<string>("FitnessMode"); }
        set { SetProperty<string>(value, "FitnessMode"); }
    }

    [ParseFieldName("bestFitness")]
    public float BestFitness
    {
        get { return GetProperty<float>("BestFitness"); }
        set { SetProperty<float>(value, "BestFitness"); }
    }

    [ParseFieldName("totalTime")]
    public float TotalTime
    {
        get { return GetProperty<float>("TotalTime"); }
        set { SetProperty<float>(value, "TotalTime"); }
    }

    [ParseFieldName("generation")]
    public int Generation
    {
        get { return GetProperty<int>("Generation"); }
        set { SetProperty<int>(value, "Generation"); }
    }

    [ParseFieldName("versionNumber")]
    public int VersionNumber
    {
        get { return GetProperty<int>("VersionNumber"); }
        set { SetProperty<int>(value, "VersionNumber"); }
    }

    [ParseFieldName("originalBrain")]
    public Brain OriginalBrain
    {
        get { return GetProperty<Brain>("OriginalBrain"); }
        set { SetProperty<Brain>(value, "OriginalBrain"); }
    }

    [ParseFieldName("multipleTargets")]
    public bool MultipleTargets
    {
        get { return GetProperty<bool>("MultipleTargets"); }
        set { SetProperty<bool>(value, "MultipleTargets"); }
    }

    [ParseFieldName("targetSize")]
    public int TargetSize
    {
        get { return GetProperty<int>("TargetSize"); }
        set { SetProperty<int>(value, "TargetSize"); }
    }

    [ParseFieldName("population")]
    public ParseFile Population
    {
        get { return GetProperty<ParseFile>("Population"); }
        set { SetProperty<ParseFile>(value, "Population"); }
    }

    [ParseFieldName("championGene")]
    public ParseFile ChampionGene
    {
        get { return GetProperty<ParseFile>("ChampionGene"); }
        set { SetProperty<ParseFile>(value, "ChampionGene"); }
    }

    public List<Brain> Children = new List<Brain>();
    
    public bool IsNewBrain;

    public Brain Branch()
    {
        Brain brain = new Brain();

        brain.Name = "(Branch) " + this.Name;
        brain.Description = this.Description;
        brain.ParentId = this.ObjectId;
        brain.UserId = this.UserId;

        brain.ReachDistance = this.ReachDistance;
        brain.MoveAround = this.MoveAround;
        brain.KeepDistance = this.KeepDistance;
        brain.DistanceToKeep = this.DistanceToKeep;
        brain.FaceTarget = this.FaceTarget;
        brain.TurretFaceTarget = this.TurretFaceTarget;
        brain.MeleeAttacks = this.MeleeAttacks;
        brain.MeleeHits = this.MeleeHits;
        brain.MeleePrecision = this.MeleePrecision;
        brain.RifleAttacks = this.RifleAttacks;
        brain.RifleHits = this.RifleHits;
        brain.RiflePrecision = this.RiflePrecision;
        brain.MortarAttacks = this.MortarAttacks;
        brain.MortarHits = this.MortarHits;
        brain.MortarPrecision = this.MortarPrecision;
        brain.MortarDamagePerHit = this.MortarDamagePerHit;
        brain.TargetBehaviorMovement = this.TargetBehaviorMovement;
        brain.TargetSize = this.TargetSize;
        brain.MultipleTargets = this.MultipleTargets;

        brain.IsNewBrain = true;


        return brain;
    }

    //public override bool Equals(object obj)
    //{
    //    if (obj == null)
    //    {
    //        return false;
    //    }
    //    Brain other = obj as Brain;
    //    if (Id == null)
    //    {
    //        return false;
    //    }
    //    return this.Id.Equals(other.Id);
    //}
}
