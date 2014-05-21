using UnityEngine;
using System.Collections;

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
        dfSlider keepDistance = GameObject.Find("s_KeepDistance").GetComponent<dfSlider>();
        if (keepDistance != null)
        {
            print("Value: " + keepDistance.Value);
        }
        dfSlider distanceToKeep = GameObject.Find("s_DistanceToKeep").GetComponent<dfSlider>();
        if (keepDistance != null)
        {
            print("Value: " + distanceToKeep.Value);
        }
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

    public void SaveClicked()
    {
        GetOptimizerSettings();

        OptimizerParameters.WriteXML();
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
