using UnityEngine;
using System.Collections;
using Parse;

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

        var flee = "Flee";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), flee))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = flee;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Stationary;
            OptimizerParameters.MultipleTargets = true;
            OptimizerParameters.WApproach = 1f;
            OptimizerParameters.DistanceToKeep = 20f;
            buttonClicked = true;
        }


        if (GUI.Button(new Rect(250, 10, 200, 60), "BATTLE!"))
        {

            Application.LoadLevel("Battle Arena");
        }

        GUI.Label(new Rect(250, 90, 200, 60), Application.persistentDataPath);

        if (GUI.Button(new Rect(500, 10, 200, 60), "NETWORK!"))
        {

            Application.LoadLevel("Network Test");
        }

        if (GUI.Button(new Rect(500, 80, 200, 60), "NETWORK!"))
        {
            print("Checking connection");
            ParseQuery<ParseObject> query = ParseObject.GetQuery("TestObject");
            query.GetAsync("BAi0Ok6Ipi").ContinueWith(t =>
            {
                if (t != null)
                {
                    print("Connected!");
                    print(t.Result.Get<string>("foo"));
                }
            });
        }

        

        top = 120;
        var turret_angle = "Turret Angle";
        top += 40;
        if (GUI.Button(new Rect(250, top, 200, 30), turret_angle))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = turret_angle;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Stationary;           
        //    OptimizerParameters.WApproach = 1f;
            OptimizerParameters.DistanceToKeep = 30f;
            OptimizerParameters.WTurretAngleTowards = 10f;
            buttonClicked = true;
        }

        var mortar_attack = "Mortar Attack";
        top += 40;
        if (GUI.Button(new Rect(250, top, 200, 30), mortar_attack))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = mortar_attack;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Stationary;
            //    OptimizerParameters.WApproach = 1f;
            OptimizerParameters.DistanceToKeep = 30f;
            OptimizerParameters.WTurretAngleTowards = 10f;
            OptimizerParameters.WMortarAttack = 0.1f;
            buttonClicked = true;
        }

        var mortar_hits = "Mortar Hits";
        top += 40;
        if (GUI.Button(new Rect(250, top, 200, 30), mortar_hits))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = mortar_hits;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Stationary;
            //    OptimizerParameters.WApproach = 1f;
         //   OptimizerParameters.DistanceToKeep = 30f;
        //    OptimizerParameters.WTurretAngleTowards = 10f;
          //  OptimizerParameters.WMortarAttack = 0.1f;
        //    OptimizerParameters.WMortarHits = 10f;
            OptimizerParameters.WMortarDamage = 1f;
            buttonClicked = true;
        }

        var mortar_precision = "Mortar Precision";
        top += 40;
        if (GUI.Button(new Rect(250, top, 200, 30), mortar_precision))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = mortar_precision;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Stationary;
            //    OptimizerParameters.WApproach = 1f;
            //   OptimizerParameters.DistanceToKeep = 30f;
            //    OptimizerParameters.WTurretAngleTowards = 10f;
            //  OptimizerParameters.WMortarAttack = 0.1f;
            //    OptimizerParameters.WMortarHits = 10f;
         //   OptimizerParameters.WMortarDamage = 1f;
            OptimizerParameters.WMortarPrecision = 1f;
            buttonClicked = true;
        }

        var mortar_dmg_hit = "Mortar Dmg pr Hit";
        top += 40;
        if (GUI.Button(new Rect(250, top, 200, 30), mortar_dmg_hit))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = mortar_dmg_hit;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Stationary;
            //    OptimizerParameters.WApproach = 1f;
            //   OptimizerParameters.DistanceToKeep = 30f;
            //    OptimizerParameters.WTurretAngleTowards = 10f;
            //  OptimizerParameters.WMortarAttack = 0.1f;
            //    OptimizerParameters.WMortarHits = 10f;
            //   OptimizerParameters.WMortarDamage = 1f;
        //    OptimizerParameters.WMortarPrecision = 1f;
            OptimizerParameters.WMortarDamagePerHit = 1f;
            buttonClicked = true;
        }

        if (buttonClicked)
        {
            Utility.DebugLog = false;
            Application.LoadLevel("Optimization scene");
        }
    }
}
