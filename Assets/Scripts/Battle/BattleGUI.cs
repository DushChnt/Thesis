using UnityEngine;
using System.Collections;

public class BattleGUI : MonoBehaviour
{
    public static float Robot1Health, Robot2Health;
    public static string Robot1Name, Robot2Name;
    public Camera MainCamera;
    public Camera RobotCamera;
    // Use this for initialization
    void Start()
    {
        MainCamera.enabled = true;
        RobotCamera.enabled = false;
    }

    // Update is called once per frame
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 100), string.Format("{0}: {1}\n{2}: {3}", Robot1Name, Robot1Health, Robot2Name, Robot2Health));
        if (GUI.Button(new Rect(10, 150, 100, 30), "Back"))
        {
            Application.LoadLevel("Optimizer Menu scene");
        }
       
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            MainCamera.enabled = !MainCamera.enabled;
            RobotCamera.enabled = !RobotCamera.enabled;
        }
    }
}
