using UnityEngine;
using System.Collections;
using System;

public class HealthScript : Photon.MonoBehaviour {

	private float _health;
    public ParticleSystem Explosion;
    public dfGUIManager GUI;
    GameObject label;
    public bool IsOpponent;
    public float MaxHealth = 100f;
    bool isDead;

    public delegate void DamageTakenHandler(float damage);
    public event DamageTakenHandler DamageTaken;

    protected virtual void OnDamageTaken(float damage)
    {
        if (DamageTaken != null)
        {
            DamageTaken(damage);
        }
    }

    public bool IsDead {
        get
        {
            return Health <= 0.0f;
        }
    }

    public float GuiHealth
    {
        get
        {
            var h = IsOpponent ? MaxHealth - Health : Health;
            h = (h / MaxHealth) * 100;
            return h;
        }
    }

    public float Health
    {
        get
        {
            return _health;
        }
    }

	public float HealthPercentage
	{
		get
		{
            var h = (Health / MaxHealth) * 100;           
			return h;
		}
	}

    public dfFollowObject FollowScript;

	bool isOne = false;

	// Use this for initialization
	void Start () {
		_health = MaxHealth;
		isOne = this.gameObject.name.Contains("1");
		if (isOne)
		{
			BattleGUI.Robot1Health = Health;
		}
		else
		{
			BattleGUI.Robot2Health = Health;
		}
        if (GUI == null)
        {
            GUI = GameObject.Find("Battle GUI").GetComponent<dfGUIManager>();
        }
        label = (GameObject)Resources.Load("Floating Label", typeof(GameObject));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void ShowFloatingText(float damage)
    {   
        dfTweenVector3 tween = label.GetComponent<dfTweenVector3>();
        //Destroy(tween);
        tween.StartValue = GUI.WorldPointToGUI(gameObject.transform.position) + new Vector2(0, -50); ;
        //print("Pos: " + label.transform.position);
        //dfFollowObject follow = label.GetComponent<dfFollowObject>();
        //follow.attach = gameObject;
        //follow.mainCamera = Camera.main;

        dfLabel glabel = GUI.AddPrefab(label) as dfLabel;

        glabel.RelativePosition = GUI.WorldPointToGUI(gameObject.transform.position) + new Vector2(0, -50);
        if (damage > 0)
        {
            glabel.Text = string.Format("-{0:#.##}!", damage);
            glabel.BottomColor = new Color32(254, 0, 0, 254);
        }
        else if (damage < 0)
        {
            glabel.Text = string.Format("+{0:#.##}!", damage);
            glabel.BottomColor = new Color32(0, 254, 0, 254);
        }
        else
        {
            glabel.Text = "Hit!";
            glabel.BottomColor = new Color32(100, 100, 254, 254);
        }
       
    }

	public void TakeDamage(float damage)
	{
        if (!isDead)
        {
            // print("Taking damage " + damage);

            ShowFloatingText(damage);

            _health -= damage;
            if (_health > MaxHealth)
            {
                _health = MaxHealth;
            }
            photonView.RPC("SetHealth", PhotonTargets.OthersBuffered, _health, damage);
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
            OnDamageTaken(damage);
            if (Health <= 0)
            {
                //	Destroy(gameObject);
                //  PhotonNetwork.Destroy(photonView);
                //   Explosion.Play();
                // print("Play explosion");
                DeathExplosion();
                if (FollowScript != null)
                {
                    Destroy(FollowScript.gameObject);
                    Destroy(FollowScript);
                }
                gameObject.renderer.enabled = false;
                gameObject.SetActive(false);
                //     Destroy(gameObject);
                //     PhotonNetwork.Destroy(gameObject);
            }
        }
	}

    IEnumerator RemoveRobot()
    {
        yield return new WaitForSeconds(3.0f);
        if (this != null && gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    void DeathExplosion()
    {
        if (!isDead)
        {
            Died(this, EventArgs.Empty);
            ParticleSystem ps = Instantiate(Explosion, transform.position + new Vector3(0, 2, 0), Quaternion.identity) as ParticleSystem;
            isDead = true;
            StartCoroutine(RemoveRobot());
        }
       // ps.Play();
    //    Destroy(ps, ps.duration + ps.startLifetime);
    }

    void OnDestroy()
    {
        if (FollowScript != null)
        {
            Destroy(FollowScript.gameObject);
            Destroy(FollowScript);
        }
    }

	[RPC]
	void SetHealth(float health, float damage)
	{
		_health = health;
        ShowFloatingText(damage);
        OnDamageTaken(damage);
        if (photonView.isMine)
        {
            if (Health <= 0)
            {
             //   Explosion.Play();
                DeathExplosion();
                // print("RPC: Play explosion");
                if (FollowScript != null)
                {
                    Destroy(FollowScript.gameObject);
                    Destroy(FollowScript);
                }
                gameObject.renderer.enabled = false;
           //     PhotonNetwork.Destroy(gameObject);
                gameObject.SetActive(false);
                // print("Death on " + Time.time);
                
            }
        }
	}

    public delegate void DeathEventHandler(object sender, EventArgs e);

    public event DeathEventHandler Died;
}
