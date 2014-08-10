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
    public TrainingDialogScript TrainingDialog;
    public HintsDialog HintsDialog;
    dfPanel panel;

    Player Player
    {
        get
        {
            return Parse.ParseUser.CurrentUser as Player;
        }
    }

    void Awake()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.offlineMode = true;
    }

	// Use this for initialization
	void Start () {
        panel = this.GetComponent<dfPanel>();
        BackButton.Click += new MouseEventHandler(BackButton_Click);
        DeleteButton.Click += new MouseEventHandler(DeleteButton_Click);
        TrainButton.Click += new MouseEventHandler(TrainButton_Click);
        InfoButton.Click += new MouseEventHandler(InfoButton_Click);
        Initialize();
        DisableLockedFocusAreas();

        if (Settings.Brain != null)
        {
            NameTextBox.Text = Settings.Brain.Name;
        }
	}

    void InfoButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        HintsDialog.ShowDialog();
    }

    IEnumerator WaitForRequest(Brain brain, bool pop)
    {
        WWW www = new WWW(pop ? brain.Population.Url.AbsoluteUri : brain.ChampionGene.Url.AbsoluteUri);
        // print("Downloading " + brain.ObjectId);
        yield return www;
        // print("Done downloading");

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

        GameObject.Find("TargetSize Slider").GetComponent<dfSlider>().Value = Settings.Brain.TargetSize > 0 ? Settings.Brain.TargetSize : 3;

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

    bool TestWeapons()
    {
        bool ok = true;
        // Test for selected weapons
        if (Player.Level > 1)
        {
            if (Player.MeleeWeapon == null || Player.MeleeWeapon.Equals(""))
            {
                ok = false;
            }
            if (Player.Level > 2)
            {
                if (Player.RangedWeapon == null || Player.RangedWeapon.Equals(""))
                {
                    ok = false;
                }
                if (Player.Level > 3)
                {
                    if (Player.MortarWeapon == null || Player.MortarWeapon.Equals(""))
                    {
                        ok = false;
                    }
                }
            }
        }
        return ok;
    }

    void TrainButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        PhotonNetwork.offlineMode = true;
        bool ok = GetOptimizerSettings();

        if (ok)
        {
            if (NameTextBox.Text == null || NameTextBox.Text.Length == 0)
            {
                panel.Disable();
                TrainingDialog.ShowDialog("You need to enter a name for the brain before continuing.");
                TrainingDialog.panel.Click += new MouseEventHandler(panel_Click);
            }
            else
            {
                if (TestWeapons())
                {
                    PlayerPrefs.SetString("Mode", "Train");
                    //   PlayerPrefs.SetInt("Evolution Speed", GetEvolutionSpeed());

                    Application.LoadLevel("Optimization scene");
                }
                else
                {
                    panel.Disable();
                    TrainingDialog.ShowDialog("You have to choose a weapon before continuing. Click on the flashing slot to the right to assign.");
                    TrainingDialog.panel.Click += new MouseEventHandler(panel_Click);
                }
            }
        }
        else
        {
            panel.Disable();
            TrainingDialog.ShowDialog();
            TrainingDialog.panel.Click += new MouseEventHandler(panel_Click);
        }
    }

    void panel_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        panel.Enable();
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
        switch (PlayerPrefs.GetString(CameFromScript.CAME_FROM))
        {
            case CameFromScript.BOOTCAMP:
                Application.LoadLevel("Bootcamp");
                break;
            case CameFromScript.CHAMPIONS_ARENA:
                Application.LoadLevel("Champions arena");
                break;
        }
       // Application.LoadLevel("Bootcamp");
    }

    private bool GetOptimizerSettings()
    {
      //  OptimizerParameters.Reset();
        if (!Settings.Brain.IsNewBrain)
        {
            var name = NameTextBox.Text;
            var numInputs = OptimizerParameters.NumInputs;
            var numOutputs = OptimizerParameters.NumOutputs;

            var moveAround = GameObject.Find("s_MoveAround").GetComponent<dfSlider>().Value;
            var distanceToKeep = GameObject.Find("s_DistanceToKeep").GetComponent<dfSlider>().Value;
            var keepDistance = GameObject.Find("s_KeepDistance").GetComponent<dfSlider>().Value;
            var reachDistance = GameObject.Find("s_ReachDistance").GetComponent<dfSlider>().Value;
            var faceTarget = GameObject.Find("s_FaceTarget").GetComponent<dfSlider>().Value;

            var meleeAttacks = GameObject.Find("s_MeleeAttacks").GetComponent<dfSlider>().Value;
            var meleeHits = GameObject.Find("s_MeleeHits").GetComponent<dfSlider>().Value;
            var meleePrecision = GameObject.Find("s_MeleePrecision").GetComponent<dfSlider>().Value;

            var rifleAttacks = GameObject.Find("s_RifleAttacks").GetComponent<dfSlider>().Value;
            var rifleHits = GameObject.Find("s_RifleHits").GetComponent<dfSlider>().Value;
            var riflePrecision = GameObject.Find("s_RiflePrecision").GetComponent<dfSlider>().Value;

            var aimTurret = GameObject.Find("s_TurretFaceTarget").GetComponent<dfSlider>().Value;
            var mortarAttacks = GameObject.Find("s_MortarAttacks").GetComponent<dfSlider>().Value;
            var mortarHits = GameObject.Find("s_MortarHits").GetComponent<dfSlider>().Value;
            var mortarPrecision = GameObject.Find("s_MortarPrecision").GetComponent<dfSlider>().Value;
            var mortarDamagePerHit = GameObject.Find("s_MortarDamagePerHit").GetComponent<dfSlider>().Value;

            var value = GameObject.Find("Target Movement Dropdown").GetComponent<dfDropdown>().SelectedValue;
            var targetBehaviourMovement = value;
            var multiTargets = GameObject.Find("Multiple Targets Checkbox").GetComponent<dfCheckbox>().IsChecked;

            var targetSize = (int)GameObject.Find("TargetSize Slider").GetComponent<dfSlider>().Value;


            // Compare slider values
            bool difference = (!name.Equals(Settings.Brain.Name)) || (moveAround != Settings.Brain.MoveAround) || (distanceToKeep != Settings.Brain.DistanceToKeep) ||
                (keepDistance != Settings.Brain.KeepDistance) || (reachDistance != Settings.Brain.ReachDistance) || (faceTarget != Settings.Brain.FaceTarget) ||
                (meleeAttacks != Settings.Brain.MeleeAttacks) || (meleeHits != Settings.Brain.MeleeHits) || (meleePrecision != Settings.Brain.MeleePrecision) ||
                (rifleAttacks != Settings.Brain.RifleAttacks) || (rifleHits != Settings.Brain.RifleHits) || (riflePrecision != Settings.Brain.RiflePrecision) ||
                (aimTurret != Settings.Brain.TurretFaceTarget) || (mortarAttacks != Settings.Brain.MortarAttacks) || (mortarHits != Settings.Brain.MortarHits) ||
                (mortarPrecision != Settings.Brain.MortarPrecision) || (mortarDamagePerHit != Settings.Brain.MortarDamagePerHit) || (!targetBehaviourMovement.Equals(Settings.Brain.TargetBehaviorMovement)) ||
                (multiTargets != Settings.Brain.MultipleTargets) || (targetSize != Settings.Brain.TargetSize);

            if (difference)
            {
                // Create a new brain with the old parameters and archive it
                Brain archive = Settings.Brain.Branch();
                archive.Generation = Settings.Brain.Generation;
                archive.BestFitness = Settings.Brain.BestFitness;
                archive.TotalTime = Settings.Brain.TotalTime;
                archive.VersionNumber = Settings.Brain.VersionNumber;
                archive.Population = Settings.Brain.Population;
                archive.ChampionGene = Settings.Brain.ChampionGene;
                archive.OriginalBrain = Settings.Brain;

                archive.SaveAsync();

                Settings.Brain.VersionNumber = Settings.Brain.VersionNumber + 1;
            }
        }
        // CUT

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

        var val = GameObject.Find("Target Movement Dropdown").GetComponent<dfDropdown>().SelectedValue;
        Settings.Brain.TargetBehaviorMovement = val;
        Settings.Brain.MultipleTargets = GameObject.Find("Multiple Targets Checkbox").GetComponent<dfCheckbox>().IsChecked;

        Settings.Brain.TargetSize = (int)GameObject.Find("TargetSize Slider").GetComponent<dfSlider>().Value;

        if (ParseUser.CurrentUser != null)
        {
            Task saveTask = Settings.Brain.SaveAsync();
        }

        bool ok = Settings.Brain.MoveAround != 0 || Settings.Brain.DistanceToKeep != 0 || Settings.Brain.ReachDistance != 0 || Settings.Brain.FaceTarget != 0 ||
            Settings.Brain.MeleeAttacks != 0 || Settings.Brain.MeleeHits != 0 || Settings.Brain.MeleePrecision != 0 ||
            Settings.Brain.RifleAttacks != 0 || Settings.Brain.RifleHits != 0 || Settings.Brain.RiflePrecision != 0 ||
            Settings.Brain.TurretFaceTarget != 0 || Settings.Brain.MortarAttacks != 0 || Settings.Brain.MortarHits != 0 ||
            Settings.Brain.MortarPrecision != 0 || Settings.Brain.MortarDamagePerHit != 0;

        return ok;
    }    
	
	// Update is called once per frame
	void Update () {
	
	}
}
