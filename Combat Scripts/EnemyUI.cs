using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Script Objective: Updates the Enemy UI

public class EnemyUI : MonoBehaviour 
{
	public Transform healthNode;
	public Transform shieldNode;
	
	public Image healthSlider;
	public Image shieldSlider;
	
	//Lerping Variables
	private bool lerpHealth = false;
	private float lerpCurrHealth;
	private float lerpCurrHealthBar;
	
	private bool lerpShield = false;
	private float lerpCurrShield;
	private float lerpCurrShieldBar;
	
	//Current Health, AP and Shield
	private int health; 
	private int maxHealth;
	
	private int shield;
	private int maxShield;
	
	//Public Speeds
	private float gaugeLerpSpeed = 0.1f;
	
	public float dampen = 0.2f;
	
	public EnemyCombatCharacter enemyStat;

	public CanvasGroup enemyUICanvas;
	private float canvasTimer = 0f;

	public Transform[] weaknessIcons; //0 - Earth, 1 - Fire, 2 - Lightning, 3 - Water

	//Affinity Shatter Bar
	public Transform shatterNode;
	public Image shatterBar;
	private int shatterMaxIntegrity;
	
	// Use this for initialization
	void Start () 
	{
		InitialiseStats();
		UpdateStatHealth ();
		canvasTimer = 0f;
		enemyUICanvas.alpha = 0f;
		shatterNode.gameObject.SetActive (true);
		if(enemyStat.affinityRevealed)
		{
			shatterBar.fillAmount = 0f;
		}
		else
		{
			shatterBar.fillAmount = 1f;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(canvasTimer > 0f)
		{
			if(enemyUICanvas.alpha < 1f)
			{
				enemyUICanvas.alpha += Time.deltaTime * 3f;
			}

			canvasTimer -= Time.deltaTime;

			if(canvasTimer <= 0f)
			{
				enemyUICanvas.alpha = 0f;
			}
		}

		if(lerpHealth)
		{
			float healthPercentage = (float)health/ (float)maxHealth;
			
			if(healthSlider.fillAmount == 0f)
			{
				healthSlider.fillAmount = (float)health/ (float)maxHealth;
			}
			
			gaugeLerpSpeed = Mathf.SmoothStep (healthPercentage, lerpCurrHealthBar, Time.time);
			
			
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
		}
		
		if(lerpShield)
		{
			float shieldPercentage = (float)shield/ (float)maxShield;
			
			gaugeLerpSpeed = Mathf.SmoothStep (shieldPercentage, lerpCurrShieldBar, Time.time);
			
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
				
				lerpShield = false;
				lerpHealth = true;
				
				shieldNode.gameObject.SetActive (false);
				healthNode.gameObject.SetActive (true);
				shieldSlider.gameObject.SetActive (false);
			
				healthSlider.fillAmount = (float)health/ (float)maxHealth;
			}			
			shieldSlider.fillAmount = Mathf.SmoothStep (shieldPercentage, lerpCurrShieldBar, Time.time);
		}

		
		if(health <= 0)
		{
			health = 0;
			
			lerpCurrHealth = 0;
			lerpCurrHealthBar = 0f;

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
		}
	}
	
	public void InitialiseStats()
	{		
		//Initiate Lerp from variables
		lerpCurrHealth = enemyStat.stat.health;
		lerpCurrHealthBar = enemyStat.stat.health/ enemyStat.stat.healthMax;
		
		lerpCurrShield = enemyStat.stat.shield;
		lerpCurrShieldBar = 1f;
		
		health = enemyStat.stat.health;
		maxHealth = enemyStat.stat.healthMax;
		
		shield = enemyStat.stat.shield;
		maxShield = enemyStat.stat.shieldMax;
		
		healthSlider.fillAmount = (float)enemyStat.stat.health/ (float)enemyStat.stat.healthMax;
		healthNode.gameObject.SetActive (false);
		
		shieldSlider.fillAmount = (float)enemyStat.stat.shield/ (float)enemyStat.stat.shieldMax;

		shatterMaxIntegrity = enemyStat.healthIntegrity;

		//Change text of weak signs
		for(int i = 0; i < weaknessIcons.Length; i++)
		{
			weaknessIcons[i].gameObject.GetComponentInChildren<Text>().text = "Weakness";
		}
	}
	
	public void UpdateStatHealth()
	{
		canvasTimer = 2f;

		shield = enemyStat.stat.shield;
		health = enemyStat.stat.health;
		
		if(shield > 0)
		{
			lerpShield = true;

			//Reveal Weakness Icons if Affinity is Shattered
			if(enemyStat.shieldRuptured)
			{
				//Clean Up Icons
				for(int i = 0; i < weaknessIcons.Length; i++)
				{
					weaknessIcons[i].gameObject.SetActive (false);
				}

				weaknessIcons[enemyStat.shieldAffinity - 1].gameObject.SetActive (true);
			}
			else
			{
				for(int i = 0; i < weaknessIcons.Length; i++)
				{
					weaknessIcons[i].gameObject.SetActive (false);
				}
			}
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
			
			if(health <= 0f)
			{
				healthSlider.fillAmount = (float)health/ (float)maxHealth;
			}

			//Reveal Weakness Icons if Affinity is Shattered
			if(enemyStat.affinityRevealed)
			{
				//Clean Up Icons
				for(int i = 0; i < weaknessIcons.Length; i++)
				{
					weaknessIcons[i].gameObject.SetActive (false);
				}
				
				weaknessIcons[enemyStat.affinity - 1].gameObject.SetActive (true);

				//Turn off Shatter Node
				shatterNode.gameObject.SetActive (false);
			}
			else
			{
				for(int i = 0; i < weaknessIcons.Length; i++)
				{
					weaknessIcons[i].gameObject.SetActive (false);
				}

				//Update Shatter Bar
				float shatterAmount = ((float)enemyStat.healthIntegrity) / 
										(float)shatterMaxIntegrity;
				shatterBar.fillAmount = shatterAmount;

				if(shatterNode.gameObject.activeInHierarchy && shatterBar.fillAmount == 0f)
				{
					Invoke ("DeactivateShatterBar", 1f);
				}
			}
		}
	}

	void DeactivateShatterBar()
	{
		shatterNode.gameObject.SetActive (false);
	}
}
