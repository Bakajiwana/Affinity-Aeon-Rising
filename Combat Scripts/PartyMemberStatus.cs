using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//This script handles the party member status information

public class PartyMemberStatus : MonoBehaviour 
{
	public Text playerName;	//Name
	public Transform healthNode;
	public Text playerHealth; //Health Text
	public Transform shieldNode;
	public Text playerShield; //Shield Text
	public Text playerActionPoints; //Action Points

	public Image healthSlider;
	public Image shieldSlider;
	public Image apSlider;

	//Lerping Variables
	private bool lerpHealth = false;
	private float lerpCurrHealth;
	private float lerpCurrHealthBar;

	private bool lerpShield = false;
	private float lerpCurrShield;
	private float lerpCurrShieldBar;

	private bool lerpActionPoints = false;
	private float lerpCurrActionPoints;

	//Current Health, AP and Shield
	private int health; 
	private int maxHealth;

	private int shield;
	private int maxShield;

	private int AP;
	private int maxAP;

	//Public Speeds
	private float gaugeLerpSpeed = 0.1f;
	private float textLerpSpeed = 1;

	public float dampen = 0.2f;

	private PlayerCombatCharacter playerStat;

	public GameObject initialAPBar;

	public Transform avatarNode;

	[HideInInspector]
	public bool isCurrent = false;
	public CanvasGroup currentPanel;
	private bool currentPanelFadeIn = false;
	private bool currentPanelFadeOut = false;

	//Remember: 0 - Attack, 1 - Defence, 2- Agility, 3 - Luck, 4 - Accuracy, 5 - Speed
	public GameObject[] positiveStats;
	public GameObject[] negativeStats;

	private bool[] positiveStatActive = new bool[6];
	private bool[] negativeStatActive = new bool[6];

	private int currentPosStat;
	private int currentNegStat;

	public float statUpdateTimer = 3f;

	//Instantiations
	//public Transform[] statusBuffsSpawn;
	//public Transform[] statusDebuffsSpawn;
	//Current
	//private Transform[] statusBuffs = new Transform[8];
	//private Transform[] statusDebuffs = new Transform[10];

	public Transform selectedPanel;

	public Transform protectedPanel;

	// Use this for initialization
	void Start () 
	{
		currentPanel.alpha = 0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(lerpHealth)
		{
			float healthPercentage = (float)health/ (float)maxHealth;

			if(healthSlider.fillAmount == 0f)
			{
				healthSlider.fillAmount = (float)health/ (float)maxHealth;
			}

			gaugeLerpSpeed = Mathf.SmoothStep (healthPercentage, lerpCurrHealthBar, Time.time);
			textLerpSpeed = Mathf.SmoothStep (health, lerpCurrHealth, Time.time);
			
			if(lerpCurrHealth > health)
			{
				lerpCurrHealth -= (textLerpSpeed * Time.deltaTime) * dampen;
			}
			else if(lerpCurrShield < health)
			{
				lerpCurrHealth += (textLerpSpeed * Time.deltaTime) * dampen;
			}
			
			
			if(lerpCurrHealthBar > healthPercentage)
			{
				lerpCurrHealthBar -= gaugeLerpSpeed * Time.deltaTime * dampen;
			}
			else if (lerpCurrHealthBar < healthPercentage)
			{
				lerpCurrHealthBar += gaugeLerpSpeed * Time.deltaTime * dampen;
			}

			if((int)lerpCurrHealth == (int)health)
			{
				lerpHealth = false;
			}

			healthSlider.fillAmount = Mathf.SmoothStep (healthPercentage, lerpCurrHealthBar, Time.time);
			playerHealth.text = health.ToString () + " HP"; 
		}

		if(lerpShield)
		{
			float shieldPercentage = (float)shield/ (float)maxShield;

			gaugeLerpSpeed = Mathf.SmoothStep (shieldPercentage, lerpCurrShieldBar, Time.time);
			textLerpSpeed = Mathf.SmoothStep (shield, lerpCurrShield, Time.time);

			if(lerpCurrShield > shield)
			{
				lerpCurrShield -= (textLerpSpeed * Time.deltaTime * dampen);
			}
			else if(lerpCurrShield < shield)
			{
				lerpCurrShield += (textLerpSpeed * Time.deltaTime * dampen);
			}

			if(lerpCurrShieldBar > shieldPercentage)
			{
				lerpCurrShieldBar -= gaugeLerpSpeed * Time.deltaTime * dampen;
			}
			else if (lerpCurrShieldBar < shieldPercentage)
			{
				lerpCurrShieldBar += gaugeLerpSpeed * Time.deltaTime * dampen;
			}

			if((int)lerpCurrShield == (int)shield)
			{
				lerpShield = false;
			}

			if(shield <= 0)
			{
				lerpCurrShield = 0;
				lerpCurrShieldBar = 0f;
				shield = 0;
				shieldSlider.fillAmount = 0f;
				playerShield.text = "0";

				lerpShield = false;
				lerpHealth = true;

				shieldNode.gameObject.SetActive (false);
				healthNode.gameObject.SetActive (true);
				shieldSlider.gameObject.SetActive (false);

				playerHealth.text = health.ToString ();
				healthSlider.fillAmount = (float)health/ (float)maxHealth;
			}


			shieldSlider.fillAmount = Mathf.SmoothStep (shieldPercentage, lerpCurrShieldBar, Time.time);
			playerShield.text = shield.ToString () + " SH"; 
		}


		if(lerpActionPoints)
		{
			float APSpeed = Mathf.SmoothStep (AP, lerpCurrActionPoints, Time.time);

			if(lerpCurrActionPoints > AP)
			{
				lerpCurrActionPoints -= (APSpeed * Time.deltaTime * dampen);
			}
			else if(lerpCurrActionPoints < AP)
			{
				lerpCurrActionPoints += (APSpeed * Time.deltaTime * dampen);
			}

			if(APSpeed <= 0f)
			{
				lerpCurrActionPoints = AP;
				lerpActionPoints = false;
			}

			if((int)lerpCurrActionPoints == AP)
			{
				lerpCurrActionPoints = AP;
				lerpActionPoints = false;
			}

			if(AP == 0)
			{
				lerpCurrActionPoints = AP;
			}

			playerActionPoints.text = ((int)lerpCurrActionPoints).ToString () + " AP";

			float percentageAP = (float)lerpCurrActionPoints / (float)maxAP;
			apSlider.fillAmount = percentageAP;
		}

		if(health <= 0)
		{
			health = 0;

			lerpCurrHealth = 0;
			lerpCurrHealthBar = 0f;

			playerHealth.text = "0";
			healthSlider.fillAmount = 0f;

			//healthSlider.gameObject.SetActive (false);
		}

		if(health > 0)
		{
			if(healthSlider.fillAmount == 0f)
			{
				healthSlider.fillAmount = (float)health/ (float)maxHealth;
			}
		}

		if(shield <= 0)
		{
			lerpCurrShield = 0;
			lerpCurrShieldBar = 0f;
			shield = 0;
			shieldSlider.fillAmount = 0f;
			playerShield.text = "0";
		}

		//Update Health, AP and Shield 

		//Health 
		//CombatUIManager.currentPlayerHealth = healthSlider.value;

		//AP 
		if(playerStat.isMain)
		{
			float percentageAP = (float)lerpCurrActionPoints / (float)maxAP;
			CombatUIManager.currentPlayerAP = percentageAP;
		}

		//Shield
		//CombatUIManager.currentPlayerShield = shieldSlider.value;

		/*Text - health, shield and ap
		if(shield > 0)
		{
			CombatUIManager.currentPlayerHealthText = playerShield.text;
		}
		else
		{
			CombatUIManager.currentPlayerHealthText = playerHealth.text;
		}
		*/

		//CombatUIManager.currentPlayerAPText = playerActionPoints.text;

		if(currentPanelFadeIn)
		{
			currentPanel.alpha += Time.deltaTime;
			if(currentPanel.alpha >= 1f)
			{
				currentPanelFadeIn = false;
			}
		}

		if(currentPanelFadeOut)
		{
			currentPanel.alpha -= Time.deltaTime;
			if(currentPanel.alpha <= 0f)
			{
				currentPanelFadeOut = false;
			}
		}
	}

	public void InitialiseStats()
	{
		playerName.text = playerStat.character.name;

		//Initiate Lerp from variables
		lerpCurrHealth = playerStat.stat.health;
		//print (playerStat.stat.healthMax + " is the max health");
		lerpCurrHealthBar = playerStat.stat.health/ playerStat.stat.healthMax;
		
		lerpCurrShield = playerStat.stat.shield;
		lerpCurrShieldBar = 1f;
		
		lerpCurrActionPoints = playerStat.stat.actionPoints;

		health = playerStat.stat.health;
		maxHealth = playerStat.stat.healthMax;

		shield = playerStat.stat.shield;
		maxShield = playerStat.stat.shieldMax;
		
		AP = playerStat.stat.actionPoints;
		maxAP = playerStat.stat.actionPointMax;

		healthSlider.fillAmount = (float)playerStat.stat.health/ (float)playerStat.stat.healthMax;
		playerHealth.text = playerStat.stat.health.ToString () + " HP"; 
		healthNode.gameObject.SetActive (false);

		shieldSlider.fillAmount = (float)playerStat.stat.shield/ (float)playerStat.stat.shieldMax;
		playerShield.text = playerStat.stat.shield.ToString () + " SH"; 

		playerActionPoints.text = playerStat.stat.actionPoints.ToString () + " AP";

		//If Main Player then send Initial AP Bar to UI Manager, if not main then turn off
		if(playerStat.isMain)
		{
			GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetInitialAPBar", initialAPBar, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			initialAPBar.gameObject.SetActive (false);
		}

		//Initialise Selected Panel on party member status panel
		if(playerStat.gameObject.GetComponent<CombatSelection>())
		{
			playerStat.gameObject.GetComponent<CombatSelection>().partyUI = this;
		}

		if(avatarNode)
		{
			if(playerStat.characterStatusAvatar)
			{
				Transform spawn = Instantiate (playerStat.characterStatusAvatar.transform, avatarNode.position,
				                               avatarNode.rotation) as Transform;

				spawn.SetParent (avatarNode);
			}
		}

		//Start coroutine
		StartCoroutine (SwitchStatBoxes ());
	}

	public void UpdateStatHealth()
	{
		shield = playerStat.stat.shield;
		health = playerStat.stat.health;

		if(shield > 0)
		{
			lerpShield = true;
		}
		else
		{
			lerpShield = false;
			lerpHealth = true;
			
			lerpCurrShield = 0;
			lerpCurrShieldBar = 0f;
			
			shieldNode.gameObject.SetActive (false);
			healthNode.gameObject.SetActive (true);
			shieldSlider.gameObject.SetActive (false);

			if(healthSlider.fillAmount == 0f)
			{
				healthSlider.fillAmount = (float)health/ (float)maxHealth;
			}
		}

		print (playerName.text + " Shield is at = " + shield+ " Health is at = " + health);
	}

	public void UpdateStatActionPoints()
	{
		AP = playerStat.stat.actionPoints;
		lerpActionPoints = true;
	}

	public void SetPlayerStat(GameObject _player)
	{
		playerStat = _player.GetComponent<PlayerCombatCharacter>();
	}

	public void SetCurrent(bool _currentPlayer)
	{
		isCurrent = _currentPlayer;

		if(_currentPlayer)
		{
			currentPanelFadeIn = true;
		}
		else
		{
			currentPanelFadeOut = true;
		}
	}

	//Couroutine that switches that cycles the stat condition boxes
	//0 - Attack, 1 - Defence, 2- Agility, 3 - Luck, 4 - Accuracy, 5 - Speed
	IEnumerator SwitchStatBoxes()
	{
		while(true) //loop forever
		{
			yield return new WaitForSeconds(statUpdateTimer);

			//Deactivate all
			for(int i = 0; i < positiveStats.Length; i++)
			{
				positiveStats[i].SetActive (false);
				negativeStats[i].SetActive (false);

				positiveStatActive[i] = false;
				negativeStatActive[i] = false;
			}

			//Calculate which stat is positive
			if(playerStat.stat.attack > playerStat.stat.attackBase)
			{
				positiveStatActive[0] = true;
			}
			
			if(playerStat.stat.defence > playerStat.stat.defenceBase)
			{
				positiveStatActive[1] = true;
			}
			
			if(playerStat.stat.agility > playerStat.stat.agilityBase)
			{
				positiveStatActive[2] = true;
			}
			
			if(playerStat.stat.luck > playerStat.stat.luckBase)
			{
				positiveStatActive[3] = true;
			}
			
			if(playerStat.stat.accuracy > playerStat.stat.accuracyBase)
			{
				positiveStatActive[4] = true;
			}
			
			if(playerStat.stat.speed > playerStat.stat.speedBase)
			{
				positiveStatActive[5] = true;
			}
			
			//Calculate which stat is negative
			if(playerStat.stat.attack < playerStat.stat.attackBase)
			{
				negativeStatActive[0] = true;
			}
			
			if(playerStat.stat.defence < playerStat.stat.defenceBase)
			{
				negativeStatActive[1] = true;
			}
			
			if(playerStat.stat.agility < playerStat.stat.agilityBase)
			{
				negativeStatActive[2] = true;
			}
			
			if(playerStat.stat.luck < playerStat.stat.luckBase)
			{
				negativeStatActive[3] = true;
			}
			
			if(playerStat.stat.accuracy < playerStat.stat.accuracyBase)
			{
				negativeStatActive[4] = true;
			}
			
			if(playerStat.stat.speed < playerStat.stat.speedBase)
			{
				negativeStatActive[5] = true;
			}

			//For every skill starting from the current stat loop through until an active bool

			//For Positive Stats
			for(int i = 0; i < positiveStats.Length; i++)
			{
				currentPosStat++;
				if(currentPosStat > 5)
				{
					currentPosStat = 0;
				}

				//Determine if stat is positive
				if(positiveStatActive[currentPosStat] == true)
				{
					positiveStats[currentPosStat].SetActive (true);
					break;
				}
			}

			//For Negative Stats
			for(int i = 0; i < negativeStats.Length; i++)
			{
				currentNegStat++;
				if(currentNegStat > 5)
				{
					currentNegStat = 0;
				}
				
				//Determine if stat is positive
				if(negativeStatActive[currentNegStat] == true)
				{
					negativeStats[currentNegStat].SetActive (true);
					break;
				}
			}
		}
	}



	/*
	public void ActivateBuff(int _index)
	{
		//Make sure its clean
		if(statusBuffs[_index])
		{
			Destroy(statusBuffs[_index].gameObject);
		}
		
		statusBuffs[_index] = Instantiate (statusBuffsSpawn[_index],
		                                   statusPositiveNode.position,
		                                   statusPositiveNode.rotation) as Transform;
		statusBuffs[_index].SetParent (statusPositiveNode, true);
	}
	
	public void ActivateDebuff(int _index)
	{
		//Make sure its clean
		if(statusDebuffs[_index])
		{
			Destroy(statusDebuffs[_index].gameObject);
		}
		
		statusDebuffs[_index] = Instantiate (statusDebuffsSpawn[_index],
		                                     statusNegativeNode.position,
		                                     statusNegativeNode.rotation) as Transform;
		statusDebuffs[_index].SetParent (statusNegativeNode, true);
	}
	
	public void DeactivateBuff(int _index)
	{
		if(statusBuffs[_index])
		{
			Destroy(statusBuffs[_index].gameObject);
		}
	}
	
	public void DeactivateDebuff(int _index)
	{
		if(statusDebuffs[_index])
		{
			Destroy(statusDebuffs[_index].gameObject);
		}
	}
	*/
}
