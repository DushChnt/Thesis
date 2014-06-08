using UnityEngine;
using System.Collections;

public class InviteScript : MonoBehaviour {

    dfPanel InvitePanel;
    public dfButton SendInviteButton, CancelButton;
    public dfDropdown ArenaDropdown, GameModeDropdown;
    public dfLabel TextLabel;
    Player playerToInvite;

	// Use this for initialization
	void Start () {
        InvitePanel = GetComponent<dfPanel>();
        CancelButton.Click += new MouseEventHandler(CancelButton_Click);
        SendInviteButton.Click += new MouseEventHandler(SendInviteButton_Click);
	}

    void SendInviteButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        if (playerToInvite != null)
        {

        }
    }

    void CancelButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        playerToInvite = null;
        InvitePanel.Hide();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void InvitePlayer(Player player)
    {
        playerToInvite = player;
        InvitePanel.Show();
        TextLabel.Text = string.Format("Invite {0} to a match.", player.Username);
    }
}
