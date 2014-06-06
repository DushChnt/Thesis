using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System.Threading.Tasks;
using System.Linq;
using System;

public class UserPanel : MonoBehaviour {

    public dfTextbox FilterTextbox;
    public dfScrollPanel PlayerPanel;
    IEnumerable<Player> users = null;
    IList<Player> pinnedPlayers = null;   
    bool FilterText = true;

    public GameObject ListItem;

    // Use this for initialization
    void Start()
    {
        // Get all users
        StartCoroutine(FindUsers());
        //   StartCoroutine(FindFriends());

        FilterTextbox.TextChanged += new PropertyChangedEventHandler<string>(FilterTextbox_TextChanged);
        FilterTextbox.LeaveFocus += new FocusEventHandler(FilterTextbox_LeaveFocus);

        pinnedPlayers = GetPinnedPlayers();
        print("Friends: " + pinnedPlayers.Count);

        foreach (Player p in pinnedPlayers)
        {
            print("Name: " + p.ObjectId);
        }
    }

    IEnumerator FindUsers()
    {
        Task findTask = Player.Query.WhereNotEqualTo("objectId", Player.CurrentUser.ObjectId).FindAsync().ContinueWith(t =>
        {
            users = t.Result.Cast<Player>();
        });
        while (!findTask.IsCompleted)
        {
            yield return null;
        }
        if (users != null && users.Count() > 0)
        {
            GenerateList("");
        }
    }

    IList<Player> GetPinnedPlayers()
    {
        Player current = Player.CurrentUser as Player;

        current.Get<string>("email");

        IList<Player> players = null;

        try
        {
            players = current.Get<IList<Player>>("pinnedPlayers");
        }
        catch (Exception e)
        {

        }
        if (players == null)
        {
            print("players is null");
            players = new List<Player>();
        }
        current["pinnedPlayers"] = players;
        current.SaveAsync();

        return players;
    }

    void SavePinnedPlayers(IList<Player> players)
    {
        Player current = Player.CurrentUser as Player;
        current["pinnedPlayers"] = players;
        current.SaveAsync();
    }

    void GenerateList(string filter)
    {
        var children = new List<GameObject>();

        foreach (Transform child in PlayerPanel.transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));

        PlayerPanel.Enable();

        IEnumerable<Player> filtered = users.Where(t => t.Username.ToLower().Contains(filter.ToLower()))
            .OrderByDescending(t => pinnedPlayers.Any(u => u.ObjectId.Equals(t.ObjectId))).ThenBy(t => t.Username);
        foreach (Player player in filtered)
        {
            

            // GameObject ListItem = Instantiate(Resources.Load("List Item")) as GameObject;
            dfPanel listItem = PlayerPanel.AddPrefab(ListItem) as dfPanel; // as UserListItem;
            listItem.Width = PlayerPanel.Width - PlayerPanel.FlowPadding.left - PlayerPanel.FlowPadding.right;

            ListItemExtras extras = listItem.GetComponent<ListItemExtras>();
            extras.Player = player;
         

            dfLabel username = listItem.Find("Username Label").GetComponent<dfLabel>();
            username.Text = player.Username;
            username.DisabledColor = new Color32(100, 100, 100, 255);           

            dfSprite onlineSprite = listItem.Find("Online Sprite").GetComponent<dfSprite>();
            if (player.IsOnline)
            {
                onlineSprite.SpriteName = "OUYA_O 24px";
            }
            else
            {
                onlineSprite.SpriteName = "OUYA_A 24px";
            }

            dfButton actionButton = listItem.Find("Action Button").GetComponent<dfButton>();

            bool f = pinnedPlayers.Any(t => t.ObjectId.Equals(player.ObjectId));

            if (f)
            {
                actionButton.Text = "Unpin";
              //  listItem.BackgroundColor = new Color32(200, 255, 200, 255);
                listItem.BackgroundColor = new Color32(255, 255, 255, 255);

                actionButton.Click += new MouseEventHandler(actionButtonUnpin_Click);
            }
            else
            {
                actionButton.Text = "Pin";
                actionButton.Click += new MouseEventHandler(actionButtonPin_Click);
            }
           
        }
    }

    void actionButtonPin_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        ListItemExtras parent = control.transform.parent.gameObject.GetComponent<ListItemExtras>();
        pinnedPlayers.Add(parent.Player);
        SavePinnedPlayers(pinnedPlayers);

        FilterText = false;
        FilterTextbox.Text = "Filter users...";
        FilterText = true;

        GenerateList("");
    }

    void actionButtonUnpin_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        ListItemExtras parent = control.transform.parent.gameObject.GetComponent<ListItemExtras>();
        var p = pinnedPlayers.Where(t => t.ObjectId.Equals(parent.Player.ObjectId)).FirstOrDefault();
        if (p != null)
        {
            pinnedPlayers.Remove(p);
        }
        else
        {
            print("p is null");
        }
        SavePinnedPlayers(pinnedPlayers);

        FilterText = false;
        FilterTextbox.Text = "Filter users...";
        FilterText = true;

        GenerateList("");
    }

    void FilterTextbox_LeaveFocus(dfControl control, dfFocusEventArgs args)
    {
        if (FilterTextbox.Text.Equals(""))
        {
            FilterText = false;
            FilterTextbox.Text = "Filter users...";
            FilterText = true;
        }
    }

    void FilterTextbox_TextChanged(dfControl control, string value)
    {
        if (FilterText)
        {
            GenerateList(value);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
