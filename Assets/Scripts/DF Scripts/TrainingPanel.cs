using UnityEngine;
using System.Collections;
using System;

public class TrainingPanel : MonoBehaviour {

    public dfLabel GenerationLabel, IterationLabel, FitnessLabel, EvolutionLabel, TimeLabel, TotalTimeLabel, BrainNameLabel;
    public dfButton TrainButton, BackButton, RunBestButton;
    public Optimizer Optimizer;
    public DialogPanel DialogPanel;
    bool EARunning;
    long startTime;
    public long Time;
    string buttonText = "Train";
    string mode;

	// Use this for initialization
	void Start () {
        mode = PlayerPrefs.GetString("Mode");
        if (mode.Equals("Train"))
        {
            TrainButton.Click += new MouseEventHandler(TrainButton_Click);
            TrainButton.TextColor = new Color32(0, 205, 0, 255);
            TrainButton.FocusTextColor = new Color32(0, 205, 0, 255);
            TrainButton.HoverTextColor = new Color32(0, 255, 0, 255);
            TrainButton.PressedTextColor = new Color32(0, 255, 0, 255);

            
        }
        else
        {
            TrainButton.Text = "Run best";
            TrainButton.TextColor = new Color32(255, 255, 255, 255);
            TrainButton.FocusTextColor = new Color32(255, 255, 255, 255);
            TrainButton.HoverTextColor = new Color32(255, 255, 255, 255);
            TrainButton.PressedTextColor = new Color32(255, 255, 255, 255);

            TrainButton.Click += new MouseEventHandler(RunBestButton_Click);
        }
        BackButton.Click += new MouseEventHandler(BackButton_Click);
     //   DialogPanel.Dismissed += new global::DialogPanel.DismissedEventHandler(BackButton_Dismissed);

        Optimizer.EAStopped += new global::Optimizer.EAStoppedEventHandler(Optimizer_EAStopped);

        BrainNameLabel.Text = Settings.Brain.Name;

        float t_time = Settings.Brain.TotalTime;
        int t_minutes = Mathf.FloorToInt(t_time / 60F);
        int t_seconds = Mathf.FloorToInt(t_time - t_minutes * 60);

        string t_niceTime = string.Format("{0:0}:{1:00}", t_minutes, t_seconds);
        TotalTimeLabel.Text = t_niceTime;
        RunBestButton.Click += new MouseEventHandler(RunBestButton_Click);

        if (mode.Equals("Train"))
        {
         //   StartCoroutine(clickOnTrain());
        }
	}

    private IEnumerator clickOnTrain()
    {
        yield return new WaitForSeconds(0.10f);
        TrainButton.DoClick();
    }

    void Optimizer_EAStopped(object sender, EventArgs e)
    {
        buttonText = "Train";
        TrainButton.Text = "Train";
        TrainButton.TextColor = new Color32(0, 205, 0, 255);
        TrainButton.FocusTextColor = new Color32(0, 205, 0, 255);
        TrainButton.HoverTextColor = new Color32(0, 255, 0, 255);
        TrainButton.PressedTextColor = new Color32(0, 255, 0, 255);
        EARunning = false;
        Settings.Brain.TotalTime += Time;

        RunBestButton.Enable();
    }

    void BackButton_Dismissed(object sender, EventArgs e, ButtonState s)
    {
        switch (s)
        {
            case ButtonState.OK:
                Application.LoadLevel("Training");
                break;
            case ButtonState.Cancel:

                break;
        }
    }

    void BackButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        if (EARunning)
        {
            DialogPanel.ShowCancel("Just a second", "Your robots are training! If you go back now you loose all their progress. Press the Stop button to save their progress. Press OK to accept loosing all progress.");
            DialogPanel.Dismissed += new global::DialogPanel.DismissedEventHandler(BackButton_Dismissed);
        }
        else
        {
            //Application.LoadLevel("Training Overview");
            Application.LoadLevel("Training");
        }
    }

    void RunBestButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Optimizer.RunBest();
    }

    void TrainButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {        
        dfButton but = control as dfButton;
        if (EARunning)
        {
            Optimizer.StopEA();
            but.Text = "Stopping";
            but.TextColor = new Color32(245, 171, 0, 255);
            but.FocusTextColor = new Color32(245, 171, 0, 255);
            but.HoverTextColor = new Color32(245, 171, 0, 255);
            but.PressedTextColor = new Color32(245, 171, 0, 255);
        }
        else
        {
            Optimizer.StartEA();
            but.Text = "Stop";
            but.TextColor = new Color32(205, 0, 0, 255);
            but.FocusTextColor = new Color32(205, 0, 0, 255);
            but.HoverTextColor = new Color32(255, 0, 0, 255);
            but.PressedTextColor = new Color32(255, 0, 0, 255);
            EARunning = true;

            startTime = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond * 1000);

            RunBestButton.Disable();
        }
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(59, 126, 124, 42), buttonText)) {
    //        if (EARunning)
    //        {
    //            Optimizer.StopEA();
    //            TrainButton.Text = "Stopping";
    //            buttonText = "Stopping";
    //            TrainButton.TextColor = new Color32(245, 171, 0, 255);
    //            TrainButton.FocusTextColor = new Color32(245, 171, 0, 255);
    //            TrainButton.HoverTextColor = new Color32(245, 171, 0, 255);
    //            TrainButton.PressedTextColor = new Color32(245, 171, 0, 255);
    //        }
    //        else
    //        {
    //            Optimizer.StartEA();
    //            buttonText = "Stop";
    //            TrainButton.Text = "Stop";
    //            TrainButton.TextColor = new Color32(205, 0, 0, 255);
    //            TrainButton.FocusTextColor = new Color32(205, 0, 0, 255);
    //            TrainButton.HoverTextColor = new Color32(255, 0, 0, 255);
    //            TrainButton.PressedTextColor = new Color32(255, 0, 0, 255);
    //            EARunning = true;

    //            startTime = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond * 1000);
    //        }
    //    }
    //}
	
	// Update is called once per frame
	void Update () {

        long time = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond * 1000);
        long diff = time - startTime;
        if (diff > Time && EARunning)
        {
            Time = diff;

            int minutes = Mathf.FloorToInt(Time / 60F);
            int seconds = Mathf.FloorToInt(Time - minutes * 60);

            string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

            TimeLabel.Text = niceTime;

            float t_time = Settings.Brain.TotalTime + Time;
            int t_minutes = Mathf.FloorToInt(t_time / 60F);
            int t_seconds = Mathf.FloorToInt(t_time - t_minutes * 60);

            string t_niceTime = string.Format("{0:0}:{1:00}", t_minutes, t_seconds);
            TotalTimeLabel.Text = t_niceTime;
        }
	}
}
