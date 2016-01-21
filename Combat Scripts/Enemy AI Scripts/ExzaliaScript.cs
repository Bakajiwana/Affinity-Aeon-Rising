using UnityEngine;
using System.Collections;

public class ExzaliaScript : MonoBehaviour 
{
	private EnemyCombatActions combatActions;

	//1 - Basic Attack, 2 - Dark Neon, 3 - Neon Blast, 4 - Corruption, 5 - Summon, 6 - Consume, 8 - Flee, 9 - Death Warning

	public int darkNeonMaxCountDown = 3;
	private int darkNeonCountDown;

	//Dark Neon Combo, after casting dark neon - corruption - then neon blast - summon - then countdown till consume

	public int neonBlastMaxCountDown = 2;
	private int neonBlastCountDown;

	public int corruptionMaxCountDown = 1;
	private int corruptionCountDown;
	[Range(0f,1f)]
	public float corruptionRate = 0.25f;

	public int consumeMaxCountDown = 3;
	private int consumeCountDown;
	[Range(0f,1f)]
	public float consumeHealRate = 0.25f;

	public int summonmMaxCountDown = 1;
	private int summonCountDown;

	//Spawns
	public Transform[] minionPresets;
	public int[] minionLevel;
	public Transform[] minionSpawnPoints;
	public Transform minionSpawnParticles;

	public bool fleeWhenShattered = false;
	public Transform fleeEffect;

	public Transform darkNeonEffect;
	private Transform darkNeon;
	public Transform darkNeonDisperseEffect;

	public Transform shedSkinEffect;
	
	// Use this for initialization
	void Start () 
	{
		combatActions = transform.parent.gameObject.GetComponent<EnemyCombatActions>();

		//Set Variable timers
		SetCountDowns();
	}
	
	//Decide Target and Action
	public void DecideAction()
	{
		//Calculate Action to take
		combatActions.attackNumber = ActionToTake ();
		combatActions.anim.SetInteger ("Attack Number", combatActions.attackNumber);
		
		CombatCamera cam = CombatCamera.control;

		//1 - Basic Attack, 2 - Dark Neon, 3 - Neon Blast, 4 - Corruption, 5 - Summon, 6 - Consume, 8 - Flee, 9 - Death Warning
		switch(combatActions.attackNumber)
		{
		case 1: //Basic Attack
			combatActions.combatStats.ui.SetGlobalMessage ("Lancing Shot");

			CameraShot ();
			break;
		case 2: //Dark Neon
			combatActions.combatStats.ui.SetGlobalMessage ("Dark Neon");

			CameraShot (1);
			break;
		case 3: //Neon Blast
			combatActions.combatStats.ui.SetGlobalMessage ("Neon Blast");

			cam.SpawnGlobalAnimation (null, "Rear Zoom");
			break;
		case 4: //Corruption
			combatActions.combatStats.ui.SetGlobalMessage ("Corruption");

			cam.SpawnGlobalAnimation (null, "View Battlefield");
			break;
		case 5: //Summon
			combatActions.combatStats.ui.SetGlobalMessage ("Summon");
			cam.SpawnGlobalAnimation (null, "Enemies");
			break;
		case 6: //Consume
			combatActions.combatStats.ui.SetGlobalMessage ("Consume");

			cam.SpawnGlobalAnimation (null, "View Battlefield");
			break;
		case 8: //Flee
			combatActions.combatStats.ui.SetGlobalMessage ("Flee");

			CameraShot (1);
			break;
		case 9: //Death Warning
			combatActions.combatStats.ui.SetGlobalMessage ("Annihilation Imminent");

			cam.SpawnGlobalAnimation (null, "View Battlefield");
			break;
		}
	}
	
	int ActionToTake()
	{
		//If only one team mate left leave the fight and player health lower than 50% leave the fight
		if(CombatManager.players.Count == 1 && fleeWhenShattered)
		{
			float lastPlayerHealth = (float)CombatManager.playerStats[0].stat.health / (float)CombatManager.playerStats[0].stat.healthMax;

			if(lastPlayerHealth <= 0.85f)
			{
				return 8; //Leave the fight
			}
		}

		float currentHealthPercentage = (float)combatActions.combatStats.stat.health / (float)combatActions.combatStats.stat.healthMax;
		//If about to die
		if(currentHealthPercentage <= 0.25f && fleeWhenShattered)
		{
			return 8;
		}
		//After 3 turns Lancing Shot
		else if(darkNeonCountDown > 0)
		{
			darkNeonCountDown--;

			if(darkNeonCountDown <= 0)
			{
				return 2; //Initiate Dark Neon
			}
			else
			{
				return 1; //Basic Attack
			}
		}
		else if (corruptionCountDown > 0)
		{
			corruptionCountDown--;

			if(corruptionCountDown <= 0)
			{
				return 4; // Initiate Corruption
			}
			else
			{
				return 1; //Basic Attack
			}
		}
		else if (neonBlastCountDown > 0)
		{
			neonBlastCountDown--;

			if(neonBlastCountDown <= 0)
			{
				return 3; //Initiate Neon Blast
			}
			else
			{
				return 9; //Death Warning
			}
		}
		else if (summonCountDown > 0)
		{
			summonCountDown--;

			if(summonCountDown <= 0)
			{
				if(currentHealthPercentage <= 0.70f && fleeWhenShattered
				   && combatActions.combatStats.affinityRevealed)
				{
					return 8;
				}

				return 5; //Initiate Summon
			}
			else
			{
				return 1; //Basic Attack
			}
		}
		else if(consumeCountDown > 0)
		{
			consumeCountDown--;

			if(consumeCountDown <= 0)
			{
				SetCountDowns (); //Reset CountDowns
				return 6; // Initiate Consume
			}
			else
			{
				return 1;
			}
		}
		//Basic Attack
		return 1;
	}

	void SetCountDowns()
	{
		darkNeonCountDown = darkNeonMaxCountDown;
		consumeCountDown = consumeMaxCountDown;
		corruptionCountDown = corruptionMaxCountDown;
		neonBlastCountDown = neonBlastMaxCountDown;
		summonCountDown = summonmMaxCountDown;
	}

	public void AnimTrigger(string _text)
	{
		combatActions.combatStats.anim.SetTrigger (_text);
	}

	//Dark Neon - Shrouds the battlefield in dark black neon fog that makes it impossible for attacks to hit Exzalia
	public void DarkNeon()
	{
		if(darkNeonEffect)
		{
			//If Dark neon already exists, destroy it
			if(darkNeon)
			{
				Destroy (darkNeon.gameObject);
				Instantiate (darkNeonDisperseEffect, transform.position, transform.rotation);	//Spawn dispersal
			}

			//Create dark neon
			darkNeon = Instantiate (darkNeonEffect, transform.position, transform.rotation) as Transform;
		}

		//Buffs Exzalia's Agility Stat
		combatActions.combatStats.stat.agility = 10000;
	}

	//Corruption - Disperses the Dark Neon and it's effect and debuffs teams stats
	public void Corruption()
	{
		//Disperse the Dark Neon
		if(darkNeon)
		{
			Destroy (darkNeon.gameObject);
			Instantiate (darkNeonDisperseEffect, transform.position, transform.rotation);	//Spawn dispersal
		}

		//Return Exzalia's stat to normal
		combatActions.combatStats.stat.agility = combatActions.combatStats.stat.agilityBase;

		//Debuff all players
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			//Debuff Attack
			CombatManager.playerStats[i].stat.attack = CombatManager.playerStats[i].stat.attackBase;
			CombatManager.playerStats[i].stat.attack -= (int)((float)CombatManager.playerStats[i].stat.attackBase * corruptionRate);
			CombatManager.playerStats[i].ShowDamageText ("Attack Down", Color.grey, 1f);
			//Debuff Defence
			CombatManager.playerStats[i].stat.defence = CombatManager.playerStats[i].stat.defenceBase;
			CombatManager.playerStats[i].stat.defence -= (int)((float)CombatManager.playerStats[i].stat.defenceBase * corruptionRate);
			CombatManager.playerStats[i].ShowDamageText ("Defence Down", Color.grey, 1f);
			//Debuff Luck
			CombatManager.playerStats[i].stat.luck = CombatManager.playerStats[i].stat.luckBase;
			CombatManager.playerStats[i].stat.luck -= (int)((float)CombatManager.playerStats[i].stat.luckBase * corruptionRate);
			CombatManager.playerStats[i].ShowDamageText ("Luck Down", Color.grey, 1f);
			//Decrease AP
			CombatManager.playerStats[i].stat.actionPoints -= (int)((float)CombatManager.playerStats[i].stat.actionPoints * corruptionRate);
			CombatManager.playerStats[i].ShowDamageText ("AP Drained", Color.grey, 1f);
		}

		ShedSkin();
	}


	//Neon Blast - Charges up for a heavy AOE Attack
	public void NeonBlast()
	{
		//Hit all players
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			//Damage
			combatActions.targetIndex = i;
			combatActions.SetPlayerDamage ();
		}
	}

	//Summon - Spawn Minions from Teleporter
	public void Summon()
	{
		for(int i = 0; i < minionSpawnPoints.Length; i++)
		{
			Transform minion = Instantiate (minionPresets[i], minionSpawnPoints[i].position, minionSpawnPoints[i].rotation) as Transform;
			if(minionSpawnParticles)
			{
				Instantiate (minionSpawnParticles, minion.position, minion.rotation);
			}

			//Add to enemy list in combat manager
			GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("InsertEnemy", minion.gameObject, SendMessageOptions.DontRequireReceiver);
			minion.SendMessage ("InitiateEnemyStats", minionLevel[i], SendMessageOptions.DontRequireReceiver);
		}
	}

	//Consume - Sacrifice and Consume all spawned enemies
	public void Consume()
	{
		int maxEnemies = CombatManager.enemies.Count - 1;
		for(int i = 0; i < maxEnemies; i++)
		{
			CombatManager.enemyStats[0].EnemyDown ();
			if(shedSkinEffect)
			{
				Instantiate (shedSkinEffect, CombatManager.enemies[1].transform.position, CombatManager.enemies[1].transform.rotation);
			}

			//Heal
			combatActions.combatStats.SetHeal (true, consumeHealRate);
		}

		ShedSkin ();
	}

	public void Flee()
	{
		//Summon();

		if(fleeEffect)
		{
			Instantiate (fleeEffect,transform.position,transform.rotation);
		}

		combatActions.combatStats.EnemyDown ();
	}

	public void CameraShot(int _index)
	{
		CombatCamera cam = CombatCamera.control;

		switch(_index)
		{
		case 0:
			//Call Camera
			cam.CameraReset ();
			cam.ScreenEffect (0);
			cam.SetTransform (CombatManager.players[combatActions.targetIndex].transform.position);
			cam.SetRotateTowards (gameObject);
			cam.SetDistance (1.5f);
			//cam.DelayStop (1.2f);
			cam.SetPosition (1);
			cam.Truck (1);
			cam.SetMoveSpeed (0.5f);
			cam.DelayStop (1f);
			//cam.SetMoveSpeed (5f);
			//cam.increaseMoveSpeed = true;
			//cam.Zoom (-1);
			break;
		case 1:
			//Call Camera
			cam.CameraReset ();
			cam.ScreenEffect (0);
			cam.SetTransform (CombatManager.players[combatActions.targetIndex].transform.position);
			cam.SetRotateTowards (gameObject);
			cam.SetDistance (1.2f);
			cam.DelayStop (1.2f);
			cam.SetPosition (10);
			cam.SetMoveSpeed (5f);
			cam.increaseMoveSpeed = true;
			cam.Zoom (-1);
			break;
		}
	}

	public void CameraShot()
	{
		CameraShot (0);
	}

	public void ShedSkin()
	{
		//Remove all debuff
		combatActions.combatStats.stat.attack = combatActions.combatStats.stat.attackBase;			
		combatActions.combatStats.stat.defence = combatActions.combatStats.stat.defenceBase;			
		combatActions.combatStats.stat.agility = combatActions.combatStats.stat.agilityBase;			
		combatActions.combatStats.stat.luck = combatActions.combatStats.stat.luckBase;			
		combatActions.combatStats.stat.accuracy = combatActions.combatStats.stat.accuracyBase;			
		combatActions.combatStats.stat.speed = combatActions.combatStats.stat.speedBase;	
		
		if(shedSkinEffect)
		{
			Instantiate (shedSkinEffect, transform.position, transform.rotation);
		}
		
		combatActions.combatStats.ShowDamageText ("Stat Restored", Color.white, 1f);
	}
}
