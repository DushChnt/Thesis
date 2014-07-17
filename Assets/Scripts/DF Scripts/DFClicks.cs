using UnityEngine;
using System.Collections;
using Parse;
using System.Threading.Tasks;

public class DFClicks : MonoBehaviour
{

    public void LoginClicked()
    {
        print("Joe");
    }

    public void RegisterClicked()
    {
        print("Register");
    }

    public void TrainButton()
    {
        PhotonNetwork.offlineMode = true;
        GetOptimizerSettings();

    //    OptimizerParameters.WriteXML();
        SaveBrainToServer();
        PlayerPrefs.SetString("Mode", "Train");
        PlayerPrefs.SetInt("Evolution Speed", GetEvolutionSpeed());

        Application.LoadLevel("Optimization scene");
    }

    private int GetEvolutionSpeed()
    {
        dfSlider slider = GameObject.Find("Speed Slider").GetComponent<dfSlider>();

        return 6 + (int)slider.Value;
    }

    public void RunBestButton()
    {
        PhotonNetwork.offlineMode = true;
        PlayerPrefs.SetString("Mode", "RunBest");
        GetOptimizerSettings();

        Application.LoadLevel("Optimization scene");
    }

    public void ResetClicked()
    {
        GameObject focusAreas = GameObject.Find("Focus Areas");

        dfSlider[] sliders = focusAreas.GetComponentsInChildren<dfSlider>();

        foreach (dfSlider slide in sliders)
        {
            slide.Value = 0;
        }

    }

    public void SimpleResetClicked()
    {
        GameObject focusAreas = GameObject.Find("Simple Focus Areas");

        dfSlider[] sliders = focusAreas.GetComponentsInChildren<dfSlider>();
        dfCheckbox[] checkboxes = focusAreas.GetComponentsInChildren<dfCheckbox>();

        foreach (dfSlider slide in sliders)
        {
            slide.Value = 0;
        }
        foreach (dfCheckbox box in checkboxes)
        {
            box.IsChecked = false;
        }
    }

    private void RecurseDelete(Brain brain)
    {
        foreach (Brain b in brain.Children)
        {
            RecurseDelete(b);
        }
        brain.DeleteAsync();
    }

    public IEnumerator YesDeleteClicked()
    {
        // Delete stuff
        GameObject.Find("Dialog Panel").GetComponent<dfPanel>().Disable();

        foreach (Brain brain in Settings.Brain.Children)
        {
            RecurseDelete(brain);
        }

        var task = Settings.Brain.DeleteAsync();

        while (!task.IsCompleted)
        {
            yield return null;
        }

        GameObject.Find("Panel").GetComponent<dfPanel>().Enable();
        GameObject.Find("Dialog Panel").GetComponent<dfPanel>().Hide();
        Application.LoadLevel("Bootcamp");

    }

    public IEnumerator OnlyThisDeleteClicked()
    {
        // Delete stuff
        GameObject.Find("Dialog Panel").GetComponent<dfPanel>().Disable();

        // Reassign children
        var parent_id = Settings.Brain.ParentId;
        foreach (Brain brain in Settings.Brain.Children)
        {
            brain.ParentId = parent_id;
            brain.SaveAsync();
        }

        var task = Settings.Brain.DeleteAsync();

        while (!task.IsCompleted)
        {
            yield return null;
        }

        GameObject.Find("Panel").GetComponent<dfPanel>().Enable();
        GameObject.Find("Dialog Panel").GetComponent<dfPanel>().Hide();
        Application.LoadLevel("Bootcamp");

    }

    public void NoDeleteClicked()
    {        
        GameObject.Find("Panel").GetComponent<dfPanel>().Enable();
        GameObject.Find("Dialog Panel").GetComponent<dfPanel>().Hide();
    }

    public void DeleteClicked()
    {
        
        GameObject.Find("Panel").GetComponent<dfPanel>().Disable();
        GameObject.Find("Dialog Panel").GetComponent<dfPanel>().Show();
    }

    public void BackClicked()
    {
        Application.LoadLevel("Start Menu");
    }

    public void SaveClicked()
    {
        GetOptimizerSettings();

      //  OptimizerParameters.WriteXML();

        SaveBrainToServer();
    }

    private void SaveBrainToServer()
    {
        dfTabstrip tabStrip = GameObject.Find("Tabstrip").GetComponent<dfTabstrip>();

        switch (tabStrip.SelectedIndex)
        {
            case 0:
                Settings.Brain.FitnessMode = Brain.SIMPLE;
                break;
            case 1:
                Settings.Brain.FitnessMode = Brain.ADVANCED;
                break;
            case 2:
                Settings.Brain.FitnessMode = Brain.BATTLE;
                break;
            default:
                Settings.Brain.FitnessMode = Brain.ADVANCED;
                break;
        }

        Settings.Brain.Name = OptimizerParameters.Name;
        Settings.Brain.Description = OptimizerParameters.Description;
        Settings.Brain.NumInputs = OptimizerParameters.NumInputs;
        Settings.Brain.NumOutputs = OptimizerParameters.NumOutputs;

        // Simple parameters
        Settings.Brain.SMovement = GameObject.Find("SMovement Checkbox").GetComponent<dfCheckbox>().IsChecked;
        Settings.Brain.SDistance = GameObject.Find("SDistance Slider").GetComponent<dfSlider>().Value;        
        Settings.Brain.SMelee = GameObject.Find("SMelee Checkbox").GetComponent<dfCheckbox>().IsChecked;
        Settings.Brain.SRifle = GameObject.Find("SRifle Checkbox").GetComponent<dfCheckbox>().IsChecked;
        Settings.Brain.SMortar = GameObject.Find("SMortar Checkbox").GetComponent<dfCheckbox>().IsChecked;

        // Advanced parameters
        Settings.Brain.KeepDistance = OptimizerParameters.WApproach;
        Settings.Brain.DistanceToKeep = OptimizerParameters.DistanceToKeep;
        Settings.Brain.FaceTarget = OptimizerParameters.WAngleTowards;
        Settings.Brain.TurretFaceTarget = OptimizerParameters.WTurretAngleTowards;
        Settings.Brain.MeleeAttacks = OptimizerParameters.WMeleeAttack;
        Settings.Brain.MeleeHits = GameObject.Find("s_MeleeHits").GetComponent<dfSlider>().Value;
        Settings.Brain.MeleePrecision = GameObject.Find("s_MeleePrecision").GetComponent<dfSlider>().Value;

        Settings.Brain.RifleAttacks = OptimizerParameters.WRifleAttack;
        Settings.Brain.RifleHits = OptimizerParameters.WRifleHits;
        Settings.Brain.RiflePrecision = OptimizerParameters.WRiflePrecision;
        Settings.Brain.MortarAttacks = OptimizerParameters.WMortarAttack;
        Settings.Brain.MortarHits = OptimizerParameters.WMortarHits;
        Settings.Brain.MortarPrecision = OptimizerParameters.WMortarPrecision;
        Settings.Brain.MortarDamagePerHit = OptimizerParameters.WMortarDamagePerHit;

        Settings.Brain.TargetBehaviorMovement = System.Enum.GetName(typeof(TargetMove), OptimizerParameters.TargetMoveStrategy);
       

        //string popFilePath = Application.persistentDataPath + string.Format("/Populations/{0}.pop.xml", "MyPopulation8");
        //System.IO.StreamReader stream = new System.IO.StreamReader (popFilePath);
        //ParseFile file = new ParseFile("joe.xml", stream.BaseStream);
        //Task sTask = file.SaveAsync();

        //Settings.Brain.Population = file;

        if (ParseUser.CurrentUser != null)
        {         
            Task saveTask = Settings.Brain.SaveAsync();
        }
    }

    private void GetOptimizerSettings()
    {
        OptimizerParameters.Reset();

        OptimizerParameters.Name = GameObject.Find("Name Textbox").GetComponent<dfTextbox>().Text;
        OptimizerParameters.Description = GameObject.Find("Description Textbox").GetComponent<dfTextbox>().Text;

        OptimizerParameters.DistanceToKeep = GameObject.Find("s_DistanceToKeep").GetComponent<dfSlider>().Value;
        OptimizerParameters.WApproach = GameObject.Find("s_KeepDistance").GetComponent<dfSlider>().Value;

        OptimizerParameters.WAngleTowards = GameObject.Find("s_FaceTarget").GetComponent<dfSlider>().Value;
        OptimizerParameters.WTurretAngleTowards = GameObject.Find("s_TurretFaceTarget").GetComponent<dfSlider>().Value;
        OptimizerParameters.WMeleeAttack = GameObject.Find("s_MeleeAttacks").GetComponent<dfSlider>().Value;
        OptimizerParameters.WRifleAttack = GameObject.Find("s_RifleAttacks").GetComponent<dfSlider>().Value;
        OptimizerParameters.WRifleHits = GameObject.Find("s_RifleHits").GetComponent<dfSlider>().Value;
        OptimizerParameters.WRiflePrecision = GameObject.Find("s_RiflePrecision").GetComponent<dfSlider>().Value;
        OptimizerParameters.WMortarAttack = GameObject.Find("s_MortarAttacks").GetComponent<dfSlider>().Value;
        OptimizerParameters.WMortarHits = GameObject.Find("s_MortarHits").GetComponent<dfSlider>().Value;
        OptimizerParameters.WMortarPrecision = GameObject.Find("s_MortarPrecision").GetComponent<dfSlider>().Value;
        OptimizerParameters.WMortarDamagePerHit = GameObject.Find("s_MortarDamagePerHit").GetComponent<dfSlider>().Value;


        string value = GameObject.Find("Target Movement Dropdown").GetComponent<dfDropdown>().SelectedValue;
        OptimizerParameters.TargetMoveStrategy = GetTargetMovePattern(value);

    }

    public static TargetMove GetTargetMovePattern(string value)
    {
       

        switch (value)
        {
            case "Stationary":
                return TargetMove.Stationary;
            case "Simple":
                return TargetMove.Simple;
            case "Random":
                return TargetMove.Random;
        }

        return TargetMove.Stationary;
    }
}
