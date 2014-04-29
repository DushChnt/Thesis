using UnityEngine;
using System.Collections;

public class HealthScript : MonoBehaviour {

    private float _health;

    public float Health
    {
        get
        {
            return _health;
        }
    }

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
        if (isOne)
        {
            BattleGUI.Robot1Health = Health;
        }
        else
        {
            BattleGUI.Robot2Health = Health;
        }
        if (Health < 0)
        {
            Destroy(gameObject);
        }
    }
}
