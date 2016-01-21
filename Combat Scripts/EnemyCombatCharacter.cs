using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyCombatCharacter : MonoBehaviour 
{
	//Enemy Stats
	public CombatStat stat;
	[HideInInspector]
	public EnemyElementalReaction elementalReaction;
	[HideInInspector]
	public int iniIndex = 0;

	public Animator anim;

	public bool randomShieldAffinity = true;
	public int forcedShieldAffinity = 1;
	public bool randomHealthAffinity = true;
	public int forcedHealthAffinity = 1;

	//Codex Information
	public string characterName;
	public Transform characterPortrait;
	public Transform characterAvatar;
	public bool[] statusImmunities;
	public bool notesRevealed = false; 
	public Transform characterNotes;
	private int characterLevel;

	[HideInInspector]
	private bool shieldReveal = false;
	private bool healthReveal = false; 

	[HideInInspector]
	public int shieldAffinity;
	public int shield;
	private int shieldMax;
	[Range(1,1000)]
	public int healthStrength = 300; //Multiplyer that will be specific to different classes or uniqueness of stat
	[Range(1,100)]
	public int actionPointStrength = 10;
	[Range(1,100)]
	public int attackStrength = 20;
	[Range(1,100)]
	public int defenceStrength = 20;
	[Range(1,100)]
	public int agilityStrength = 10;
	[Range(1,100)]
	public int luckStrength = 10;
	[Range(1,100)]
	public int accuracyStrength = 40;
	[Range(1,100)]
	public int speedStrength = 30;

	[HideInInspector]
	public bool defending;

	[HideInInspector] //Had to make this public so Action script can use this.
	public int affinity; //0-none, 1 - Earth, 2 - Fire, 3 - Lightning, 4 - Water
	private int affinityWeakness;
	private int affinityResistance; 

	private int shieldWeakness; 
	private int shieldResistance;

	public GameObject textCanvas;
	public GameObject damageText;

	//If Hit by a projectile
	private bool hitByProjectile = false;
	private GameObject incomingProjectile;

	//Critical Integrity
	public float criticalChanceIntegrity = 1f;
	public float criticalHitIntegrity = 2f;

	[Range(0f,1f)]
	public float statPercentage = 0.5f;

	public int statusWeaknessDuration = 4;
	public int statusStandardDuration = 3;
	public int statusResistedDuration = 2;
	public int statusStrengthDuration = 1;

	public Transform[] statusParticles;

	private Transform condemnedParticles;
	private Transform burningParticles;
	private Transform stunnedParticles;
	private Transform rustedParticles;

	private Transform auraParticles;
	private Transform blazingSpiritParticles;
	private Transform overchargedParticles;
	private Transform purifyParticles; 

	//Integrity = The amount of which the weakness is revealed
	//CHANGE: The Integrities will now be revealed by the number of Reactions.
	public int healthIntegrity = 3; 
	public bool affinityRevealed = false;

	public int shieldIntegrity = 3;
	public bool shieldRuptured = false;

	[Range(0f,1f)]
	public float sealedDamageReduction = 0.5f;

	//If affinity is revealed then reveal their affinity material
	public Material[] shieldAffinityMaterials;
	public GameObject[] shieldAffinitySwitchObjects;
	public Transform[] shieldHiddenReveals; 
	public Transform[] shieldHideObjects;

	public Material[] healthAffinityMaterials;
	public GameObject[] healthAffinitySwitchObjects;

	public Transform[] healthHiddenReveals; 
	public Transform[] healthHideObjects;

	public Transform[] earthAffinityReveal;
	public Transform[] fireAffinityReveal;
	public Transform[] lightningAffinityReveal;
	public Transform[] waterAffinityReveal;

	public bool instantDeath;
	public Transform deathEffect;
	public long expReward;

	//Access UI
	[HideInInspector]
	public CombatUIManager ui;
	private EnemyUI enemyUI;

	//Current Turn
	[HideInInspector]
	public bool currentTurn = false;

	// Use this for initialization
	void Start () 
	{
		//Initialise Combat UI Manager
		ui = GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<CombatUIManager>();
		enemyUI = gameObject.GetComponent<EnemyUI>();

		//Enemies begin with a Random Affinity
		if(randomHealthAffinity)
		{
			affinity = Random.Range (1, 5);

			//Inserting this code because it never ever hits 4 and 5 is not desired
			if(affinity == 5)
			{
				affinity --;
			}
		}
		else
		{
			affinity = forcedHealthAffinity;
		}

		if(randomShieldAffinity)
		{
			shieldAffinity = Random.Range (1, 5);

			//Inserting this code because it never ever hits 4 and 5 is not desired
			if(shieldAffinity == 5)
			{
				shieldAffinity --;
			}
		}
		else
		{
			shieldAffinity = forcedShieldAffinity;
		}

		//Initiate shield 
		shieldMax = shield;

		//Describe the weakness and strength of this enemy's affinity
		switch (affinity)
		{
		case 1: //The affinity is Earth
			affinityWeakness = 2; //Weakness is Fire
			affinityResistance = 3; //Resistant to Lightning
			break;
		case 2: //The affinity is Fire
			affinityWeakness = 4; //Weakness is water
			affinityResistance = 1; //Resistant to Earth
			break;
		case 3: //The affinity is lightning
			affinityWeakness = 1; //Weakness is Earth
			affinityResistance = 4; //Resistant to water
			break;
		case 4:	//The affinity is Water
			affinityWeakness = 3; //Weakness is Lightning
			affinityResistance = 2; //Resistant to Fire
			break;
		}

		if(shield > 0) //If this enemy has a shield
		{
			//Calculate its weakness and strength
			switch(shieldAffinity)
			{
			case 1: //The affinity is Earth
				shieldWeakness = 2; //Weakness is Fire
				shieldResistance = 3; //Resistant to Lightning
				break;
			case 2: //The affinity is Fire
				shieldWeakness = 4; //Weakness is water
				shieldResistance = 1; //Resistant to Earth
				break;
			case 3: //The affinity is lightning
				shieldWeakness = 1; //Weakness is Earth
				shieldResistance = 4; //Resistant to water
				break;
			case 4:	//The affinity is Water
				shieldWeakness = 3; //Weakness is Lightning
				shieldResistance = 2; //Resistant to Fire
				break;
			}
		}

		for(int i = 0; i < healthAffinitySwitchObjects.Length; i++)
		{
			healthAffinitySwitchObjects[i].GetComponent<Renderer>().material = healthAffinityMaterials[0];
		}

		for(int i = 0; i < shieldAffinitySwitchObjects.Length; i++)
		{
			shieldAffinitySwitchObjects[i].GetComponent<Renderer>().material = shieldAffinityMaterials[0];
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Check if Shattered
		if(!affinityRevealed)
		{
			//healthIntegrity -= _damage;
			if(healthIntegrity <= 0)
			{
				RevealHealthAffinity ();
				affinityRevealed = true;
				healthReveal = true;
				GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("CriticalCameraOn", gameObject,SendMessageOptions.DontRequireReceiver);
			}
		}

		if(Application.isEditor)
		{
			if(Input.GetKeyDown (KeyCode.PageDown))
			{
				Invoke ("EnemyDown", Random.Range(0f, 3f));
			}
		}
	}
	
	//Obtain stat information from spawn, this is done by recieving level information from the spawner
	public void InitiateEnemyStats(int _level)
	{		
		//Construct stats
		stat = new CombatStat(_level, shieldAffinity, shield, healthStrength, actionPointStrength, 
		                      attackStrength, defenceStrength, agilityStrength,
		                      luckStrength, accuracyStrength, speedStrength);

		characterLevel = _level;

		//Obtain the Elemental Reaction Component
		elementalReaction = gameObject.GetComponent<EnemyElementalReaction>();
		
		iniIndex = EnemyIndex ();		
	}

	//This function is to show damage text and take a string, colour and whether or not it was a critical
	public void ShowDamageText(string _text, Color _textColor, float _textSizeMultiply)
	{
		GameObject textDamage = Instantiate (damageText, textCanvas.transform.position, transform.rotation) as GameObject;
		textDamage.transform.SetParent (textCanvas.transform, false);

		Text textObject = textDamage.GetComponentInChildren<Text>();

		textObject.text = _text;
		textObject.color = _textColor;

		Vector3 textSizeMultiply = new Vector3(_textSizeMultiply,_textSizeMultiply,_textSizeMultiply);
		textObject.gameObject.transform.localScale = Vector3.Scale (textObject.gameObject.transform.localScale, 
																	textSizeMultiply);

		enemyUI.UpdateStatHealth();
	}

	public void SetDamage(CombatStat _opposingStat, int _damageType, int _damage, 
	                      float _statusEffectChance, float _criticalChance)
	{
		int element = _damageType;
		Color textColour = Color.white;
		float textSize = 1f;

		/*
		switch(_damageType)
		{
		case 1:
			textColour = Color.green;
			break;
		case 2:
			textColour = Color.red;
			break;
		case 3:
			textColour = Color.yellow;
			break;
		case 4:
			textColour = Color.blue;
			break;
		}
		*/

		// Calculate if dodged attack
		bool dodged = false;
		float evasionChance = 0f;

		//print ("Opposing accuracy = " + _opposingStat.accuracy);
		//print ("Agility = " + stat.agility);
		evasionChance = ((float)stat.agility / (float)_opposingStat.accuracy) * 100f;
		int randomEvasionChance = Random.Range (0, 100);
		//print ("Random Evasion Chance = " + randomEvasionChance);
		evasionChance = Random.Range (0,(int)evasionChance);
		//print ("Evasion Chance = " + evasionChance);
		if(randomEvasionChance < evasionChance)
		{
			dodged = true;

			//If Projectile 
			if(hitByProjectile && incomingProjectile)
			{
				incomingProjectile.SendMessage ("TargetMiss",SendMessageOptions.DontRequireReceiver);
				hitByProjectile = false;
			}
		}
		else
		{
			dodged = false;

			//If Projectile 
			if(hitByProjectile && incomingProjectile)
			{
				incomingProjectile.SendMessage ("TargetHit",SendMessageOptions.DontRequireReceiver);
				hitByProjectile = false;
			}

			//Send Message to Elemental Reaction Script to Activate Elemental Reaction
			if(element > 0)
			{
				elementalReaction.ActivateElementalEffect (element);
			}
		}

		if(dodged)
		{
			//Dodge attack
			ShowDamageText ("Miss", textColour, textSize);
		}
		else //Did not dodge attack, then calculate damages
		{
			int criticalHitChance = (int)((_criticalChance * (float)_opposingStat.luck) * criticalChanceIntegrity);
			//print (criticalHitChance);
			//Damage Calculations - If its a critical hit
			if(Random.Range (0,100) <= criticalHitChance)
			{
				//Then critical hit
				_damage = (int)((float)_damage * (float)criticalHitIntegrity);
				textSize = 1.5f;
				//GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("CriticalCameraOn", gameObject,SendMessageOptions.DontRequireReceiver);
				ShowDamageText ("Critical", textColour, textSize);
				//print ("CRITICAL HIT");
			}


			//No Debuff can be afflicted when shield is up
			if(shield > 0) //if shield is still active
			{

				if(defending)
				{
					_damage /= 2;
				}

				if(shieldWeakness == element)	//If weakness
				{
					if(shieldRuptured)
					{
						_damage *= 4; //quadruple damage
						textSize += 0.5f;
					}
					else
					{
						_damage *= 2; //Double damage
					}
					shield -= _damage;	
					if(_statusEffectChance < 0.4f)
					{
						ShowDamageText (_damage.ToString (), textColour, textSize + 0.25f);
					}
				}
				else if(shieldResistance == element)	//if resistance
				{
					_damage /= 2;
					shield -= _damage;	//Half damage
					if(_statusEffectChance < 0.4f)
					{
						ShowDamageText (_damage.ToString (), textColour, textSize - 0.25f);
					}
				}
				else if (shieldAffinity == element)	//Shield is the same as damage, convert to health
				{
					_damage = -_damage;
					shield -= _damage;
					if(_statusEffectChance < 0.4f)
					{
						ShowDamageText ((-_damage).ToString (), textColour, textSize - 0.25f);
					}
				}
				else
				{
					//Deduct the amount of damage to shield
					shield -= _damage;
					if(_statusEffectChance < 0.4f)
					{
						ShowDamageText (_damage.ToString (), textColour, textSize - 0.25f);
					}
				}

				if(shield < 0)
				{
					shield = 0;
					//Hide shield objects
					for (int i = 0; i < shieldHiddenReveals.Length; i++)
					{
						shieldHiddenReveals[i].gameObject.SetActive (false);
					}
				}
				else
				{
					if(!shieldRuptured)
					{
						//shieldIntegrity -= _damage;
						if(shieldIntegrity <= 0)
						{
							shieldRuptured = true;
							shieldReveal = true;
							RevealShieldAffinity ();
							GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("CriticalCameraOn", gameObject,SendMessageOptions.DontRequireReceiver);
						}
					}
				}
				if(shield > shieldMax)
				{
					shield = shieldMax;
				}

				//Set Stagger
				anim.SetTrigger ("Stagger");
			}
			else //Damage will go to health
			{
				//Status Effect Formula: Attack StatusChance minus by enemy weak-strength.
				float statusEffectChance = 0f;
				int statusEffectVerdict = Random.Range (0, 100);
				int maxStatusDuration = 1;
				//Status effect chance should only have a limit
				if(_statusEffectChance > 2f)
				{
					_statusEffectChance = 2f;
				}

				//Defence is a resistance factor when shield is down

				if(defending)
				{
					_damage /= 2;
				}

				_damage -= (int)((float)stat.defence/2f); 
				if(_damage <= 0)
				{
					_damage = 1;	//Make sure damage doesn't fall into a negative
				}

				if(affinityRevealed)
				{
					if(affinityWeakness == element)
					{
						if(affinityRevealed)
						{
							_damage *= 4; //quadruple damage
							textSize += 0.5f;
						}
						else
						{
							_damage *= 2; //Double damage
						}
						stat.health -= _damage;
						if(_statusEffectChance < 0.4f)
						{
							ShowDamageText (_damage.ToString (), textColour,  textSize + 0.25f);
						}
						statusEffectChance = _statusEffectChance * (((float)Random.Range (100,150))/100f);	//Calcualte Status Effect Chance
						maxStatusDuration = statusWeaknessDuration;	//Calculate a random max duration
					}
					else if (affinityResistance == element)
					{
						_damage /= 2;
						stat.health -= _damage;
						if(_statusEffectChance < 0.4f)
						{
							ShowDamageText (_damage.ToString (), textColour,  textSize + 0.25f);
						}
						statusEffectChance = _statusEffectChance * (((float)Random.Range (100,150))/100f);	//Calcualte Status Effect Chance
						maxStatusDuration = statusResistedDuration;	//Calculate a random max duration
					}
					else if (affinity == element)
					{
						stat.health -=  -_damage;
						if(_statusEffectChance < 0.4f)
						{
							ShowDamageText ("+" + (_damage).ToString (), textColour,  textSize + 0.25f);
						}
						statusEffectChance = _statusEffectChance * (((float)Random.Range (100,150))/100f);	//Calcualte Status Effect Chance
						maxStatusDuration = statusStrengthDuration;	//Calculate a random max duration
					}
					else
					{
						stat.health -= _damage;
						if(_statusEffectChance < 0.4f)
						{
							ShowDamageText (_damage.ToString (), textColour,  textSize + 0.25f);
						}
						statusEffectChance = _statusEffectChance * (((float)Random.Range (100,150))/100f);	//Calcualte Status Effect Chance
						maxStatusDuration = statusStandardDuration;	//Calculate a random max duration
					}		
				}
				else
				{
					stat.health -= _damage - (int)((float)_damage * sealedDamageReduction);
					if(_statusEffectChance < 0.4f)
					{
						ShowDamageText (_damage.ToString (), textColour,  textSize + 0.25f);
					}
					statusEffectChance = _statusEffectChance * (((float)Random.Range (100,150))/100f);	//Calcualte Status Effect Chance
					maxStatusDuration = statusStandardDuration;	//Calculate a random max duration
				}

				//Calculate whether a debuff has been afflicted
				if(statusEffectVerdict < (int)(statusEffectChance * 100f))
				{
					//maxStatusDuration = Random.Range (1, maxStatusDuration);
					switch(element)
					{
					case 1:	//Earth Status Effect
						//Condemned: Decrease Defence and Accuracy. Prevents Healing
						//print ("I am condemned");
						SetCondemned (maxStatusDuration);
						break;
					case 2:	//Fire Status Effect
						//Burning: Fire Damage over time. Decrease attack.
						//print ("I am burning");
						SetBurning (maxStatusDuration);
						break;
					case 3:	//Lightning Status Effect
						//Stun: Disables moves
						//print ("I am stunned");
						SetStunned (maxStatusDuration);
						break;
					case 4:	//Water Status Effect
						//Rust: Increase debuff chance. Also decreases speed and agility
						//print ("I am rusted");
						SetRusted (maxStatusDuration);
						break;
					}
				}

				//Health management - if health reaches 0 - Player dies
				if(stat.health <= 0)
				{
					stat.health = 0;
					
					EnemyDown ();
				}
				else
				{
					if(!affinityRevealed)
					{
						//healthIntegrity -= _damage;
						if(healthIntegrity <= 0)
						{
							RevealHealthAffinity ();
							affinityRevealed = true;
							healthReveal = true;
							GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("CriticalCameraOn", gameObject,SendMessageOptions.DontRequireReceiver);
						}
					}
				}

				if(stat.health > stat.healthMax)
				{
					stat.health = stat.healthMax;
				}

				//If Health is lower than 70% and not shattered turn Low Health Animation on
				if(stat.health <= 0)
				{
					if(stat.health <= stat.healthBase * 0.70)
					{
						if(affinityRevealed)
						{
							anim.SetBool ("Low Health", true);
						}
						else
						{
							if(stat.health <= stat.healthBase * 0.30)
							{
								anim.SetBool ("Low Health", true);
							}
							else
							{
								anim.SetBool ("Low Health", false);
							}
						}
					}
					else
					{
						anim.SetBool ("Low Health", false);
					}

					//Set Stagger
					anim.SetTrigger ("Stagger");
				}

				//Show damage screen
				if(_damage > 200 && healthReveal)
				{
					switch(_damageType)
					{
					case 0: //white
						CombatCamera.control.ScreenEffect (1);
						break;
					case 1: //Green
						CombatCamera.control.ScreenEffect (2);
						break;
					case 2: //Red
						CombatCamera.control.ScreenEffect (3);
						break;
					case 3: //Yellow
						CombatCamera.control.ScreenEffect (4);
						break;
					case 4: //Cyan
						CombatCamera.control.ScreenEffect (5);
						break;
					}
				}
			}

			gameObject.SendMessage ("UpdateTutorialUI", SendMessageOptions.DontRequireReceiver);
			//print ("I took " + _damage +" damage....Ouch");
			//print ("Shield = " + shield);
			//print ("Health = " + stat.health);
		}
	}

	//These are status effect setter functions
	public void SetCondemned(int _statusDuration)
	{
		//Stat Debuff - Decrease Defence
		stat.defence = (int)((float)stat.defenceBase * statPercentage);

		//Show Text
		ShowDamageText ("Defence Down", Color.white, 0.8f);

		//Instantiate Effect
		Instantiate (statusParticles[0], transform.position, transform.rotation);
	}

	public void SetBurning(int _statusDuration)
	{
		//Stat Debuff - Decrease Attack
		stat.attack = (int)((float)stat.attackBase * statPercentage);

		//Show Text
		ShowDamageText ("Attack Down", Color.white, 0.8f);
		
		//Instantiate Effect
		Instantiate (statusParticles[1], transform.position, transform.rotation);
	}

	public void SetStunned(int _statusDuration)
	{
		//Stat Debuff - Decrease Speed
		stat.speed = (int)((float)stat.speedBase * statPercentage);

		//Show Text
		ShowDamageText ("Speed Down", Color.white, 0.8f);
		
		//Instantiate Effect
		Instantiate (statusParticles[2], transform.position, transform.rotation);
	}

	public void SetRusted(int _statusDuration)
	{
		//Stat Debuff - Decrease Accuracy and Agility
		stat.accuracy = (int)((float)stat.accuracyBase * statPercentage);
		stat.agility = (int)((float)stat.agilityBase * statPercentage);

		//Show Text
		ShowDamageText ("Accuracy Down", Color.white, 0.8f);
		ShowDamageText ("Agility Down", Color.white, 0.8f);
		
		//Instantiate Effect
		Instantiate (statusParticles[3], transform.position, transform.rotation);
	}

	//BUFF Setters
	public void SetAura(int _statusDuration)
	{
		//Stat Buff - Increase Defence
		stat.defence = (int)((float)stat.defenceBase * (1 + statPercentage));

		//Show Text
		ShowDamageText ("Defence Up", Color.white, 0.8f);

		//Instantiate Effect
		Instantiate (statusParticles[4], transform.position, transform.rotation);
	}
	
	public void SetBlazingSpirit(int _statusDuration)
	{
		//Stat Buff - Increase Attack
		stat.attack = (int)((float)stat.attackBase * (1 + statPercentage));

		//Show Text
		ShowDamageText ("Attack Up", Color.white, 0.8f);
		
		//Instantiate Effect
		Instantiate (statusParticles[5], transform.position, transform.rotation);
	}
	
	public void SetOvercharged(int _statusDuration)
	{
		//Stat Buff - Regain AP
		RegenAP (true, 0.30f);

		//Show Text
		ShowDamageText ("Regen AP", Color.white, 0.8f);
		
		//Instantiate Effect
		Instantiate (statusParticles[6], transform.position, transform.rotation);
	}
	
	public void SetPurify(int _statusDuration)
	{
		//Stat Buff - Regain Health
		SetHeal (true, 0.30f);

		//Show Text
		ShowDamageText ("Heal", Color.white, 0.8f);
		
		//Instantiate Effect
		Instantiate (statusParticles[7], transform.position, transform.rotation);
	}

	//This Function is called to heal
	public void SetHeal(bool _percentage, float _healAmount)
	{
		if(_percentage)
		{
			int heal = (int)((float)stat.healthBase * _healAmount);
			stat.health += heal;
			ShowDamageText ("+" + heal.ToString (),Color.white, 1f);
		}
		else
		{
			stat.health += (int)_healAmount;
			ShowDamageText ("+" + _healAmount.ToString (),Color.white, 1f);
		}

		if(stat.health > stat.healthMax)
		{
			stat.health = stat.healthMax;
		}
	}

	//This function is called to regenerate AP
	public void RegenAP(bool _percentage, float _regenAmount)
	{
		if(_percentage)
		{
			int regen = (int)((float)stat.actionPointBase * _regenAmount);
			stat.actionPoints += regen;
			ShowDamageText ("+" + regen.ToString (),Color.white, 1f);
		}
		else
		{
			stat.actionPoints += (int)_regenAmount;
			ShowDamageText ("+" + _regenAmount.ToString (),Color.white, 1f);
		}
		
		if(stat.actionPoints > stat.actionPointMax)
		{
			stat.actionPoints = stat.actionPointMax;
		}
	}

	//This function is called for AP use
	public void APCost (int _cost)
	{
		stat.actionPoints -= _cost;
	}

	//This function is used to calculate the index of this player
	public int EnemyIndex()
	{
		for(int i = 0; i < CombatManager.enemies.Count; i++)
		{
			if(CombatManager.enemies[i] == this.gameObject)
			{
				return i;
			}
		}

		return 0;
	}

	public void HitByProjectile(GameObject _projectile)
	{
		hitByProjectile = true;
		incomingProjectile = _projectile;
	}

	public void EnemyDown()
	{
		RewardExp ();

		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("RemoveEnemy", EnemyIndex(), SendMessageOptions.DontRequireReceiver);

		if(currentTurn)
		{
			//Send Message to Combat Manager to start next turn
			GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("NextTurn", SendMessageOptions.DontRequireReceiver);
		}

		if(instantDeath)
		{
			if(deathEffect)
			{
				Instantiate (deathEffect, anim.transform.position, anim.transform.rotation);
			}
			gameObject.SetActive (false); //Make this object inactive temporarily just in case
		}
		else
		{
			//Play Death Animation
			gameObject.SetActive (false); //Make this object inactive temporarily just in case
			print ("Play Death Animation");
		}
	}

	void RewardExp()
	{
		//Calculate Exp and add to combat manager exp
		//expReward = expReward * (long)((float)characterLevel / 1.5f);
		print ("Rewarded " + expReward + " exp.");
		CombatManager.experienceEarned += expReward;
	}

	//This function is called when Affinity is revealed
	public void RevealShieldAffinity()
	{
		for(int i = 0; i< shieldAffinitySwitchObjects.Length; i++)
		{
			shieldAffinitySwitchObjects[i].GetComponent<Renderer>().material = shieldAffinityMaterials[shieldAffinity];
		}
		
		//Reveal hidden objects
		for (int i = 0; i < shieldHiddenReveals.Length; i++)
		{
			shieldHiddenReveals[i].gameObject.SetActive (true);
		}
		
		//Hide Objects
		for(int i = 0; i < shieldHideObjects.Length; i++)
		{
			shieldHideObjects[i].gameObject.SetActive(false);
		}
	}

	//This function is called when Affinity is revealed
	public void RevealHealthAffinity()
	{
		for(int i = 0; i< healthAffinitySwitchObjects.Length; i++)
		{
			healthAffinitySwitchObjects[i].GetComponent<Renderer>().material = healthAffinityMaterials[affinity];
		}

		//Reveal hidden objects
		for (int i = 0; i < healthHiddenReveals.Length; i++)
		{
			healthHiddenReveals[i].gameObject.SetActive (true);
		}

		//Hide Objects
		for(int i = 0; i < healthHideObjects.Length; i++)
		{
			healthHideObjects[i].gameObject.SetActive(false);
		}

		//Reveal the Affinity Objects
		switch(affinity)
		{
		case 1:	//Earth
			for(int i = 0; i < earthAffinityReveal.Length; i++)
			{
				earthAffinityReveal[i].gameObject.SetActive (true);
				DestroyScript destroyScript = earthAffinityReveal[i].gameObject.GetComponent<DestroyScript>();
				destroyScript.enabled = true;
				destroyScript.destroyTime = Random.Range (1f,20f);
			}
			break;
		case 2:	//Fire
			for(int i = 0; i < fireAffinityReveal.Length; i++)
			{
				fireAffinityReveal[i].gameObject.SetActive (true);
				DestroyScript destroyScript = fireAffinityReveal[i].gameObject.GetComponent<DestroyScript>();
				destroyScript.enabled = true;
				destroyScript.destroyTime = Random.Range (1f,20f);
			}
			break;
		case 3:	//Lightning
			for(int i = 0; i < lightningAffinityReveal.Length; i++)
			{
				lightningAffinityReveal[i].gameObject.SetActive (true);
				DestroyScript destroyScript = lightningAffinityReveal[i].gameObject.GetComponent<DestroyScript>();
				destroyScript.enabled = true;
				destroyScript.destroyTime = Random.Range (1f,20f);
			}
			break;
		case 4:	//Water
			for(int i = 0; i < waterAffinityReveal.Length; i++)
			{
				waterAffinityReveal[i].gameObject.SetActive (true);
				DestroyScript destroyScript = waterAffinityReveal[i].gameObject.GetComponent<DestroyScript>();
				destroyScript.enabled = true;
				destroyScript.destroyTime = Random.Range (1f,20f);
			}
			break;
		}
	}


	//This Function is called to send information to Codex
	void SetCodex()
	{
		//Send the Set Character Codex Function

		//Gather all the buff lengths
		int[] statusBuffLengths = new int[4];
		
		//Gather all the debuff lengths
		int[] statusDebuffLengths = new int[10];
		
		//Send the Set Character Codex Function

		ui.SetCharacterCodex (new CodexCharacter (characterPortrait, characterName, characterLevel,
		                                          shieldAffinity, shield, shieldMax, shieldReveal,
		                                          affinity, stat.health, stat.healthMax, healthReveal, 
		                                          statusBuffLengths, statusDebuffLengths, statusImmunities,
		                                          stat.attack, stat.defence, stat.agility, stat.luck,
		                                          stat.accuracy, stat.speed, notesRevealed, characterNotes, true,
		                                          EnemyIndex ()));
		                                          
	}


	public void SetScan(int[] _scanValues)
	{
		//Scan Chances of discovering shield and health affinity

		//Scanning Chances for the Shield Affinity
		if(!shieldReveal && shieldAffinity > 0)
		{
			int shieldRandom = Random.Range (0, 100);

			if(shieldRandom <= _scanValues[shieldAffinity - 1])
			{
				shieldReveal = true;
			}
		}

		//Scanning Chances for the Health Affinity
		if(!healthReveal && affinity > 0)
		{
			int healthRandom = Random.Range (0,100);

			if(healthRandom <= _scanValues[affinity - 1])
			{
				healthReveal = true;
			}
		}

		//Notes Revealed
		if(!notesRevealed)
		{
			notesRevealed = true;
		}
	}
}
