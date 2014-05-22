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
        GetOptimizerSettings();

        OptimizerParameters.WriteXML();
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

    public void BackClicked()
    {
        Application.LoadLevel("Start Menu");
    }

    public void SaveClicked()
    {
        GetOptimizerSettings();

        OptimizerParameters.WriteXML();

        SaveBrainToServer();
    }

    private void SaveBrainToServer()
    {
        Settings.Brain.Name = OptimizerParameters.Name;
        Settings.Brain.Description = OptimizerParameters.Description;

    
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

        OptimizerParameters.TargetMoveStrategy = GetTargetMovePattern();

    }

    private TargetMove GetTargetMovePattern()
    {
        string value = GameObject.Find("Target Movement Dropdown").GetComponent<dfDropdown>().SelectedValue;

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
