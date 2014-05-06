using UnityEngine;
using System.Collections;

public class GameMasterScript : MonoBehaviour {

    public GameObject Robot1, Robot2;
    BattleController controller1, controller2;

	// Use this for initialization
	void Start () {
        if (Robot1 != null && Robot2 != null)
        {
            // Load brains
            
            //var brain1 = Utility.LoadBrain(string.Format(@"Assets\Scripts\Populations\{0}Champ.gnm.xml", "Stationary Melee"));
            var brain1 = Utility.LoadBrain(Application.persistentDataPath + string.Format("/Populations/{0}Champ.gnm.xml", "Rifle Markman"));
            BattleGUI.Robot1Name = "Rifle Markman";
            Destroy(Robot1.GetComponent<RobotController>());
            Robot1.AddComponent<BattleController>();
            Robot1.transform.position = new Vector3( Robot1.transform.position.x + Utility.GenerateNoise(5f), 1, Robot1.transform.position.z + Utility.GenerateNoise(5f));
            controller1 = Robot1.GetComponent<BattleController>();
            controller1.HitLayers = 1 << LayerMask.NameToLayer("Robot");
         

           // var brain2 = Utility.LoadBrain(string.Format(@"Assets\Scripts\Populations\{0}Champ.gnm.xml", "Simple Attack"));
            var brain2 = Utility.LoadBrain(Application.persistentDataPath + string.Format("/Populations/{0}Champ.gnm.xml", "Simple Attack"));
            BattleGUI.Robot2Name = "Simple Attack";
            Destroy(Robot2.GetComponent<RobotController>());
            Robot2.AddComponent<BattleController>();
            Robot2.transform.position = new Vector3(Robot2.transform.position.x + Utility.GenerateNoise(5f), 1, Robot2.transform.position.z + Utility.GenerateNoise(5f));
            controller2 = Robot2.GetComponent<BattleController>();

            controller1.Activate(brain1, Robot2);
            controller2.Activate(brain2, Robot1);
        }
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
