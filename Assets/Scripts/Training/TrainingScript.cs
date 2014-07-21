using UnityEngine;
using System.Collections;
using Parse;
using System.IO;
using System.Threading.Tasks;

public class TrainingScript : MonoBehaviour {

    public dfButton BackButton, InfoButton, DeleteButton, TrainButton;
    public dfLabel UnlockLevel2Label, UnlockLevel3Label, UnlockLevel4Label;
    public dfPanel MovementPanel, MeleePanel, RiflePanel, MortarPanel;
    public dfSlicedSprite ZeroDenominator;
    public dfTextbox NameTextBox;

    Player Player
    {
        get
        {
            return Parse.ParseUser.CurrentUser as Player;
        }
    }

	// Use this for initialization
	void Start () {
        BackButton.Click += new MouseEventHandler(BackButton_Click);
        DeleteButton.Click += new MouseEventHandler(DeleteButton_Click);
        TrainButton.Click += new MouseEventHandler(TrainButton_Click);
        Initialize();
        DisableLockedFocusAreas();

        if (Settings.Brain != null)
        {
            NameTextBox.Text = Settings.Brain.Name;
        }
	}

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
        if (!pop)
        {
            popFilePath = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", ParseUser.CurrentUser.Username, brain.ObjectId);
        }

        File.WriteAllText(popFilePath, www.text);
        TrainButton.Enable();
    }

    void Initialize()
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
       
        NameTextBox.Text = Settings.Brain.Name;


        GameObject.Find("s_ReachDistance").GetComponent<dfSlider>().Value = Settings.Brain.ReachDistance;
        GameObject.Find("s_DistanceToKeep").GetComponent<dfSlider>().Value = Settings.Brain.DistanceToKeep;
        GameObject.Find("s_KeepDistance").GetComponent<dfSlider>().Value = Settings.Brain.KeepDistance;
        GameObject.Find("s_MoveAround").GetComponent<dfSlider>().Value = Settings.Brain.MoveAround;

        GameObject.Find("s_FaceTarget").GetComponent<dfSlider>().Value = Settings.Brain.FaceTarget;
        GameObject.Find("s_TurretFaceTarget").GetComponent<dfSlider>().Value = Settings.Brain.TurretFaceTarget;
        GameObject.Find("s_MeleeAttacks").GetComponent<dfSlider>().Value = Settings.Brain.MeleeAttacks;
        GameObject.Find("s_MeleeHits").GetComponent<dfSlider>().Value = Settings.Brain.MeleeHits;
        GameObject.Find("s_MeleePrecision").GetComponent<dfSlider>().Value = Settings.Brain.MeleePrecision;
        GameObject.Find("s_RifleAttacks").GetComponent<dfSlider>().Value = Settings.Brain.RifleAttacks;
        GameObject.Find("s_RifleHits").GetComponent<dfSlider>().Value = Settings.Brain.RifleHits;
        GameObject.Find("s_RiflePrecision").GetComponent<dfSlider>().Value = Settings.Brain.RiflePrecision;
        GameObject.Find("s_MortarAttacks").GetComponent<dfSlider>().Value = Settings.Brain.MortarAttacks;
        GameObject.Find("s_MortarHits").GetComponent<dfSlider>().Value = Settings.Brain.MortarHits;
        GameObject.Find("s_MortarPrecision").GetComponent<dfSlider>().Value = Settings.Brain.MortarPrecision;
        GameObject.Find("s_MortarDamagePerHit").GetComponent<dfSlider>().Value = Settings.Brain.MortarDamagePerHit;
        GameObject.Find("s_MortarDamagePerHit").GetComponent<dfSlider>().PerformLayout();

        GameObject.Find("Multiple Targets Checkbox").GetComponent<dfCheckbox>().IsChecked = Settings.Brain.MultipleTargets;

        dfDropdown dropdown = GameObject.Find("Target Movement Dropdown").GetComponent<dfDropdown>();

        int index = 0;
        dropdown.SelectedIndex = index;
        for (index = 0; index < dropdown.Items.Length; index++)
        {
            if (dropdown.Items[index].Equals(Settings.Brain.TargetBehaviorMovement))
            {
                dropdown.SelectedIndex = index;
                break;
            }
        }
    }

    void TrainButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        PhotonNetwork.offlineMode = true;
        GetOptimizerSettings();
       
        PlayerPrefs.SetString("Mode", "Train");
     //   PlayerPrefs.SetInt("Evolution Speed", GetEvolutionSpeed());

        Application.LoadLevel("Optimization scene");
    }  

    void DisableLockedFocusAreas()
    {
        ZeroDenominator.Height = 551;
        if (Player.Level < 4)
        {
            ZeroDenominator.Height = 365;
            MortarPanel.Hide();
            UnlockLevel4Label.Show();
            if (Player.Level < 3)
            {
                ZeroDenominator.Height = 240;
                RiflePanel.Hide();
                UnlockLevel3Label.Show();
                if (Player.Level < 2)
                {
                    ZeroDenominator.Height = 130;
                    MeleePanel.Hide();
                    UnlockLevel2Label.Show();
                }
            }
        }
    }

    void DeleteButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        
    }

    void BackButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Application.LoadLevel("Bootcamp");
    }

    private void GetOptimizerSettings()
    {
      //  OptimizerParameters.Reset();

        Settings.Brain.Name = NameTextBox.Text;       
        Settings.Brain.NumInputs = OptimizerParameters.NumInputs;
        Settings.Brain.NumOutputs = OptimizerParameters.NumOutputs;

        Settings.Brain.MoveAround = GameObject.Find("s_MoveAround").GetComponent<dfSlider>().Value;
        Settings.Brain.DistanceToKeep = GameObject.Find("s_DistanceToKeep").GetComponent<dfSlider>().Value;
        Settings.Brain.KeepDistance = GameObject.Find("s_KeepDistance").GetComponent<dfSlider>().Value;
        Settings.Brain.ReachDistance = GameObject.Find("s_ReachDistance").GetComponent<dfSlider>().Value;
        Settings.Brain.FaceTarget = GameObject.Find("s_FaceTarget").GetComponent<dfSlider>().Value;
       
        Settings.Brain.MeleeAttacks = GameObject.Find("s_MeleeAttacks").GetComponent<dfSlider>().Value;
        Settings.Brain.MeleeHits = GameObject.Find("s_MeleeHits").GetComponent<dfSlider>().Value;
        Settings.Brain.MeleePrecision = GameObject.Find("s_MeleePrecision").GetComponent<dfSlider>().Value;

        Settings.Brain.RifleAttacks = GameObject.Find("s_RifleAttacks").GetComponent<dfSlider>().Value;
        Settings.Brain.RifleHits = GameObject.Find("s_RifleHits").GetComponent<dfSlider>().Value;
        Settings.Brain.RiflePrecision = GameObject.Find("s_RiflePrecision").GetComponent<dfSlider>().Value;

        Settings.Brain.TurretFaceTarget = GameObject.Find("s_TurretFaceTarget").GetComponent<dfSlider>().Value;
        Settings.Brain.MortarAttacks = GameObject.Find("s_MortarAttacks").GetComponent<dfSlider>().Value;
        Settings.Brain.MortarHits = GameObject.Find("s_MortarHits").GetComponent<dfSlider>().Value;
        Settings.Brain.MortarPrecision = GameObject.Find("s_MortarPrecision").GetComponent<dfSlider>().Value;
        Settings.Brain.MortarDamagePerHit = GameObject.Find("s_MortarDamagePerHit").GetComponent<dfSlider>().Value;

        Settings.Brain.FitnessMode = Brain.ADVANCED;

        string value = GameObject.Find("Target Movement Dropdown").GetComponent<dfDropdown>().SelectedValue;
        Settings.Brain.TargetBehaviorMovement = value;
        Settings.Brain.MultipleTargets = GameObject.Find("Multiple Targets Checkbox").GetComponent<dfCheckbox>().IsChecked;

        if (ParseUser.CurrentUser != null)
        {
            Task saveTask = Settings.Brain.SaveAsync();
        }
    }    
	
	// Update is called once per frame
	void Update () {
	
	}
}
