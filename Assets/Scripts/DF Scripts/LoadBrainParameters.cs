using UnityEngine;
using System.Collections;
using System.IO;
using Parse;

public class LoadBrainParameters : MonoBehaviour {

    public dfTextbox NameTextbox;
    public dfTextbox DescriptionTextBox;
    public dfButton TrainButton;

    IEnumerator WaitForRequest(Brain brain, bool pop)
    {
        WWW www = new WWW(pop ? brain.Population.Url.AbsoluteUri : brain.ChampionGene.Url.AbsoluteUri); 
        print("Downloading");
        yield return www;
        print("Done downloading");

        string folderPath = Application.persistentDataPath + string.Format("/{0}", ParseUser.CurrentUser.Username);
        DirectoryInfo dirInf = new DirectoryInfo(folderPath);
        if (!dirInf.Exists)
        {
            Debug.Log("Creating subdirectory");
            dirInf.Create();
        }
        string popFilePath = Application.persistentDataPath + string.Format("/{0}/{1}.pop.xml", ParseUser.CurrentUser.Username, brain.ObjectId);
        if (!pop) {
            popFilePath = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", ParseUser.CurrentUser.Username, brain.ObjectId);
        }
        
        File.WriteAllText(popFilePath, www.text);
        TrainButton.Enable();
    }

	// Use this for initialization
    void Start()
    {
        if (Settings.Brain == null)
        {
            Settings.Brain = new Brain();
        }
        if (Settings.Brain.Population != null)
        {
            TrainButton.Disable();
            StartCoroutine(WaitForRequest(Settings.Brain, true));
        }
        if (Settings.Brain.ChampionGene != null)
        {
            TrainButton.Disable();
            StartCoroutine(WaitForRequest(Settings.Brain, false));
        }
        //   if (!Settings.IsNewBrain && Settings.Brain.Name != null && Settings.Brain.Name.Length > 0)
        {
            NameTextbox.Text = Settings.Brain.Name;
        }
        //     if (!Settings.IsNewBrain && Settings.Brain.Description != null && Settings.Brain.Description.Length > 0)
        {
            DescriptionTextBox.Text = Settings.Brain.Description;
        }

        GameObject.Find("s_DistanceToKeep").GetComponent<dfSlider>().Value = Settings.Brain.DistanceToKeep;
        GameObject.Find("s_KeepDistance").GetComponent<dfSlider>().Value = Settings.Brain.KeepDistance;

        GameObject.Find("s_FaceTarget").GetComponent<dfSlider>().Value = Settings.Brain.FaceTarget;
        GameObject.Find("s_TurretFaceTarget").GetComponent<dfSlider>().Value = Settings.Brain.TurretFaceTarget;
        GameObject.Find("s_MeleeAttacks").GetComponent<dfSlider>().Value = Settings.Brain.MeleeAttacks;
        GameObject.Find("s_RifleAttacks").GetComponent<dfSlider>().Value = Settings.Brain.RifleAttacks;
        GameObject.Find("s_RifleHits").GetComponent<dfSlider>().Value = Settings.Brain.RifleHits;
        GameObject.Find("s_RiflePrecision").GetComponent<dfSlider>().Value = Settings.Brain.RiflePrecision;
        GameObject.Find("s_MortarAttacks").GetComponent<dfSlider>().Value = Settings.Brain.MortarAttacks;
        GameObject.Find("s_MortarHits").GetComponent<dfSlider>().Value = Settings.Brain.MortarHits;
        GameObject.Find("s_MortarPrecision").GetComponent<dfSlider>().Value = Settings.Brain.MortarPrecision;
        GameObject.Find("s_MortarDamagePerHit").GetComponent<dfSlider>().Value = Settings.Brain.MortarDamagePerHit;
        GameObject.Find("s_MortarDamagePerHit").GetComponent<dfSlider>().PerformLayout();

        dfDropdown dropdown = GameObject.Find("Target Movement Dropdown").GetComponent<dfDropdown>();

        int index = -1;
        dropdown.SelectedIndex = index;
        for (index = 0; index < dropdown.Items.Length; index++) {
            if (dropdown.Items[index].Equals(Settings.Brain.TargetBehaviorMovement)) {
                dropdown.SelectedIndex = index;
                break;
            }
        }


        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
