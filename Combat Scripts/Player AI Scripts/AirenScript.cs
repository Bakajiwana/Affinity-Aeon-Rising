using UnityEngine;
using System.Collections;

public class AirenScript : MonoBehaviour 
{
	private PlayerCombatAI combatActions;

	public int maxBlazearraCharge = 1;
	private int curBlazearraCharge = 0;

	//APCost
	[Range(0,200)]
	public int apRally = 10;
	[Range(0,200)]
	public int apFocus = 10;
	[Range(0,200)]
	public int apSynergy = 30;
	[Range(0,200)]
	public int apBlazearra = 5;
	[Range(0,200)]
	public int apBasic = 0;

	public Transform reviveEffect;
	
	// Use this for initialization
	void Start () 
	{
		combatActions = transform.parent.gameObject.GetComponent<PlayerCombatAI>();
	}

	//Decide Target and Action
	public void DecideAction()
	{
		//1-Blazear, 2-DecideAction, 3-Rally, 4-Focus, 0-TriolynStance, 5-Blazearra, 6-Defend, 7-Synergy
		//Calculate Action to take
		combatActions.attackNumber = ActionToTake ();
		combatActions.anim.SetInteger ("Attack Number", combatActions.attackNumber);

		CombatCamera cam = CombatCamera.control;
		
		switch(combatActions.attackNumber)
		{
		case 0:
			TriolynStance ();
			combatActions.combatStats.ui.SetGlobalMessage ("Triolyn Stance");
			break;
		case 1: 
			combatActions.combatStats.ui.SetGlobalMessage ("Blazear");
			combatActions.combatStats.APCost (apBasic);

			//Call Camera
			cam.CameraReset ();
			cam.ScreenEffect (0);
			cam.SetTransform (CombatManager.enemies[combatActions.targetIndex].transform.position);
			cam.SetRotateTowards (gameObject);
			cam.SetDistance (1.2f);
			cam.DelayStop (2f);
			cam.SetPosition (10);
			cam.SetMoveSpeed (2f);
			cam.increaseMoveSpeed = true;
			cam.Zoom (-1);

			break;
		case 3:
			combatActions.combatStats.ui.SetGlobalMessage ("Rally");
			combatActions.combatStats.APCost (apRally);

			cam.SpawnGlobalAnimation (null, "View Players");
			cam.SetMoveSpeed (2f);
			cam.DelayStop (2.5f);
			//cam.Zoom (1);

			break;
		case 4:
			combatActions.combatStats.ui.SetGlobalMessage ("Focus");
			combatActions.combatStats.APCost (apFocus);

			cam.SpawnGlobalAnimation (null, "View Players");
			cam.SetMoveSpeed (2f);
			cam.DelayStop (2.5f);
			//cam.Zoom (1);

			break;
		case 5:
			if(curBlazearraCharge >= maxBlazearraCharge)
			{
				combatActions.combatStats.ui.SetGlobalMessage ("Blazearra");
				combatActions.combatStats.APCost (apBlazearra);

				//Call the camera
				//CombatCamera.control.SpawnGlobalAnimation (null, "View Battlefield", 400);
				cam.CameraReset ();
				cam.SetAnimated (gameObject);
				cam.DelayStop (2f);
				cam.SetTransform (transform.position);
				cam.SetPosition(9);
				cam.SetMoveSpeed(2f);
				cam.Truck (1);
				cam.SetDistance (0.8f);
				cam.SetRotateTowards (CombatManager.enemies[combatActions.targetIndex]);
			}
			break;
		case 6:
			//If Defending call defend Function
			combatActions.Defend ();
			break;
		case 7:
			combatActions.combatStats.ui.SetGlobalMessage ("Synergy");
			combatActions.combatStats.APCost (apSynergy);

			cam.SpawnGlobalAnimation (null, "View Players");

			break;
		case 8:
			combatActions.combatStats.ui.SetGlobalMessage ("Revive");
			break;
		}

	}

	int ActionToTake()
	{
		CombatStat stat = combatActions.combatStats.stat;

		//Calculate if whole team is below 30%, equation current health / max health
		bool teamLowHealth = false;
		int maxTeamHealth = 0;
		int currentTeamHealth = 0;

		bool allAttBuffed = true;
		float buffChance = Random.Range (0f,1f);
		bool allLckBuffed = true;
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			maxTeamHealth += CombatManager.playerStats[i].stat.healthMax;
			currentTeamHealth += CombatManager.playerStats[i].stat.health;

			//Calculate Attack Stat
			if(CombatManager.playerStats[i].stat.attack <= CombatManager.playerStats[i].stat.attackBase)
			{
				allAttBuffed = false;
			}
			//Calculate Luck Stat
			if(CombatManager.playerStats[i].stat.luck <= CombatManager.playerStats[i].stat.luckBase)
			{
				allLckBuffed = false;
			}
		}
		
		if((float)currentTeamHealth / (float)maxTeamHealth <= 0.30f)
		{
			teamLowHealth = true;
		}

		//Revive Main Player if dead
		if(CombatUIManager.mainPlayerScript.combatStats.stat.health <= 0)
		{
			return 8;
		}
		else if(curBlazearraCharge == maxBlazearraCharge && stat.actionPoints > apBlazearra)
		{
			return 5;
		}

		//7 - Synergy - If Whole team health is less than 30%
		if(teamLowHealth && stat.actionPoints > apSynergy)
		{
			return 7;
		}
		//0- Triolyn Stance - If AP is below 30% and team health low
		else if((float)stat.actionPoints / (float)stat.actionPointBase <= 0.30f && teamLowHealth)
		{
			return 0;
		}
		//3- Rally - If a team members attack is not above base
		else if(!allAttBuffed && stat.actionPoints > apRally && buffChance < 0.5f)
		{
			return 3;
		}
		//4 - Focus - If a team members luck is not above base
		else if(!allLckBuffed && stat.actionPoints > apFocus &&  buffChance < 0.5f)
		{
			return 4;
		}
		//5 - Blazearra - If in Triolyn Stance and enough AP and enemy not weak to fire
		else if(combatActions.anim.GetBool ("Triolyn Stance") &&
		        CombatManager.enemyStats[combatActions.targetIndex].affinity != 2 && 
		        CombatManager.enemyStats[combatActions.targetIndex].affinityRevealed && 
		        stat.actionPoints > apBlazearra ||
		        !CombatManager.enemyStats[combatActions.targetIndex].affinityRevealed && 
		        combatActions.anim.GetBool ("Triolyn Stance") &&  stat.actionPoints > apBlazearra)
		{
			return 5;
		}
		//1 - Blazear - If enough AP and enemy not weak to fire
		else if (CombatManager.enemyStats[combatActions.targetIndex].affinity != 2 && 
		         CombatManager.enemyStats[combatActions.targetIndex].affinityRevealed && 
		         stat.actionPoints > apBasic ||
		         !CombatManager.enemyStats[combatActions.targetIndex].affinityRevealed && 
		         stat.actionPoints > apBasic)
		{
			return 1;
		}
		//6- Defend
		else
		{
			return 6;
		}
	}

	//3. Rally - Team Attack Buff
	public void Rally()
	{
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			CombatManager.playerStats[i].SetBlazingSpirit (0);
		}
	}

	//4. Focus - Team Luck Buff
	public void Focus()
	{
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			CombatManager.playerStats[i].stat.luck = CombatManager.playerStats[i].stat.luckBase * 2;
			CombatManager.playerStats[i].ShowDamageText ("Luck Up", Color.white, 1f);
		}
	}

	//5. Triolyn Stance - Two Hand Mode - Doubles own attack (activated from the death of an ally or own health below 30%
	public void TriolynStance()
	{
		combatActions.anim.SetBool ("Triolyn Stance", true);
		combatActions.anim.SetInteger ("Attack Number", 5);
		combatActions.attackNumber = 5;
	}

	//6. Blazearra - Able to do charged attacks when in two hand mode
	public void ChargeBlazearra()
	{
		if(curBlazearraCharge < maxBlazearraCharge)
		{
			combatActions.combatStats.ui.SetGlobalMessage ("Blazearra Charging " + curBlazearraCharge + "/" + maxBlazearraCharge);
			curBlazearraCharge ++;

			combatActions.EndTurnDelay (2f);
		}
		else
		{
			curBlazearraCharge = 0;
			combatActions.anim.SetTrigger ("Next");
		}
	}

	//7. Synergy - If whole team health is below 30%, buff everything
	public void Synergy()
	{
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			CombatManager.playerStats[i].stat.luck = CombatManager.playerStats[i].stat.luckBase * 2;
			CombatManager.playerStats[i].ShowDamageText ("Luck Up", Color.white, 1f);
			CombatManager.playerStats[i].SetAura (0);
			CombatManager.playerStats[i].SetBlazingSpirit (0);
			CombatManager.playerStats[i].stat.agility = (int)(CombatManager.playerStats[i].stat.agilityBase * 1.3f);
			CombatManager.playerStats[i].ShowDamageText ("Agility Up", Color.white, 1f);
			CombatManager.playerStats[i].stat.speed = (int)(CombatManager.playerStats[i].stat.speedBase * 1.3f);
			CombatManager.playerStats[i].ShowDamageText ("Speed Up", Color.white, 1f);
		}
	}

	public void ReviveMainPlayer()
	{
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("InsertPlayer", CombatUIManager.mainPlayerScript.gameObject, SendMessageOptions.DontRequireReceiver);
		CombatUIManager.mainPlayerScript.combatStats.stat.health = CombatUIManager.mainPlayerScript.combatStats.stat.healthBase /2;
		CombatUIManager.mainPlayerScript.combatStats.stat.actionPoints = CombatUIManager.mainPlayerScript.combatStats.stat.actionPointBase /2;
	
		CombatUIManager.mainPlayerScript.combatStats.anim.SetBool ("Downed", false);

		if(reviveEffect)
		{
			Instantiate (reviveEffect, CombatUIManager.mainPlayerScript.gameObject.transform.position,
			             CombatUIManager.mainPlayerScript.gameObject.transform.rotation);
		}
	}
}
