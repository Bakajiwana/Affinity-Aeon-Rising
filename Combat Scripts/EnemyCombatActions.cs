using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyCombatActions : MonoBehaviour 
{	
	//Animator 
	public Animator anim;

	[HideInInspector]
	public EnemyCombatCharacter combatStats;

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
	private GameObject target;
	[HideInInspector]
	public int targetIndex;

	//Emit Action
	public Transform emitAction;
	private Transform currentEmit;

	void Awake()
	{
		//Connect to Stats
		combatStats = gameObject.GetComponent<EnemyCombatCharacter>();

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
			print ("I'm the Enemy and its my turn woop WOOP.. now I end my turn prematurely");

			//Calculate target to attack
			targetIndex = Random.Range (0, CombatManager.players.Count);
			target = CombatManager.players[targetIndex]; //Select Random target

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
		trajectory.SetTarget (combatStats.stat, target, (int)damage, true, 
		                      statusChance, attackCritChance[attackNumber-1]);
	}	

	public void SetPlayerDamage()
	{
		float damage;
		if(!randomDamage[attackNumber - 1])
		{
			overallDamage = (((float)attackDamage[attackNumber-1] / 100f) + 1f) * combatStats.stat.attack;
		}
		else
		{
			damage = Random.Range (minDamage[attackNumber - 1], maxDamage[attackNumber - 1]);
			overallDamage = (damage + 1f) * combatStats.stat.attack;
		}

		//Calculate Damage
		damage = overallDamage;

		//Calculate Status Chance
		float statusChance = attackStatusEffectChance[attackNumber-1];

		CombatManager.playerStats[targetIndex].SetDamage (combatStats.stat, 0, (int)damage, statusChance,attackCritChance[attackNumber-1]); 
	}

	public void SetPlayerDamageDelay(float _time)
	{
		Invoke ("SetPlayerDamage", _time);
	}

	//This function ends the turn with a delay
	public void EndTurnDelay(float _time)
	{
		Invoke ("EndTurn", _time);
		anim.SetInteger ("Attack Number", 0);
	}

	
	public void EndTurn()
	{
		anim.SetInteger ("Attack Number", 0);
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
	}
}
