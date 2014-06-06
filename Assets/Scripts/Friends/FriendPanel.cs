using UnityEngine;
using System.Collections;
using Parse;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

public class FriendPanel : MonoBehaviour {

    public const string FRIEND_REQUEST = "FriendRequest";
    public const string REQUEST_FROM = "requestFrom";
    public const string REQUEST_TO = "requestTo";
    public const string STATUS = "status";
    public const string S_PENDING = "pending";
    public const string S_ACCEPTED = "accepted";
    public const string S_REJECTED = "rejected";

    public dfTextbox FilterTextbox;
    public dfScrollPanel UserPanel;
    IEnumerable<Player> users = null;
    IEnumerable<ParseObject> friends = null;
    List<string> friendList = null;
    bool FilterText = true;

    public GameObject ListItem;

	// Use this for initialization
	void Start () {
	    // Get all users
        StartCoroutine(FindUsers());
     //   StartCoroutine(FindFriends());

        FilterTextbox.TextChanged += new PropertyChangedEventHandler<string>(FilterTextbox_TextChanged);
        FilterTextbox.LeaveFocus += new FocusEventHandler(FilterTextbox_LeaveFocus);
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

    

    IEnumerator FindUsers()
    {
        var from = ParseObject.GetQuery(FRIEND_REQUEST).WhereEqualTo(REQUEST_FROM, Player.CurrentUser);
        var to = ParseObject.GetQuery(FRIEND_REQUEST).WhereEqualTo(REQUEST_TO, Player.CurrentUser);

        Task friendTask = from.Or(to).FindAsync().ContinueWith(t =>
            {
                friends = t.Result;
                print("Results: " + t.Result.Count());
                friendList = new List<string>();
                foreach (ParseObject po in friends)
                {
                    Player p_from = po[REQUEST_FROM] as Player;
                    Player p_to = po[REQUEST_TO] as Player;
                    if (!friendList.Contains(p_from.ObjectId))
                    {
                        friendList.Add(p_from.ObjectId);
                    }
                    if (!friendList.Contains(p_to.ObjectId))
                    {
                        friendList.Add(p_to.ObjectId);
                    }
                }
            });
        
        Task findTask = Player.Query.FindAsync().ContinueWith(t => 
        {
            users = t.Result.Cast<Player>();
        });
        while (!findTask.IsCompleted || !friendTask.IsCompleted)
        {
            yield return null;
        }
        if (users != null && users.Count() > 0)
        {
            print("Found " + users.Count() + " users");
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
        //Player current = Player.CurrentUser as Player;

        //current.Get<string>("email");

        //IList<Player> players = null;

        //try
        //{
        //    players = current.Get<IList<Player>>("pinnedPlayers");
        //}
        //catch (Exception e)
        //{
            
        //}
        //if (players == null)
        //{
        //    print("players is null");
        //    players = new List<Player>() { users.LastOrDefault() };
        //}
        //print("count " + players.Count);
        //players.Add(users.FirstOrDefault());
        //current["pinnedPlayers"] = players;
        //current.SaveAsync();

        //current.PinnedPlayers.FetchAllAsync().ContinueWith(t =>
        //{
        //    if (current.PinnedPlayers != null)
        //    {
        //        foreach (Player p in current.PinnedPlayers)
        //        {
        //            print("Player: " + p.Username);
        //        }
        //    }



        //    current.PinnedPlayers.Add(users.LastOrDefault());
        //    current.SaveAsync();
        //    print("Saving pinnedPlayers");
        //});
        

        var children = new List<GameObject>();

        foreach (Transform child in UserPanel.transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));

        UserPanel.Enable();

        IEnumerable<Player> filtered = users.Where(t => t.Username.ToLower().Contains(filter.ToLower())).OrderByDescending(t => t.ObjectId.Equals(Player.CurrentUser.ObjectId))
            .ThenByDescending(t => friendList.Contains(t.ObjectId));
        foreach (Player player in filtered)
        {
            ParseObject friend = null;           
            if (friends != null)
            {
                friend = friends.Where(t => ((Player)t[REQUEST_FROM]).ObjectId.Equals(player.ObjectId) || ((Player)t[REQUEST_TO]).ObjectId.Equals(player.ObjectId)).FirstOrDefault();

            //    friend = friends.FirstOrDefault();
             //   print("Friend: " + ((Player)friend[REQUEST_TO]).ObjectId);
            //    print("Player: " + player.ObjectId);
            }

           // GameObject ListItem = Instantiate(Resources.Load("List Item")) as GameObject;
            dfPanel listItem = UserPanel.AddPrefab(ListItem) as dfPanel; // as UserListItem;
            listItem.Width = UserPanel.Width - UserPanel.FlowPadding.left - UserPanel.FlowPadding.right;

            ListItemExtras extras = listItem.GetComponent<ListItemExtras>();
            extras.Player = player;
            extras.FriendRequest = friend;

            dfLabel username = listItem.Find("Username Label").GetComponent<dfLabel>();
            username.Text = player.Username;
            username.DisabledColor = new Color32(100, 100, 100, 255);

            dfLabel email = listItem.Find("Email label").GetComponent<dfLabel>();
            email.Text = player.Email;
            email.DisabledColor = new Color32(100, 100, 100, 255);

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
            if (friend == null)
            {
                actionButton.Text = "Add";
                actionButton.Click += new MouseEventHandler(actionClick_add);
            }
            else
            {
                string status = friend[STATUS] as string;
                Player from = friend[REQUEST_FROM] as Player;
                if (Player.CurrentUser.ObjectId.Equals(from.ObjectId))
                {
                    switch (status)
                    {
                        case S_ACCEPTED:
                            actionButton.Text = "Remove";
                            actionButton.Click += new MouseEventHandler(actionClick_remove);
                            listItem.BackgroundColor = new Color32(200, 255, 200, 255);
                            break;
                        case S_REJECTED:
                            actionButton.Text = "Sorry";
                            break;
                        case S_PENDING:
                            actionButton.Text = "Pending";
                            actionButton.Disable();
                            break;
                    }
                }
                else
                {
                    switch (status)
                    {
                        case S_ACCEPTED:
                            actionButton.Text = "Remove";
                            actionButton.Click += new MouseEventHandler(actionClick_remove);
                            listItem.BackgroundColor = new Color32(200, 255, 200, 255);
                            break;
                        case S_REJECTED:
                            actionButton.Text = "Rejected";
                            break;
                        case S_PENDING:
                            actionButton.Text = "Accept";
                            actionButton.Click += new MouseEventHandler(actionClick_accept);
                            break;
                    }
                }
               
            }

            if (player.Username.Equals(Player.CurrentUser.Username))
            {
                actionButton.Hide();
            }
        }
    }

    void GenerateList2(string filter)
    {
        var children = new List<GameObject>();

        foreach (Transform child in UserPanel.transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));


        IEnumerable<Player> filtered = users.Where(t => t.Username.ToLower().Contains(filter.ToLower()));
        foreach (Player player in filtered)
        {
            ParseObject friend = null;
            if (friends != null)
            {
                friend = friends.Where(t => ((Player)t[REQUEST_FROM]).ObjectId.Equals(player.ObjectId) || ((Player)t[REQUEST_TO]).ObjectId.Equals(player.ObjectId)).FirstOrDefault();

                //    friend = friends.FirstOrDefault();
                //   print("Friend: " + ((Player)friend[REQUEST_TO]).ObjectId);
                //    print("Player: " + player.ObjectId);
            }

            var panel = UserPanel.AddControl<UserListItem>();
            panel.BackgroundSprite = "BLANK_TEXTURE";
            if (friend != null)
            {
                panel.BackgroundColor = new Color32(255, 229, 255, 255);
            }
            else
            {
                panel.BackgroundColor = new Color32(213, 229, 255, 255);
            }
            panel.Height = 50;
            panel.Width = UserPanel.Width - UserPanel.FlowPadding.left - UserPanel.FlowPadding.right;
            panel.User = player;

            dfLabel nameLabel = panel.AddControl<dfLabel>();
            nameLabel.Text = player.Username;
            nameLabel.AutoSize = true;
            nameLabel.RelativePosition = new Vector3(50, 12);
            nameLabel.Color = new Color32(0, 0, 0, 255);

            dfSprite onlineSprite = panel.AddControl<dfSprite>();
            if (player.IsOnline)
            {
                onlineSprite.SpriteName = "OUYA_O 24px";
            }
            else
            {
                onlineSprite.SpriteName = "OUYA_A 24px";
            }
            onlineSprite.RelativePosition = new Vector3(13, 13);

            if (!player.Username.Equals(Player.CurrentUser.Username))
            {
                var addButton = panel.AddControl<dfButton>();
                addButton.BackgroundSprite = "button-normal";
                addButton.HoverSprite = "button-hilite";
                addButton.PressedSprite = "button-normal";
                addButton.FocusSprite = "button-hilite";
                addButton.DisabledSprite = "button-disabled";
                addButton.Text = "Add";
                addButton.Width = 50;
                addButton.Height = 30;
                addButton.RelativePosition = new Vector3(panel.Width - addButton.Width - 5, 10);

                addButton.Click += new MouseEventHandler(actionClick_add);
            }
        }
    }

    void actionClick_add(dfControl control, dfMouseEventArgs mouseEvent)
    {
        ListItemExtras parent = control.transform.parent.gameObject.GetComponent<ListItemExtras>();
        print("Username: " + parent.Player.Username);

        ParseObject request = new ParseObject(FRIEND_REQUEST);
        request[REQUEST_FROM] = Player.CurrentUser;
        request[REQUEST_TO] = parent.Player;
        request[STATUS] = S_PENDING;

        parent.FriendRequest = request;

        Task saveTask = request.SaveAsync();

        StartCoroutine(WaitForRequest(saveTask));
    }

    void actionClick_remove(dfControl control, dfMouseEventArgs mouseEvent)
    {
        ListItemExtras parent = control.transform.parent.gameObject.GetComponent<ListItemExtras>();

        Task deleteTask = parent.FriendRequest.DeleteAsync();
        StartCoroutine(WaitForRequest(deleteTask));
    }

    void actionClick_accept(dfControl control, dfMouseEventArgs mouseEvent)
    {
        ListItemExtras parent = control.transform.parent.gameObject.GetComponent<ListItemExtras>();

        parent.FriendRequest[STATUS] = S_ACCEPTED;
        Task saveTask = parent.FriendRequest.SaveAsync();
        StartCoroutine(WaitForRequest(saveTask));
    }

    IEnumerator WaitForRequest(Task task)
    {
        UserPanel.Disable();
        while (!task.IsCompleted)
        {
            yield return null;
        }
        
        StartCoroutine(FindUsers());
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
