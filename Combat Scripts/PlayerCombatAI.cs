using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCombatAI : MonoBehaviour 
{	
	//Animator 
	public Animator anim;
	
	[HideInInspector]
	public PlayerCombatCharacter combatStats;
	
	//ATTACKS Variables, based off the animation number
	//This is how it works, the attacks are triggered by the attack number, and the animation does all the work
	[HideInInspector]
	public int attackNumber; //0 = idle, > attack sets
	
	public bool patternOverride = false;
	public int[] attackPattern;
	private int currentPattern = 0;
	
	public int numberOfActions = 1;
	
	public bool[] displayAttackName;
	public string[] attackName;
	
	//Attack Damage
	public bool[] randomDamage;
	
	public int [] minDamage;
	public int [] maxDamage;

	public bool [] randomElement;
	public int [] element;
	
	[Range(1,5)]
	public int[] attackDamage; 
	private float overallDamage;
	
	//Status Effects Chance of Attack, as arrays so attack 1 = attack status effect chance 1 etc
	[Range(0f,1f)]
	public float[] attackStatusEffectChance; 
	
	//Critical Chance of attack
	[Range(0f,1f)]
	public float[] attackCritChance;
	
	//Is attack a projectile
	public int[] attackProjectile;
	
	//Attack Projectiles
	public Transform[] projectileNode;
	public Transform[] projectile; 
	
	//AI Calculations
	[HideInInspector]
	public GameObject target;
	[HideInInspector]
	public int targetIndex;
	
	//Emit Action
	public Transform emitAction;
	private Transform currentEmit;
	
	void Awake()
	{
		//Connect to Stats
		combatStats = gameObject.GetComponent<PlayerCombatCharacter>();
		
		//Turn this script off in case its on at the start
		this.enabled = false;
	}
	
	void Update()
	{
		//-------------------------------------Increase Time during action---------------------------
		
		//If time scale is normal and attack confirmed
		if(Input.GetButtonDown ("Earth") || Input.GetButtonDown ("Submit") ||
		   Input.GetMouseButtonDown (0))
		{
			if(combatStats.ui.skipPrompt.activeInHierarchy)
			{
				Time.timeScale = CombatManager.skipTimeScale;
				combatStats.ui.skipPrompt.SetActive (true);
			}
			else
			{
				Time.timeScale = 1f;
				combatStats.ui.skipPrompt.SetActive (true);
			}
		}
	}
	
	
	void OnEnable()
	{
		//Call Default Camera
		CombatCamera.control.SpawnGlobalAnimation (null, "View Battlefield", 400);
		
		//If Battered
		if(combatStats.elementalReaction.elementalEffect[3] > 0)
		{
			float batteredChance = Random.Range (0f, 1f);
			
			if(batteredChance > 0.5f)
			{
				print ("I'm battered bruh");
				combatStats.ShowDamageText ("Battered", Color.white, 1f);
				EndTurnDelay (0.11f);
			}
			else
			{
				//Snapped out of it
				combatStats.elementalReaction.elementalEffect[3] = 0;
				
				//Re Call OnEnable
				OnEnable ();
			}
		}
		else
		{
			print ("I'm the Player and its my turn woop WOOP");
			
			//Calculate target to attack
			targetIndex = Random.Range (0, CombatManager.enemies.Count);
			target = CombatManager.enemies[targetIndex]; //Select Random target
			
			if(!patternOverride)
			{
				//Calculate what to attack
				attackNumber = Random.Range (1, numberOfActions);
			}
			else
			{
				//Begin Next Pattern
				attackNumber = attackPattern[currentPattern];
				
				//Increment current pattern
				currentPattern ++;
				
				//Reset Pattern if end of pattern
				if(currentPattern >= attackPattern.Length)
				{
					currentPattern = 0;
				}
			}
			
			//Attack
			anim.SetInteger ("Attack Number", attackNumber);
			
			if(displayAttackName[attackNumber - 1])
			{
				//Set Global Message
				combatStats.ui.SetGlobalMessage (attackName[attackNumber - 1]);
			}
			
			if(!randomDamage[attackNumber - 1])
			{
				overallDamage = (((float)attackDamage[attackNumber-1] / 100f) + 1f) * combatStats.stat.attack;
			}
			else
			{
				float damage = Random.Range (minDamage[attackNumber - 1], maxDamage[attackNumber - 1]);
				overallDamage = (damage + 1f) * combatStats.stat.attack;
			}
			
			//Mark as Current
			combatStats.currentTurn = true;
			
			//Reveal Emit Action
			if(emitAction)
			{
				if(currentEmit)
				{
					Destroy (currentEmit.gameObject);
				}
				
				currentEmit = Instantiate (emitAction, transform.position, transform.rotation) as Transform;
				currentEmit.SetParent (transform, true);
			}
		}
	}
	
	public void FireProjectile(int _projectileSlot)
	{
		//Calculate Damage
		float damage = overallDamage;
		
		//Spawn Projectiles and specify information to projectile
		Transform shot = Instantiate (projectile[attackProjectile[attackNumber-1] - 1],
		                              projectileNode[_projectileSlot-1].position,
		                              projectileNode[_projectileSlot-1].rotation) as Transform;
		
		//Calculate Status Chance
		float statusChance = attackStatusEffectChance[attackNumber-1];
		//Get the targets script component and set target
		CombatProjectile trajectory = shot.gameObject.GetComponent<CombatProjectile>();
		trajectory.SetTarget (combatStats.stat, target, (int)damage, false, 
		                      statusChance, attackCritChance[attackNumber-1]);
	}	
	
	public void SetEnemyDamage()
	{
		//Calculate Damage
		if(!randomDamage[attackNumber - 1])
		{
			overallDamage = (((float)attackDamage[attackNumber-1] / 100f) + 1f) * combatStats.stat.attack;
		}
		else
		{
			float ranDamage = Random.Range (minDamage[attackNumber - 1], maxDamage[attackNumber - 1]);
			overallDamage = (ranDamage + 1f) * combatStats.stat.attack;
		}

		float damage = overallDamage;
		
		//Calculate Status Chance
		float statusChance = attackStatusEffectChance[attackNumber-1];

		if(randomElement[attackNumber-1])
		{
			element[attackNumber-1] = Random.Range (1,4);
		}
		
		CombatManager.enemyStats[targetIndex].SetDamage (combatStats.stat, element[attackNumber-1], (int)damage, statusChance,attackCritChance[attackNumber-1]); 
	}
	
	public void SetEnemyDamageDelay(float _time)
	{
		Invoke ("SetEnemyDamage", _time);
	}

	public void Defend()
	{
		//Calculate Resistance
		float defendRating = 2f;
		
		defendRating += combatStats.character.level / 100f; 		
		
		//Activate and send defend message to the PlayerCombatCharacter Script
		combatStats.SetDefend(1, defendRating);
		
		//Move into the Defend Animation
		anim.SetInteger ("Attack Number", 6);
		anim.SetInteger ("Index", 1);
		
		//Set Global Message
		combatStats.ui.SetGlobalMessage ("Guard");
		
		//Stop the camera
		CombatCamera.control.Stop ();
		CombatCamera.control.CameraReset ();
		
		EndTurnDelay (2f);
	}
	
	//This function ends the turn with a delay
	public void EndTurnDelay(float _time)
	{
		Invoke ("EndTurn", _time);
		if(anim.GetInteger("Attack Number") != 6)
		{
			anim.SetInteger ("Attack Number", 0);
		}
	}
	
	
	public void EndTurn()
	{
		if(anim.GetInteger("Attack Number") != 6)
		{
			anim.SetInteger ("Attack Number", 0);
		}
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("NextTurn", SendMessageOptions.DontRequireReceiver);
		this.enabled = false; 
		
		//Unmark as Current
		combatStats.currentTurn = false;
		
		//Destroy Emit Action 
		if(currentEmit)
		{
			Destroy (currentEmit.gameObject);
		}

		combatStats.ui.skipPrompt.SetActive (false);

		//Tutorial Segment of Team mate having low AP
		if(SaveLoadManager.tutorialAPLow == false)
		{
			if(CombatManager.players.Count > 1)
			{
				float percent = (float)combatStats.stat.actionPoints / (float)combatStats.stat.actionPointMax;
				//If there is another player keep an eye on AP
				if(percent < 0.3f)
				{
					//If AP is less than 30% than activate tutorial slide
					combatStats.ui.tutorialSlides[9].gameObject.SetActive (true);
					SaveLoadManager.tutorialAPLow = true;
				}
			}
		}
	}
}
