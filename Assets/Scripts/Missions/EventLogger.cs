using UnityEngine;
using System.Collections;
using Parse;
using System.Collections.Generic;

public class EventLogger {
    
    public string Arena;
   
    Player Player
    {
        get
        {
            return ParseUser.CurrentUser as Player;
        }
    }

    Brain activeBrain;
    int activeBrainNumber = -1;
    Match match;
    Frame currentFrame;
    float time;
    IList<ParseObject> frames;
    bool IsActive;
    BrainEvent currentBrainEvent;
    FightController Controller;

    private float damageTaken, damageGiven;

	// Use this for initialization
    public EventLogger(FightController controller, string arena, Frame frame)
    {
        this.currentFrame = frame;
        this.match = frame.Match;
        this.match.Distance = 1;
        this.frames = new List<ParseObject>();
        this.frames.Add(currentFrame);

        Initialize(controller, arena);
    }

	public EventLogger(FightController controller, string arena) {        
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

        Initialize(controller, arena);

     //   UI.UIStartPressed += new MissionUI.UIEventHandler(UI_UIStartPressed);
	}

    private void Initialize(FightController controller, string arena)
    {
        this.Arena = arena;
        this.Controller = controller;       

        controller.MeleeAttackEvent += new FightController.AttackEventHandler(Controller_MeleeAttackEvent);
        controller.HitMelee += new FightController.HitEventHandler(Controller_HitMelee);
        controller.RangedAttackEvent += new FightController.AttackEventHandler(Controller_RangedAttackEvent);
        controller.HitRanged += new FightController.HitEventHandler(Controller_HitRanged);
        controller.MortarAttackEvent += new FightController.AttackEventHandler(Controller_MortarAttackEvent);
        controller.HitMortar += new FightController.HitEventHandler(Controller_HitMortar);
        controller.BrainSwitched += new FightController.SwitchBrainEventHandler(Controller_BrainSwitched);

        controller.HealthScript.DamageTaken += new HealthScript.DamageTakenHandler(HealthScript_DamageTaken);
    }

    public void StartLogging()
    {
        IsActive = true;

        GetBrainSwitch(Controller.GetActiveBrain());
    }

    public void StopLogging(string outcome)
    {
        if (IsActive)
        {
            UpdateBrainEvent();

            currentFrame.Outcome = outcome;
            currentFrame.DamageGiven = damageGiven;
            currentFrame.DamageTaken = damageTaken;
            currentFrame.Time = time;
            currentFrame.SaveAsync();

            match.Won = outcome.Equals("won");
            match.SaveAsync();

            IsActive = false;
        }
    }

    public void UpdateTime(float deltaTime)
    {
        if (IsActive)
        {            
            time += deltaTime;
        }
    }  

    void GetBrainSwitch(int number)
    {
        if (activeBrainNumber != number)
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
            currentBrainEvent.PlayerHealth = Controller.HealthScript.Health;
            currentBrainEvent.OpponentHealth = Controller.OpponentHealth.Health;
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
	

}
