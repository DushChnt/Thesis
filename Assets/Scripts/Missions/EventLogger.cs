using UnityEngine;
using System.Collections;
using Parse;
using System.Collections.Generic;

public class EventLogger : MonoBehaviour {

    public MissionUI UI;
    public string Arena;
   
    Player Player
    {
        get
        {
            return ParseUser.CurrentUser as Player;
        }
    }

    Brain activeBrain;   
    Match match;
    Frame currentFrame;
    float time;
    IList<ParseObject> frames;
    bool IsActive;
    BrainEvent currentBrainEvent;

    private float damageTaken, damageGiven;

	// Use this for initialization
	void Start () {
        frames = new List<ParseObject>();       
        match = new Match();      
        currentFrame = new Frame();
        frames.Add(currentFrame);       
        match["frames"] = frames;
        match.SaveAsync().ContinueWith(t =>
        {         
            currentFrame["match"] = match;
            currentFrame.SaveAsync();
        });

        UI.Controller.MeleeAttackEvent += new FightController.AttackEventHandler(Controller_MeleeAttackEvent);
        UI.Controller.HitMelee += new FightController.HitEventHandler(Controller_HitMelee);
        UI.Controller.RangedAttackEvent += new FightController.AttackEventHandler(Controller_RangedAttackEvent);
        UI.Controller.HitRanged += new FightController.HitEventHandler(Controller_HitRanged);
        UI.Controller.MortarAttackEvent += new FightController.AttackEventHandler(Controller_MortarAttackEvent);
        UI.Controller.HitMortar += new FightController.HitEventHandler(Controller_HitMortar);
        UI.Controller.BrainSwitched += new FightController.SwitchBrainEventHandler(Controller_BrainSwitched);

        UI.Controller.HealthScript.DamageTaken += new HealthScript.DamageTakenHandler(HealthScript_DamageTaken);

        UI.UIStartPressed += new MissionUI.UIEventHandler(UI_UIStartPressed);
	}

    void UI_UIStartPressed()
    {
        IsActive = true;

        GetBrainSwitch(UI.Controller.GetActiveBrain());
    }

    void GetBrainSwitch(int number)
    {
        Brain b = null;
        switch (number)
        {
            case 1:
                b = Player.Brain1;
                break;
            case 2:
                b = Player.Brain2;
                break;
            case 3:
                b = Player.Brain3;
                break;
            case 4:
                b = Player.Brain4;
                break;
        }
        if (b != null)
        {
            activeBrain = b;
          //  CreateEvent(SWITCH_BRAIN, 0, 0);
            CreateBrainEvent();
        }
    }

    void HealthScript_DamageTaken(float damage)
    {
      //  CreateEvent(TAKE_DAMAGE, 0, damage);

        currentBrainEvent.DamageTaken += damage;
        damageTaken += damage;
    }

    void Controller_HitMortar(float damage, float ownDamage)
    {
     //   CreateEvent(MORTAR_HIT, damage, ownDamage);

        currentBrainEvent.MortarHits++;
        currentBrainEvent.MortarDamage += damage;
        currentBrainEvent.MortarOwnDamage += ownDamage;
        damageGiven += damage;
    }

    void Controller_HitRanged(float damage, float ownDamage)
    {
     //   CreateEvent(RANGED_HIT, damage, ownDamage);

        currentBrainEvent.RangedHits++;
        currentBrainEvent.RangedDamage += damage;
        damageGiven += damage;
    }

    void Controller_HitMelee(float damage, float ownDamage)
    {
      //  CreateEvent(MELEE_HIT, damage, ownDamage);

        currentBrainEvent.MeleeHits++;
        currentBrainEvent.MeleeDamage += damage;
        damageGiven += damage;
    }

    void Controller_BrainSwitched(int number)
    {
        GetBrainSwitch(number);
    }    

    void Controller_MortarAttackEvent()
    {
      //  CreateEvent(MORTAR_ATTACK, 0, 0);

        currentBrainEvent.MortarAttacks++;
    }

    void Controller_RangedAttackEvent()
    {
     //   CreateEvent(RANGED_ATTACK, 0, 0);

        currentBrainEvent.RangedAttacks++;
    }

    void Controller_MeleeAttackEvent()
    {
    //    CreateEvent(MELEE_ATTACK, 0, 0);

        currentBrainEvent.MeleeAttacks++;
    }

    void UpdateBrainEvent()
    {
        if (currentBrainEvent != null)
        {
            currentBrainEvent.PlayerHealth = UI.Controller.HealthScript.Health;
            currentBrainEvent.OpponentHealth = UI.Controller.OpponentHealth.Health;
            currentBrainEvent.Duration = time - currentBrainEvent.Time;
            currentBrainEvent.SaveAsync();
        }
    }

    void CreateBrainEvent()
    {
        UpdateBrainEvent();  

        currentBrainEvent = new BrainEvent();
        currentBrainEvent.Player = Player;
        currentBrainEvent.Frame = currentFrame;
        currentBrainEvent.ActiveBrain = activeBrain;
        currentBrainEvent.Arena = this.Arena;
        currentBrainEvent.Time = time;

        currentBrainEvent.SaveAsync();
    }

    //void CreateEvent(string action, float damageGiven, float damageTaken)
    //{
    // //   CreateEvent(action, this.Arena, this.activeBrain, damageGiven, damageTaken, UI.Controller.HealthScript.Health, UI.Controller.OpponentHealth.Health, time);
    //}

    //void CreateEvent(string action, string arena, Brain activeBrain, float damageGiven, float damageTaken, float currentHealth, float opponentHealth, float time)
    //{
    //    BattleEvent be = new BattleEvent();
    //    be.Action = action;
    //    be.Arena = arena;
    //    be.ActiveBrain = activeBrain;
    //    be.DamageGiven = damageGiven;
    //    be.DamageTaken = damageTaken;
    //    be.CurrentHealth = currentHealth;
    //    be.OpponentHealth = opponentHealth;
    //    be.Time = time;
    //    be.Frame = currentFrame;

    //    be.SaveAsync();
    //}

	// Update is called once per frame
	void Update () {
        if (IsActive)
        {
            time += Time.deltaTime;
        }
	}

    void OnDestroy()
    {
        print("DestructinO!");
        UpdateBrainEvent();

    }
}
