using UnityEngine;
using System.Collections;

public class TavenScript : MonoBehaviour 
{
	private PlayerCombatAI combatActions;

	//Garazquata chance
	[Range(0,100)]
	public int chanceGarazquata = 25;

	//Terraquata chance
	[Range(0,100)]
	public int chanceTerraquata = 40;

	//AP Costs
	[Range(0,200)]
	public int apGarazquata = 30;
	[Range(0,200)]
	public int apTerraquata = 25;
	[Range(0,200)]
	public int apCurseOfZarra = 15;
	[Range(0,200)]
	public int apHealHydra = 20;
	[Range(0,200)]
	public int apBasic = 2;

	public Transform comet;

	public Transform reviveEffect;

	// Use this for initialization
	void Start () 
	{
		combatActions = transform.parent.gameObject.GetComponent<PlayerCombatAI>();
	}

	//Decide Target and Action
	public void DecideAction()
	{
		//Calculate Action to take
		combatActions.attackNumber = ActionToTake ();
		combatActions.anim.SetInteger ("Attack Number", combatActions.attackNumber);

		CombatCamera cam = CombatCamera.control;
		
		switch(combatActions.attackNumber)
		{
		case 1: 
			combatActions.combatStats.ui.SetGlobalMessage ("Aquata");
			combatActions.combatStats.APCost (apBasic);

			//Call Camera
			cam.CameraReset ();
			cam.ScreenEffect (0);
			cam.SetTransform (CombatManager.enemies[combatActions.targetIndex].transform.position);
			cam.SetRotateTowards (gameObject);
			cam.SetDistance (1.2f);
			cam.DelayStop (1.2f);
			cam.SetPosition (10);
			cam.SetMoveSpeed (5f);
			cam.increaseMoveSpeed = true;
			cam.Zoom (-1);

			break;
		case 3:
			combatActions.combatStats.ui.SetGlobalMessage ("Garazquata");
			combatActions.combatStats.APCost (apGarazquata);

			//Call Camera
			cam.SpawnGlobalAnimation (null, "Rear Zoom");

			break;
		case 4:
			combatActions.combatStats.ui.SetGlobalMessage ("Terraquata");
			combatActions.combatStats.APCost (apTerraquata);

			//Call Camera
			cam.SpawnGlobalAnimation (null, "Rear View");

			break;
		case 5:
			combatActions.combatStats.ui.SetGlobalMessage ("Curse of Zarra");
			combatActions.combatStats.APCost (apCurseOfZarra);

			//Call Camera
			cam.SpawnGlobalAnimation (null, "Rear Zoom");

			break;
		case 6:
			//If Defending call defend Function
			combatActions.Defend ();
			break;
		case 7:
			combatActions.combatStats.ui.SetGlobalMessage ("Heal Hydra");
			combatActions.combatStats.APCost (apHealHydra);

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

		//Calculate chances
		int garazChance = Random.Range (0,100);
		int terraChance = Random.Range (0,100);

		//Calculate if whole team is below 30%, equation current health / max health
		bool teamLowHealth = false;
		int maxTeamHealth = 0;
		int currentTeamHealth = 0;
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			maxTeamHealth += CombatManager.playerStats[i].stat.healthMax;
			currentTeamHealth += CombatManager.playerStats[i].stat.health;
		}

		if((float)currentTeamHealth / (float)maxTeamHealth <= 0.30f)
		{
			teamLowHealth = true;
		}

		//Calculate if all enemies shattered
		bool isNoneShattered = true;	//Figure out if no enemy is shattered

		for(int i = 0; i < CombatManager.enemies.Count; i++)
		{
			//Figure out isNoneShattered
			if(CombatManager.enemyStats[i].affinityRevealed)
			{
				isNoneShattered = false;
			}
		}

		//Revive Main Player if dead
		if(CombatUIManager.mainPlayerScript.combatStats.stat.health <= 0)
		{
			return 8;
		}
		//7 - Heal Hydra, if whole team is below 30% health and enough AP
		else if(teamLowHealth && stat.actionPoints > apHealHydra)
		{
			return 7;
		}
		//3 - Garazquata, if within chance and all enemies not shattered and enough AP
		else if (chanceGarazquata <= garazChance && isNoneShattered && stat.actionPoints > apGarazquata)
		{
			return 3;
		}
		//4 - Terraquata, if within chance and all enemies not shattered and enough AP
		else if(chanceTerraquata <= terraChance && isNoneShattered && stat.actionPoints > apTerraquata)
		{
			return 4;
		}
		//5 - Curse of Zarra, if all enemies not shattered and enough AP
		else if(isNoneShattered && stat.actionPoints > apCurseOfZarra)
		{
			return 5;
		}
		//1 - Basic Attack, if enough AP
		else if(CombatManager.enemyStats[combatActions.targetIndex].affinity != 4 && 
		        CombatManager.enemyStats[combatActions.targetIndex].affinityRevealed && 
		        stat.actionPoints > apBasic ||
		        !CombatManager.enemyStats[combatActions.targetIndex].affinityRevealed && 
		        stat.actionPoints > apBasic)
		{
			return 1;
		}
		//6 - Defend
		else
		{
			return 6;
		}
	}

	//Water Elemental AOe Blast if no one is shattered
	public void Garazquata()
	{
		for(int i = 0; i < CombatManager.enemies.Count; i++)
		{
			combatActions.targetIndex = i;
			combatActions.SetEnemyDamage ();
		}
	}

	//Meteor Call, can use any element
	public void Terraquata()
	{
		for(int i = 0; i < CombatManager.enemies.Count; i++)
		{
			combatActions.targetIndex = i;
			Transform cometStrike = Instantiate (comet.transform, CombatManager.enemies[i].transform.position, CombatManager.enemies[i].transform.rotation) as Transform;
			Transform[] cometElement = cometStrike.gameObject.GetComponentsInChildren<Transform>();

			for(int c = i; c < 4; c++)
			{
				cometElement[c].gameObject.SetActive (false);
			}

			cometElement[i].gameObject.SetActive (true);
		}

		Invoke ("DelayDamage", 1f);
	}

	void DelayDamage()
	{
		for(int i = 0; i < CombatManager.enemies.Count; i++)
		{
			combatActions.targetIndex = i;
			combatActions.SetEnemyDamage ();
		}
	}

	//AOe that causes Debuff
	public void CurseOfZarra()
	{
		for(int i = 0; i < CombatManager.enemies.Count; i++)
		{
			combatActions.targetIndex = i;
			combatActions.SetEnemyDamage ();
		}
	}

	//Double Element Attack on non shattered enemies, to set and react at once
	//Using Action script

	//If Whole Team Health is below 30% heal everyone
	public void HealHydra()
	{
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			CombatManager.playerStats[i].SetHeal (true, 0.30f);
			CombatManager.playerStats[i].ShowDamageText ("Heal", Color.white, 1f);
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
