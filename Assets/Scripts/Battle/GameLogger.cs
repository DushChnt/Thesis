using UnityEngine;
using System.Collections;
using System.IO;

public class GameLogger : MonoBehaviour {

    bool isRunning;   
    BattleController OwnController, OpponentController;
    string filePath;
    Player Player
    {
        get
        {
            return Parse.ParseUser.CurrentUser as Player;
        }
    }

    NetworkGUI gui;

    private static GameLogger instance;
    public static GameLogger Instance
    {
        get
        {
            return instance ?? (instance = new GameObject("GameLogger").AddComponent<GameLogger>());
        }
    }

	// Use this for initialization
	void Awake () {
        filePath = Application.persistentDataPath + string.Format("/{0}/{1}.log", Parse.ParseUser.CurrentUser.Username, Parse.ParseUser.CurrentUser.ObjectId + System.DateTime.Now.Ticks);
        print("Awake: " + filePath);
        gui = GameObject.Find("Battle GUI").GetComponent<NetworkGUI>();
	}

    public void StartLogging(BattleController ownRobot, BattleController opponentRobot)
    {        
        this.OwnController = ownRobot;
        this.OpponentController = opponentRobot;
        WriteHeaderLine();
        this.isRunning = true;
    }

    public void StopLogging()
    {
        this.isRunning = false;
    }

    private void WriteHeaderLine()
    {
        if (Player == null)
        {
            print("Player is null");
        }
        string brain1 = Player.Brain1 != null ? Player.Brain1.ObjectId : "null";
        string brain2 = Player.Brain2 != null ? Player.Brain2.ObjectId : "null";
        string brain3 = Player.Brain3 != null ? Player.Brain3.ObjectId : "null";
        string brain4 = Player.Brain4 != null ? Player.Brain4.ObjectId : "null";
        string header = string.Format("Brain1:{0};Brain2:{1};Brain3:{2};Brain4:{3}", brain1, brain2, brain3, brain4);
        WriteLineToLog(header);
    }

    /// <summary>
    /// Writes the game state in a line.
    /// 
    /// Format: Arena;Time;OwnPosX;OwnPosY;OwnAngle;OwnVel;OwnHealth;OppPosX;OppPosY;OppAngle;OppVel;OppHealth;Brain
    /// </summary>
    private void WriteGameStateLine()
    {        
        WriteLineToLog(GetGameState());
    }

    /// <summary>
    /// Get a string representation of the game state
    /// 
    /// Format: Arena;Time;OwnPosX;OwnPosY;OwnAngle;OwnVel;OwnHealth;OppPosX;OppPosY;OppAngle;OppVel;OppHealth;Brain
    /// </summary>
    /// <returns>String representation of game state</returns>
    public string GetGameState()
    {
        string state = "1;"; // Arena
        state += gui._Timer + ";";
        state += OwnController.transform.position.x + ";";
        state += OwnController.transform.position.z + ";";
        state += Utility.AngleSigned(new Vector3(0, 0, 1), OwnController.transform.forward, new Vector3(0, 1, 0)) + ";";
        state += OwnController.CurrentSpeed + ";";
        state += OwnController.Health.Health + ";";
        state += OpponentController.transform.position.x + ";";
        state += OpponentController.transform.position.z + ";";
        state += Utility.AngleSigned(new Vector3(0, 0, 1), OpponentController.transform.forward, new Vector3(0, 1, 0)) + ";";
        state += OpponentController.CurrentSpeed + ";";
        state += OpponentController.Health.Health + ";";
        state += OwnController.ActiveBrain;

        return state;
    }

    private void WriteLineToLog(string line)
    {
     //   print("Writing line: " + line);
        using (StreamWriter writer = File.AppendText(filePath)) 
        {
            writer.WriteLine(line);
        }
    }

	// Update is called once per frame
	void Update () {
        if (isRunning && OwnController != null && OpponentController != null)
        {
            // Do the logging!
            WriteGameStateLine();
        }
	}
}
