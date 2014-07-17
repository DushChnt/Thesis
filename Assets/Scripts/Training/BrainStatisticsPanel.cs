using UnityEngine;
using System.Collections;

public class BrainStatisticsPanel : MonoBehaviour {

    public dfLabel GenerationLabel, FitnessLabel, TimeLabel;

	// Use this for initialization
	void Start () {
        if (Settings.Brain != null)
        {
            GenerationLabel.Text = Settings.Brain.Generation + "";
            FitnessLabel.Text = string.Format("{0:0.00}", Settings.Brain.BestFitness);
            float t_time = Settings.Brain.TotalTime;
            int t_minutes = Mathf.FloorToInt(t_time / 60F);
            int t_seconds = Mathf.FloorToInt(t_time - t_minutes * 60);

            string t_niceTime = string.Format("{0:0}:{1:00}", t_minutes, t_seconds);
            TimeLabel.Text = t_niceTime;           
            
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
