using UnityEngine;
using System.Collections;

//Script Objective: Unique Behaviours for Iona

public class IonaScript : MonoBehaviour 
{
	private PlayerCombatAI combatActions;

	private int focusedTargetIndex = 0;

	private int ionicFieldDurations = 0;
	public int ionicFieldDurationsMax = 3;
	private Transform ionicFieldOn;
	public Transform ionicfield;

	//AP Costs
	[Range(0,200)]
	public int apDustStorm = 40;
	[Range(0,200)]
	public int apTechBlast = 30;
	[Range(0,200)]
	public int apEmp = 20;
	[Range(0,200)]
	public int apIonicField = 10;
	[Range(0,200)]
	public int apBasic = 5;

	public Transform reviveEffect;

	// Use this for initialization
	void Start () 
	{
		combatActions = transform.parent.gameObject.GetComponent<PlayerCombatAI>();

		focusedTargetIndex = Random.Range (0, 2);
	}

	//Decide Target and Action -- Focus Damage over one enemy, preferably weak to electricity
	public void DecideAction()
	{
		//Turn down Ionic Field 
		if(ionicFieldOn)
		{
			ionicFieldOn.SendMessage ("DrainEnemies", SendMessageOptions.DontRequireReceiver);

			ionicFieldDurations--;

			if(ionicFieldDurations <= 0)
			{
				ionicFieldOn.SendMessage ("DestroyField", SendMessageOptions.DontRequireReceiver);
			}
		}

		//Calculate Target to Focus on
		CalculateTarget ();

		//Calculate Action to take, //1-Basic Attack, 3-Dust Storm, 4-Techblast, 5-Emp, 6-Defend, 7-Ionicfield
		combatActions.attackNumber = ActionToTake ();
		combatActions.anim.SetInteger ("Attack Number", combatActions.attackNumber);

		CombatCamera cam = CombatCamera.control;

		switch(combatActions.attackNumber)
		{
		case 1: 
			combatActions.combatStats.ui.SetGlobalMessage ("Lightning Shot");
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
			combatActions.combatStats.ui.SetGlobalMessage ("Dust Storm");
			if(CombatManager.enemies.Count == 1)
			{
				combatActions.targetIndex = 0;
			}
			else
			{
				combatActions.targetIndex = 1;
			}

			//Call the camera
			cam.SpawnGlobalAnimation (null, "Rear View");
			cam.SetRotateTowards (gameObject);

			combatActions.combatStats.APCost (apDustStorm);
			break;
		case 4:
			combatActions.combatStats.ui.SetGlobalMessage ("Tech Blast");
			combatActions.combatStats.APCost (apTechBlast);

			//Call the camera
			//CombatCamera.control.SpawnGlobalAnimation (null, "View Battlefield", 400);
			cam.CameraReset ();
			cam.SetAnimated (gameObject);
			cam.DelayStop (1.6f);
			cam.SetTransform (transform.position);
			cam.SetPosition(9);
			cam.SetMoveSpeed(2f);
			cam.Truck (1);
			cam.SetDistance (0.8f);
			cam.SetRotateTowards (CombatManager.enemies[combatActions.targetIndex]);

			break;
		case 5:
			combatActions.combatStats.ui.SetGlobalMessage ("EMP Blast");
			combatActions.combatStats.APCost (apEmp);

			//Call the camera
			cam.SpawnGlobalAnimation (null, "Rear View");
			cam.SetRotateTowards (gameObject);

			break;
		case 6:
			//If Defending call defend Function
			combatActions.Defend ();
			break;
		case 7:
			combatActions.combatStats.ui.SetGlobalMessage ("Ionic Field");
			combatActions.combatStats.APCost (apIonicField);
			break;
		case 8:
			combatActions.combatStats.ui.SetGlobalMessage ("Revive");
			break;
		}
	}

	void CalculateTarget()
	{
		//print (focusedTargetIndex);
		//Iona focuses on a single target
		combatActions.targetIndex = focusedTargetIndex;

		if(focusedTargetIndex < CombatManager.enemies.Count)
		{
			combatActions.target = CombatManager.enemies[combatActions.targetIndex]; //Select Random target
		}
		else
		{
			focusedTargetIndex = Random.Range (0, CombatManager.enemies.Count-1);
			combatActions.targetIndex = focusedTargetIndex;
			combatActions.target = CombatManager.enemies[combatActions.targetIndex]; //Select Random target
		}

		if(CombatManager.enemyStats[combatActions.targetIndex].affinityRevealed && 
		   CombatManager.enemyStats[combatActions.targetIndex].affinity != 3)
		{
			focusedTargetIndex = Random.Range (0, CombatManager.enemies.Count-1);
			combatActions.	targetIndex = focusedTargetIndex;
			combatActions.target = CombatManager.enemies[combatActions.targetIndex]; //Select Random target
		}
	}

	//This function calculates which action number to take
	int ActionToTake()
	{
		//1 - Basic Attack, 3 - Dust Storm, 4 - Techblast, 5 - Emp, 6 - Defend, 7 - Ionicfield
		CombatStat stat = combatActions.combatStats.stat;

		bool isNoneShattered = true;	//Figure out if no enemy is shattered
		bool isDusty = true;			//Figure out if at least one enemy has dust
		bool isBattered = true;			//Figure out if at least one enemy is battered
		for(int i = 0; i < CombatManager.enemies.Count; i++)
		{
			//Figure out isNoneShattered
			if(CombatManager.enemyStats[i].affinityRevealed)
			{
				isNoneShattered = false;
			}

			//Figure out is Dusty	
			if(CombatManager.enemyStats[i].elementalReaction.currentElementalDust[0] == null &&
			   CombatManager.enemyStats[i].elementalReaction.currentElementalDust[1] == null &&
			   CombatManager.enemyStats[i].elementalReaction.currentElementalDust[2] == null &&
			   CombatManager.enemyStats[i].elementalReaction.currentElementalDust[3] == null)
			{
				isDusty = false;			
			}

			//Figure out is battered
			if(CombatManager.enemyStats[i].elementalReaction.elementalEffect[3] <= 0)
			{
				isBattered = false;
			}
		}

		//Calculate if whole team is below 30%, equation current health / max health
		bool teamLowAP = false;
		int maxTeamAP = 0;
		int currentTeamAP = 0;
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			maxTeamAP += CombatManager.playerStats[i].stat.actionPointMax;
			currentTeamAP += CombatManager.playerStats[i].stat.actionPoints;
		}
		
		if((float)currentTeamAP / (float)maxTeamAP <= 0.50f)
		{
			teamLowAP = true;
		}

		float empChance = Random.Range (0f,1f);

		//print (isNoneShattered);
		//print (isDusty);
		//print (isBattered);

		//Revive Main Player if dead
		if(CombatUIManager.mainPlayerScript.combatStats.stat.health <= 0)
		{
			return 8;
		}
		//If Ionic field is off and team ap lower than 30%
		else if(!ionicFieldOn && stat.actionPoints > apIonicField && teamLowAP)
		{
			return 7;	//Ionic Field
		}
		//If all enemies affinity not shattered, at least one has no dust and enough AP, perform dust storm
		else if(isNoneShattered && !isDusty && stat.actionPoints > apDustStorm)
		{
			return 3;	//Dust Storm
		}
		//If no enemies shattered and all have dust and enough AP , perform Tech Blast
		else if(isNoneShattered && isDusty && stat.actionPoints > apTechBlast)
		{
			return 4;	//Tech Blast
		}
		//If at least one enemy isn't battered and enough AP, perform EMP
		else if(!isBattered && stat.actionPoints > apEmp && empChance < 0.5f)
		{
			return 5;	//EMP
		}
		//If Enemy isn't weak to electricty and enough AP, perform Basic attack
		else if(CombatManager.enemyStats[combatActions.targetIndex].affinity != 3 && 
			    CombatManager.enemyStats[combatActions.targetIndex].affinityRevealed && 
		        stat.actionPoints > apBasic ||
		        !CombatManager.enemyStats[combatActions.targetIndex].affinityRevealed && 
		        stat.actionPoints > apBasic)
		{
			return 1;	//Basic Attack
		}
		else
		{
			return 6;	//Defend
		}
	}

	//Sets Every Enemy Up with any elemental Dust
	public void DustStorm()
	{
		int element = Random.Range (1,4);

		if(CombatManager.enemies.Count == 1)
		{
			combatActions.targetIndex = 0;
		}
		else
		{
			combatActions.targetIndex = 1;
		}

		bool isDusty = true;			//Figure out if at least one enemy has dust
		for(int i = 0; i < CombatManager.enemies.Count; i++)
		{				
			//Figure out is Dusty
			//Figure out is Dusty	
			if(CombatManager.enemyStats[i].elementalReaction.currentElementalDust[0] == null &&
			   CombatManager.enemyStats[i].elementalReaction.currentElementalDust[1] == null &&
			   CombatManager.enemyStats[i].elementalReaction.currentElementalDust[2] == null &&
			   CombatManager.enemyStats[i].elementalReaction.currentElementalDust[3] == null)
			{
				isDusty = false;			
			}

			print (isDusty);
			if(!isDusty)
			{
				CombatManager.enemyStats[i].SetDamage (combatActions.combatStats.stat,
				                                       element, 0, 0f, 0f);
			}
		}
	}

	//Tech Blast - Single High Electrical Damage
	//Called from Action Script

	//Emp Grenade - Sets Battered on Enemies
	public void Emp()
	{
		for(int i = 0; i < CombatManager.enemies.Count; i++)
		{
			CombatManager.enemyStats[i].elementalReaction.elementalEffect[3] = 1;
			CombatManager.enemyStats[i].ShowDamageText ("Immobilised", Color.white, 0.75f);
		}
	}

	//Non elemental DOT effect gadget on focused enemy
	public void IonicField()
	{
		if(ionicFieldOn)
		{
			ionicFieldOn.SendMessage ("DestroyField", SendMessageOptions.DontRequireReceiver);
		}

		if(ionicfield)
		{
			ionicFieldOn = Instantiate (ionicfield, Vector3.zero, Quaternion.identity) as Transform;
		}

		ionicFieldDurations = ionicFieldDurationsMax;
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
