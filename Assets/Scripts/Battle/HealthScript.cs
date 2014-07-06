using UnityEngine;
using System.Collections;
using System;

public class HealthScript : Photon.MonoBehaviour {

	private float _health;
    public ParticleSystem Explosion;

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
        print("Taking damage " + damage);
        
		_health -= damage;
        if (_health > 100)
        {
            _health = 100;
        }
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
         //   Explosion.Play();
            print("Play explosion");
            DeathExplosion();
            if (FollowScript != null)
            {
                Destroy(FollowScript.gameObject);
                Destroy(FollowScript);
            }
       //     PhotonNetwork.Destroy(gameObject);
		}
	}

    void DeathExplosion()
    {
        Died(this, EventArgs.Empty);
        ParticleSystem ps = Instantiate(Explosion, transform.position + new Vector3(0, 2, 0), Quaternion.identity) as ParticleSystem;
        ps.Play();
        Destroy(ps, ps.duration + ps.startLifetime);
    }

	[RPC]
	void SetHealth(float health)
	{
		_health = health;
        if (photonView.isMine)
        {
            if (Health <= 0)
            {
             //   Explosion.Play();
                DeathExplosion();
                print("RPC: Play explosion");
                if (FollowScript != null)
                {
                    Destroy(FollowScript.gameObject);
                    Destroy(FollowScript);
                }
                PhotonNetwork.Destroy(gameObject);
               
                print("Death on " + Time.time);
                
            }
        }
	}

    public delegate void DeathEventHandler(object sender, EventArgs e);

    public event DeathEventHandler Died;
}
