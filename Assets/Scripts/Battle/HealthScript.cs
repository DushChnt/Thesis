using UnityEngine;
using System.Collections;

public class HealthScript : Photon.MonoBehaviour {

	private float _health;

	public float Health
	{
		get
		{
			return _health;
		}
	}

    public dfFollowObject FollowScript;

	bool isOne = false;

	// Use this for initialization
	void Start () {
		_health = 100;
		isOne = this.gameObject.name.Contains("1");
		if (isOne)
		{
			BattleGUI.Robot1Health = Health;
		}
		else
		{
			BattleGUI.Robot2Health = Health;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void TakeDamage(float damage)
	{
		_health -= damage;
		photonView.RPC("SetHealth", PhotonTargets.OthersBuffered, _health);
        if (photonView.isMine)
        {

        }
		if (isOne)
		{
			BattleGUI.Robot1Health = Health;
		}
		else
		{
			BattleGUI.Robot2Health = Health;
		}
		if (Health <= 0)
		{
		//	Destroy(gameObject);
          //  PhotonNetwork.Destroy(photonView);

            Destroy(FollowScript.gameObject);
            Destroy(FollowScript);
            PhotonNetwork.Destroy(gameObject);
		}
	}

	[RPC]
	void SetHealth(float health)
	{
		_health = health;
        if (photonView.isMine)
        {
            if (Health <= 0)
            {
                Destroy(FollowScript.gameObject);
                Destroy(FollowScript);
                PhotonNetwork.Destroy(gameObject);
                print("Death on " + Time.time);
            }
        }
	}
}
