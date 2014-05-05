using UnityEngine;
using System.Collections;

public class OptimizerMenuGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void OnGUI () {
        var top = 10;
        var stationary_aprroach = "Stationary Approach";
        var buttonClicked = false;
        if (GUI.Button(new Rect(10, top, 200, 30), stationary_aprroach))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = stationary_aprroach;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Stationary;
            OptimizerParameters.WMeleeAttack = 0;
            OptimizerParameters.WApproach = 10;
            buttonClicked = true;
        }

        var stationary_melee = "Stationary Melee";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), stationary_melee))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = stationary_melee;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Stationary;
            OptimizerParameters.WMeleeAttack = 1;
            OptimizerParameters.WApproach = 1;
            buttonClicked = true;
        }

        var simple_approach = "Simple Approach";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), simple_approach))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = simple_approach;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Simple;
            OptimizerParameters.WMeleeAttack = 0;
            OptimizerParameters.WApproach = 10;
            buttonClicked = true;
        }

        var simple_attack = "Simple Attack";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), simple_attack))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = simple_attack;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Simple;
            OptimizerParameters.WMeleeAttack = 1;
            OptimizerParameters.WApproach = 1;
            buttonClicked = true;
        }

        var random_approach = "Random Approach";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), random_approach))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = random_approach;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Random;
            OptimizerParameters.WMeleeAttack = 0;
            OptimizerParameters.WApproach = 10;
            buttonClicked = true;
        }

        var random_attack = "Random Attack";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), random_attack))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = random_attack;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Random;
            OptimizerParameters.WMeleeAttack = 1;
            OptimizerParameters.WApproach = 1;
            buttonClicked = true;
        }

        var angle_stationary = "Angle Stationary";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), angle_stationary))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = angle_stationary;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Stationary;
            OptimizerParameters.WMeleeAttack = 0;
            OptimizerParameters.WApproach = 10f;
            OptimizerParameters.WRifleAttack = 0;
            OptimizerParameters.WAngleTowards = 0.1f;
            OptimizerParameters.DistanceToKeep = 15f;
            buttonClicked = true;
        }

        var rifle_stationary = "Rifle Stationary";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), rifle_stationary))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = rifle_stationary;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Stationary;
            OptimizerParameters.WMeleeAttack = 0;
        //    OptimizerParameters.WApproach = 1f;
            OptimizerParameters.WRifleAttack = 0.1f;
            OptimizerParameters.WRifleHits = 1f;
       //     OptimizerParameters.WAngleTowards = 1f;
       //     OptimizerParameters.DistanceToKeep = 15f;
            buttonClicked = true;
        }

        var rifle_markan = "Rifle Markman";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), rifle_markan))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = rifle_markan;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Stationary;
            OptimizerParameters.WMeleeAttack = 0;
            //    OptimizerParameters.WApproach = 1f;
        //    OptimizerParameters.WRifleAttack = 0.1f;
            OptimizerParameters.WRifleHits = 1f;
            OptimizerParameters.WRiflePrecision = 10f;
            //     OptimizerParameters.WAngleTowards = 1f;
            //     OptimizerParameters.DistanceToKeep = 15f;
            buttonClicked = true;
        }


        if (GUI.Button(new Rect(250, 10, 200, 60), "BATTLE!"))
        {

            Application.LoadLevel("Battle Arena");
        }

        GUI.Label(new Rect(250, 90, 200, 60), Application.persistentDataPath);

        if (buttonClicked)
        {
            Utility.DebugLog = false;
            Application.LoadLevel("Optimization scene");
        }
    }
}
