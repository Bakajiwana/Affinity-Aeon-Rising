using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerCombatCharacter : MonoBehaviour 
{	
	public bool isMain = false;
	[HideInInspector]
	public int iniIndex = 0;
	public Character character; 
	public CombatStat stat;
	[HideInInspector]
	public PlayerElementalReaction elementalReaction;

	public Animator anim;

	//Codex Information
	public Transform characterPortrait;
	public Transform characterAvatar;
	public Transform characterStatusAvatar;
	public bool[] statusImmunities;
	public Transform characterNotes;

	public int healthStrength; //Multiplyer that will be specific to different classes or uniqueness of stat
	public int actionPointStrength;
	public int attackStrength;
	public int defenceStrength;
	public int agilityStrength;
	public int luckStrength;
	public int accuracyStrength;
	public int speedStrength;

	private int shieldWeakness;
	private int shieldResistance;

	//Text Canvas for Information Output
	public GameObject textCanvas; 
	public GameObject damageText;

	//If Hit by a projectile
	private bool hitByProjectile = false;
	private GameObject incomingProjectile;

	//Critical Integrity
	public float criticalChanceIntegrity = 1f;
	public float criticalHitIntegrity = 2f;
	
	public Transform[] statusParticles;

	//Percentage Buff Stats
	[Range(0f, 1f)]
	public float statPercentage = 0.5f;

	//Debuff Particles	
	private Transform condemnedParticles;
	private Transform burningParticles;
	private Transform stunnedParticles;
	private Transform rustedParticles;

	//Buff Particles
	private Transform auraParticles;
	private Transform blazingSpiritParticles;
	private Transform overchargedParticles;
	private Transform purifyParticles; 

	//Defend Mode
	[HideInInspector]
	public bool defend;
	private float defendResistance;
	private int defendElement;

	//Overwatch 
	private Transform overwatchTarget;
	private float overwatchTeleportTimer;
	private float overwatchTeleportMaxTimer = 0.5f;
	private bool overwatchCall = false;

	//Overwatch Protected
	[HideInInspector]
	public bool overwatchProtected = false;
	[HideInInspector]
	public int overwatchIndex = -1;
	[HideInInspector]
	public int overwatchElement;
	private bool overwatchReturn = false;

	//Cancelling Overwatch protected
	private bool cancellingOverwatch = false;

	//AP Costs
	public int[] elementalAPCost = new int[4];

	//Access UI
	[HideInInspector]
	public CombatUIManager ui;
	[HideInInspector]
	public PartyMemberStatus statusUI;

	public bool instantDeath;
	public Transform deathEffect;

	//Current Turn
	[HideInInspector]
	public bool currentTurn = false;	

	// Use this for initialization
	void Start () 
	{
		//Intro Animate
		if(GameObject.FindGameObjectWithTag ("Combat Manager"))
		{
			Invoke ("IntroAnimate", GameObject.FindGameObjectWithTag ("Combat Manager").GetComponent<CombatManager>().battleStartTime - 1.5f);
		}

		//Initialise AP Cost
		for(int i = 0; i < elementalAPCost.Length; i++)
		{
			elementalAPCost[i] = 5;
		}

		overwatchIndex = -1; //Make sure this doesn't start with 0;

		//Initialise Combat UI Manager
		ui = GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<CombatUIManager>();

		if(stat.shieldMax > 0) //If this has a shield
		{
			//Calculate its weakness and strength
			switch(stat.shieldAffinity)
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
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If recieved message to protect an ally
		if(overwatchCall)
		{
			if(overwatchTeleportTimer > 0f)
			{
				overwatchTeleportTimer -= Time.deltaTime;

				//Teleport to ally
				float step = 80f * Time.deltaTime;
				anim.transform.position = Vector3.MoveTowards (anim.transform.position, 
				                                          overwatchTarget.position, step);
			}
			else
			{
				//Check for enemies nearby
				if(CheckForNearbyEnemy() != null)
				{
					//If there is an enemy nearby go into a counter attack
					anim.SetTrigger ("Counter");

					//Set Global Message
					ui.SetGlobalMessage ("Blocked");

					//print ("Counter");

					//Counterattack damage
					CounterAttack (overwatchElement);
				}
				else
				{
					//print ("block");

					//If there is no enemy nearby block animation
					anim.SetTrigger ("Block");

					//Set Global Message
					ui.SetGlobalMessage ("Blocked");
				}
				//print ("Check");
				overwatchCall = false;
			}
		}

		if(overwatchReturn)
		{
			if(overwatchTeleportTimer > 0f)
			{
				overwatchTeleportTimer -= Time.deltaTime;

				//Teleport to initial position
				float step = 80f * Time.deltaTime;
				anim.gameObject.transform.position = Vector3.MoveTowards (anim.gameObject.transform.position,
				                                                          transform.position, step);
			}
			else
			{
				overwatchReturn = false;
			}
		}

		if(Application.isEditor)
		{
			if(Input.GetKeyDown (KeyCode.PageUp))
			{
				if(isMain)
				{				
					Invoke ("PlayerDown", Random.Range (0f,1f));
				}
			}
			if(Input.GetKeyDown (KeyCode.End))
			{
				if(!isMain)
				{				
					Invoke ("PlayerDown", Random.Range (0f,1f));
				}
			}
		}

		if(stat.health > 0)
		{
			anim.SetBool ("Downed", false);
		}
		else
		{
			anim.SetBool ("Downed", true);
		}
	}
	
	//Obtain stat information from spawn, this is done by recieving level information from the spawner
	public void InitiatePlayerStats(Character _character)
	{
		//This character now defined with its character stats
		character = _character;

		//Construct stats
		stat = new CombatStat(character.level, character.currentShieldAffinity,
		                      character.currentShield, healthStrength, actionPointStrength, 
		                      attackStrength, defenceStrength, agilityStrength,
		                      luckStrength, accuracyStrength, speedStrength);

		//Make sure player character health is at current health 
		stat.health = character.currentHealth;

		if(stat.health > stat.healthMax)
		{
			stat.health = stat.healthMax;
		}

		//Make sure player AP is at current AP
		stat.actionPoints = character.currentAP;

		if(stat.actionPoints > stat.actionPointMax)
		{
			stat.actionPoints = stat.actionPointMax;
		}

		//Obtain the Elemental Reaction Component
		elementalReaction = gameObject.GetComponent<PlayerElementalReaction>();

		//iniIndex = PlayerIndex ();
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
	}

	public void SetDamage(CombatStat _opposingStat, int _damageType, int _damage, 
	                      float _statusEffectChance, float _criticalChance)
	{
		int element = _damageType;
		Color textColour = Color.white;
		float textSize = 1f;

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

			if(!overwatchCall)
			{
				//Play Dodge Animation
				anim.SetTrigger ("Dodge");
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
			
		//If Not overwatch protected
		if(!overwatchProtected)
		{
			if(dodged)
			{
				if(!overwatchCall)
				{
					//Dodge attack
					ShowDamageText ("Miss", textColour, textSize);
				}
				else
				{
					ShowDamageText ("0", textColour, textSize);
				}
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
					ShowDamageText ("Critical", textColour, textSize);
					//print ("CRITICAL HIT");
					//GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("CriticalCameraOn", gameObject, SendMessageOptions.DontRequireReceiver);
				}
				
				//No Debuff can be afflicted when shield is up
				if(stat.shield > 0) //if shield is still active
				{
					//Defence
					_damage -= (int)((float)stat.defence/2f); 
					if(_damage <= 0)
					{
						_damage = 1;	//Make sure damage doesn't fall into a negative
					}

					//If Defending
					if(defend)
					{
						//Deactivate Icon
						statusUI.protectedPanel.gameObject.SetActive (false);
						
						if(element == defendElement)
						{
							_damage /= (int)(defendResistance * 4f); 
						}
						else
						{
							_damage /= (int)(defendResistance * 2f); 
						}
						
						//Check for enemies nearby
						if(CheckForNearbyEnemy() != null)
						{
							//If there is an enemy nearby go into a counter attack
							anim.SetTrigger ("Counter");
							
							//Counterattack damage
							CounterAttack (defendElement);
						}
						else
						{
							//If there is no enemy nearby block animation
							anim.SetTrigger ("Block");
						}
					}
					else
					{
						//Set Shielded Stagger
						anim.SetTrigger ("Stagger");
					}

					if(shieldWeakness == element)	//If weakness
					{
						_damage *= 2; //Double damage
						stat.shield -= _damage;	
						textSize += 0.25f;
					}
					else if(shieldResistance == element)	//if resistance
					{
						_damage /= 2;
						stat.shield -= _damage;	//Half damage
						textSize -= 0.25f;
					}
					else if (stat.shieldAffinity == element)	//Shield is the same as damage, convert to health
					{
						_damage /= 4;
						stat.shield -= _damage;
						textSize -= 0.25f;
					}
					else
					{
						//Deduct the amount of damage to shield
						stat.shield -= _damage;
						textSize -= 0.25f;
					}

					if(stat.shield > stat.shieldMax)
					{
						stat.shield = stat.shieldMax;
					}

					if(stat.shield < 0)
					{
						stat.shield = 0;
					}

					//Show text damage
					ShowDamageText (_damage.ToString (), textColour, textSize);

				}
				else //Damage will go to health
				{
					//Status Effect Formula: Attack StatusChance minus by enemy weak-strength.
					float statusEffectChance = 0f;
					int statusEffectVerdict = Random.Range (0, 100);
					int maxStatusDuration = 3;
					//Status effect chance should only have a limit
					if(_statusEffectChance > 2f)
					{
						_statusEffectChance = 2f;
					}
					
					//Defence is a resistance factor
					_damage -= (int)((float)stat.defence/2f); 
					if(_damage <= 0)
					{
						_damage = 1;	//Make sure damage doesn't fall into a negative
					}

					//If Defending
					if(defend)
					{
						//Deactivate Icon
						statusUI.protectedPanel.gameObject.SetActive (false);
						
						if(element == defendElement)
						{
							_damage /= (int)(defendResistance * 4f); 
						}
						else
						{
							_damage /= (int)(defendResistance * 2f); 
						}
						
						//Check for enemies nearby
						if(CheckForNearbyEnemy() != null)
						{
							//If there is an enemy nearby go into a counter attack
							anim.SetTrigger ("Counter");
							
							//Counterattack damage
							CounterAttack (defendElement);
						}
						else
						{
							//If there is no enemy nearby block animation
							anim.SetTrigger ("Block");
						}
					}
					else
					{
						//Set Stagger
						anim.SetTrigger ("Stagger");
					}

					
					//Calculate damage
					stat.health -= _damage;


					//Show Damage Text
					ShowDamageText (_damage.ToString (), textColour, 0.75f);

					//Calcualte Status Effect Chance
					statusEffectChance = _statusEffectChance;	
					
					//Calculate whether a debuff has been afflicted and if not defending
					if(statusEffectVerdict <= (int)(statusEffectChance * 100f) && !defend)
					{
						maxStatusDuration = Random.Range (1, maxStatusDuration);
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
					if(stat.health > stat.healthMax)
					{
						stat.health = stat.healthMax;
					}

					//If Health is lower than 30% and not shattered turn Low Health Animation on
					if(stat.health > 0)
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

					if(stat.health <= 0)
					{
						stat.health = 0;

						statusUI.UpdateStatHealth();	//Update health UI

						PlayerDown ();
					}
				}
				print ("I took " + _damage +" damage....Ouch and I now have " + stat.health + " health and " + 
				       stat.shield + " shield");

				//Update Save Health
				GameObject.FindGameObjectWithTag ("Combat Spawner").SendMessage ("SetIndexPlayerInformation", PlayerIndex (),SendMessageOptions.DontRequireReceiver);

				if(stat.health > 0)
				{
					statusUI.UpdateStatHealth();	//Update health UI
					print ("Updating Stat Health UI");
				}
			}
		}
		else
		{
			//Call overwatch to protect
			CombatManager.playerStats[overwatchIndex].TeleportToProtect (gameObject);

			//Overwatch dodge animation
			anim.SetTrigger ("Next");

			//Calculate Damage output here
			float reducedDamage = 2f;

			if(_damageType == overwatchElement)
			{
				reducedDamage = 4f;
			}

			switch (overwatchElement)
			{
			case 0:
				reducedDamage += CombatManager.playerStats[overwatchIndex].character.level / 100f;
				break;
			case 1:	//Earth
				reducedDamage += CombatManager.playerStats[overwatchIndex].character.earthAffinity / 100f;
				break;
			case 2:	//Fire
				reducedDamage += CombatManager.playerStats[overwatchIndex].character.fireAffinity / 100f;
				break;
			case 3:	//Lightning
				reducedDamage += CombatManager.playerStats[overwatchIndex].character.lightningAffinity / 100f;
				break;
			case 4:	//Water
				reducedDamage += CombatManager.playerStats[overwatchIndex].character.waterAffinity / 100f;
				break;
			}

			_damage /= (int)reducedDamage; 

			//Send Damage Output to Overwatch Volunteer
			CombatManager.playerStats[overwatchIndex].SetDamage (_opposingStat, _damageType, _damage, 0f, 0f);

			//Need to make sure index is out of range
			overwatchIndex = -1;

			//No Longer overwatched
			overwatchProtected = false;

			////statusUI.DeactivateDebuff (overwatchElement + 3);
			statusUI.protectedPanel.gameObject.SetActive (false);
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
		ShowDamageText ("Defence Up", Color.white, 1f);
		
		//Instantiate Effect
		Instantiate (statusParticles[4], transform.position, transform.rotation);
	}
	
	public void SetBlazingSpirit(int _statusDuration)
	{
		//Stat Buff - Increase Attack
		stat.attack = (int)((float)stat.attackBase * (1 + statPercentage));

		//Show Text
		ShowDamageText ("Attack Up", Color.white, 1f);
		
		//Instantiate Effect
		Instantiate (statusParticles[5], transform.position, transform.rotation);
	}
	
	public void SetOvercharged(int _statusDuration)
	{
		//Stat Buff - Regain AP
		RegenAP (true, 1f);

		//Show Text
		//ShowDamageText ("Regen AP", Color.white, 1f);
		
		//Instantiate Effect
		Instantiate (statusParticles[6], transform.position, transform.rotation);
	}
	
	public void SetPurify(int _statusDuration)
	{
		//Stat Buff - Regain Health
		SetHeal (true, 0.7f);

		//Show Text
		//ShowDamageText ("Heal", Color.white, 1f);
		
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
			ShowDamageText ("+" + heal.ToString (),new Color(0.5f,1f, 1f, 1f), 1f);
		}
		else
		{
			stat.health += (int)_healAmount;
			ShowDamageText ("+" + _healAmount.ToString (),new Color(0.5f,1f, 1f, 1f), 1f);
		}
		
		if(stat.health > stat.healthMax)
		{
			stat.health = stat.healthMax;
		}

		statusUI.UpdateStatHealth();	//Update health UI
	}

	//This function is called to regenerate AP
	public void RegenAP(bool _percentage, float _regenAmount)
	{
		if(_percentage)
		{
			int regen = (int)((float)stat.actionPointBase * _regenAmount);
			stat.actionPoints += regen;
			ShowDamageText ("+" + regen.ToString (),new Color(1f,0.8f, 0.5f, 1f), 0.75f);
		}
		else
		{
			stat.actionPoints += (int)_regenAmount;
			ShowDamageText ("+" + _regenAmount.ToString (), new Color(1f,0.8f, 0.5f, 1f), 0.75f);
		}

		if(stat.actionPoints > stat.actionPointMax)
		{
			stat.actionPoints = stat.actionPointMax;
		}

		statusUI.UpdateStatActionPoints();	//Update AP UI
		ui.UpdateAPBar ();
		ui.costAP.text = "AP Cost ";
	}

	//This function is called for AP use
	public void APCost (int _cost)
	{
		stat.actionPoints -= _cost;
		statusUI.UpdateStatActionPoints();	//Update AP UI
		ui.UpdateAPBar ();
		ui.costAP.text = "AP Cost: " + _cost.ToString ();
	}

	//This function is called for AP use
	public void APCost (int _cost, int _element)
	{
		stat.actionPoints -= _cost;
		statusUI.UpdateStatActionPoints();	//Update AP UI
		ui.UpdateAPBar (_element);
		ui.costAP.text = "AP Cost: " + _cost.ToString ();
	}

	//This function is used to calculate the index of this player
	public int PlayerIndex()
	{
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			if(CombatManager.players[i] == this.gameObject)
			{
				return i;
			}
		}

		return 0;
	}

	void IntroAnimate()
	{
		anim.SetTrigger ("Intro");
	}

	public void SetPartyUI(PartyMemberStatus _partyMemberStatus)
	{
		statusUI = _partyMemberStatus; 
		statusUI.SetPlayerStat (this.gameObject);

		statusUI.UpdateStatHealth();
		statusUI.UpdateStatActionPoints();
	}

	//This function is called to set up defend mode
	public void SetDefend(int _defendElement, float _defendRate)
	{
		defend = true;
		defendResistance = _defendRate; 
		defendElement = _defendElement;
		//print ("HERE");
	}

	public void HitByProjectile(GameObject _projectile)
	{
		hitByProjectile = true;
		incomingProjectile = _projectile;
	}

	public void TeleportToProtect(GameObject _target)
	{
		overwatchTarget = _target.transform;
		overwatchCall = true;
		overwatchTeleportTimer = overwatchTeleportMaxTimer;
	}

	public void TeleportBackToPosition()
	{
		overwatchReturn = true;
		overwatchCall = false;
		overwatchTeleportTimer = overwatchTeleportMaxTimer;
	}

	public void SetOverwatch(int _index, int _element)
	{
		//statusUI.DeactivateDebuff (overwatchElement + 3);
		statusUI.protectedPanel.gameObject.SetActive (true);
		overwatchIndex = _index;
		overwatchElement = _element;
		overwatchProtected = true;
	}

	GameObject CheckForNearbyEnemy()
	{
		//5 Meters
		for(int i = 0; i < CombatManager.enemies.Count; i ++)
		{
			//Get the distance of the enemy
			float dist = Vector3.Distance (CombatManager.enemies[i].transform.position,  anim.transform.position);

			if(dist < 10f)
			{
				return CombatManager.enemies[i];
			}
		}

		return null;
	}

	void CounterAttack(int _element)
	{
		//Calculate damage
		float damage = (((float)character.earthAffinity / 100f) + 1f) * stat.defence;

		EnemyCombatCharacter enemyStat = CheckForNearbyEnemy().GetComponent<EnemyCombatCharacter>();

		enemyStat.SetDamage (stat, _element, (int)damage, 0f, 0f); 
	}

	//This function is called when cancelling overwatch
	public void CancelOverwatchToggle()
	{
		cancellingOverwatch = !cancellingOverwatch;


		if(cancellingOverwatch)
		{
			overwatchProtected = false;
			//statusUI.DeactivateDebuff (overwatchElement + 3);
			statusUI.protectedPanel.gameObject.SetActive (false);

			//Set Global Message
			ui.SetGlobalMessage ("Cancelling Overwatch");
		}
		else
		{
			overwatchProtected = true;
			//statusUI.ActivateDebuff (overwatchElement + 3);
			statusUI.protectedPanel.gameObject.SetActive (true);

			//Set Global Message
			ui.SetGlobalMessage ("Activating Overwatch");
		}
	}

	//This function is called when character is taken out
	void PlayerDown()
	{
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("RemovePlayer", PlayerIndex(), SendMessageOptions.DontRequireReceiver);

		if(currentTurn)
		{
			//Send Message to Combat Manager to start next turn
			GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("NextTurn", SendMessageOptions.DontRequireReceiver);
		}
		
		if(instantDeath)
		{
			if(deathEffect)
			{
				Instantiate (deathEffect, transform.position, transform.rotation);
			}
			gameObject.SetActive (true); //Make this object inactive temporarily just in case
		}
		else
		{
			//Play Death Animation
			//gameObject.SetActive (false); //Make this object inactive temporarily just in case

			stat.health = 0;
			print ("Play Death Animation");
		}
	}

	//This Function is called to send information to Codex
	void SetCodex()
	{
		//Gather all the buff lengths
		int[] statusBuffLengths = new int[4];

		//Gather all the debuff lengths
		int[] statusDebuffLengths = new int[10];

		//Send the Set Character Codex Function
		ui.SetCharacterCodex (new CodexCharacter (characterPortrait, character.name, character.level,
		                                          stat.shieldAffinity, stat.shield, stat.shieldMax, true,
		                                          0, stat.health, stat.healthMax, true, 
		                                          statusBuffLengths, statusDebuffLengths, statusImmunities,
		                                          stat.attack, stat.defence, stat.agility, stat.luck,
		                                          stat.accuracy, stat.speed, true, characterNotes, false,
		                                          PlayerIndex ()));
	}

	//This Procedure is called when the elemental experience needs to be updated, also checks for level up
	public void UpdateElementExp(int _element)
	{
		switch(_element)
		{
		case 1:	//Earth
			character.earthExperience += CombatManager.experienceElemental;
			//print (character.earthExperience + " / " + character.earthMaxExperience);
			//Check for Level Up
			if(character.earthExperience >= character.earthMaxExperience)
			{
				character.EarthUp ();
				//print ("LEVEL UP EARTH");
				//ui.levelUpUIEffects[0].SetActive (true);
			}			
			break;
		case 2:	//Fire
			character.fireExperience += CombatManager.experienceElemental;
			
			//Check for Level Up
			if(character.fireExperience >= character.fireMaxExperience)
			{
				character.FireUp ();
				//print ("LEVEL UP FIRE");
				//ui.levelUpUIEffects[1].SetActive (true);
			}
			break;
		case 3:	//Lightning
			character.lightningExperience += CombatManager.experienceElemental;
			
			//Check for Level Up
			if(character.lightningExperience >= character.lightningMaxExperience)
			{
				character.LightningUp ();
				//print ("LEVEL UP LIGHTNING");
				//ui.levelUpUIEffects[2].SetActive (true);
			}
			break;
		case 4:	//Water
			character.waterExperience += CombatManager.experienceElemental;
			
			//Check for Level Up
			if(character.waterExperience >= character.waterMaxExperience)
			{
				character.WaterUp ();
				//print ("LEVEL UP WATER");
				//ui.levelUpUIEffects[3].SetActive (true);
			}
			break; 
		}
	}
}
