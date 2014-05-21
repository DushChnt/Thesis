using UnityEngine;
using System.Collections;

public class TipsBehaviour : MonoBehaviour {

    dfPanel panel;
    dfLabel infoLabel;
    dfPanel focusAreas;
    TipState currentState = TipState.None;

	// Use this for initialization
	void Start () {
        panel = GetComponent<dfPanel>();
        infoLabel = panel.transform.FindChild("Info Label").GetComponent<dfLabel>();
        infoLabel.Text = "Look here for tips and tricks!";

        focusAreas = GameObject.Find("Focus Areas").GetComponent<dfPanel>();
        focusAreas.MouseHover += focusAreas_MouseHover;

        panel.MouseHover += panel_MouseHover;

        GameObject.Find("Target Behavior").GetComponent<dfPanel>().MouseHover += TipsBehaviour_MouseHover;
        GameObject.Find("Train Button").GetComponent<dfButton>().MouseHover += TrainButton_MouseHover;
        GameObject.Find("Reset Button").GetComponent<dfButton>().MouseHover += ResetButton_MouseHover;

        
	}

    void TrainButton_MouseHover(dfControl control, dfMouseEventArgs mouseEvent)
    {
        updateTipText(TipState.TrainButton);
    }

    void ResetButton_MouseHover(dfControl control, dfMouseEventArgs mouseEvent)
    {
       // print("ResetButton_MouseHover");
        mouseEvent.Use();
        updateTipText(TipState.ResetButton);
    }

    void TipsBehaviour_MouseHover(dfControl control, dfMouseEventArgs mouseEvent)
    {
        updateTipText(TipState.TargetBehavior);
    }

    void panel_MouseHover(dfControl control, dfMouseEventArgs mouseEvent)
    {
        updateTipText(TipState.None);
    }

    void focusAreas_MouseHover(dfControl control, dfMouseEventArgs mouseEvent)
    {
        updateTipText(TipState.FocusAreas);
    }

    private void updateTipText(TipState state)
    {
        if (currentState != state)
        {
            currentState = state;
            switch (state)
            {
                case TipState.None:
                    infoLabel.Text = "Look here for tips and tricks!";
                    break;
                case TipState.FocusAreas:
                    infoLabel.Text = "Try to only choose a few focus areas at a time in order to focus the search more efficiently.";
                    break;
                case TipState.TargetBehavior:
                    infoLabel.Text = "Choose how the target should move. Simple movement are easier for the robots to learn to target, while moving targets are more realistic.";
                    break;
                case TipState.TrainButton:
                    infoLabel.Text = "When all parameters are satisfactory set, start the training with this button.";
                    break;
                case TipState.ResetButton:
                    infoLabel.Text = "Set all parameter values to 0.";
                    break;
            }
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}

public enum TipState { None, FocusAreas, TargetBehavior, TrainButton, ResetButton}