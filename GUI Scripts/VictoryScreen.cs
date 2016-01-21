using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

//Victory Screen Event at the end of battle

public class VictoryScreen : MonoBehaviour 
{
	//Information Variables
	private Character latestStats;
	private Character oldStats;
	private int[] combatIndices;

	//GUI Variables

	//EXP
	public Text oldExp;
	public Text nextExp;
	//Levels
	public Text currentLevel;
	public Text nextLevel;
	//Signs
	public Transform levelUpSign;
	public Transform[] affinityLevelUpSigns;
	//Affinity Levels
	public Text[] affinityLevels;
	//Level Slider
	public Image expSlider; 
	//Continue Button
	public Transform continueButton;

	private bool revealProgression = false;
	private bool revealRush = false;
	public float lerpSpeed = 500f;

	//Updating Variables
	private float levelExp;
	private float currentExp;
	private float expEarned;

	// Use this for initialization
	void Start () 
	{
		//Get the latest stat information from the Combat Information Node which is tagged with "Combat Spawner"
		GameObject.FindGameObjectWithTag ("Combat Spawner").SendMessage ("GetCurrentPlayerInformation", gameObject, SendMessageOptions.DontRequireReceiver);

		//Acquire the Old Stats from the SaveLoadManager
		oldStats = SaveLoadManager.saveStats;

		//Assign all the old Levels to the UI
		//To the EXP Texts
		oldExp.text = oldStats.levelExperience.ToString ();
		nextExp.text = latestStats.levelMaxExperience.ToString ();

		//Update the current levels
		currentLevel.text = oldStats.level.ToString ();
		nextLevel.text = (oldStats.level + 1).ToString ();

		//Hide all Signs
		levelUpSign.gameObject.SetActive (false);
		for(int i = 0; i < affinityLevelUpSigns.Length; i++)
		{
			affinityLevelUpSigns[i].gameObject.SetActive (false);
		}

		//Affinity Levels
		affinityLevels[0].text = oldStats.earthAffinity.ToString ();
		affinityLevels[1].text = oldStats.fireAffinity.ToString ();
		affinityLevels[2].text = oldStats.lightningAffinity.ToString ();
		affinityLevels[3].text = oldStats.waterAffinity.ToString ();

		//Assign Affinity Slider
		expSlider.fillAmount = oldStats.levelExperience / oldStats.levelMaxExperience;

		//Obtain Exp Earned from Combat Manager
		expEarned = (float)CombatManager.experienceEarned;

		continueButton.gameObject.SetActive (false);

		//Start showing progression
		revealProgression = true;
		
		//Update the affinity levels
		if(latestStats.earthAffinity > oldStats.earthAffinity)
		{
			affinityLevelUpSigns[0].gameObject.SetActive (true);
		}
		if(latestStats.fireAffinity > oldStats.fireAffinity)
		{
			affinityLevelUpSigns[1].gameObject.SetActive (true);
		}
		if(latestStats.lightningAffinity > oldStats.lightningAffinity)
		{
			affinityLevelUpSigns[2].gameObject.SetActive (true);
		}
		if(latestStats.waterAffinity > oldStats.waterAffinity)
		{
			affinityLevelUpSigns[3].gameObject.SetActive (true);
		}
		
		affinityLevels[0].text = latestStats.earthAffinity.ToString ();
		affinityLevels[1].text = latestStats.fireAffinity.ToString ();
		affinityLevels[2].text = latestStats.lightningAffinity.ToString ();
		affinityLevels[3].text = latestStats.waterAffinity.ToString ();
		
		currentExp = (float)latestStats.levelExperience;

		print (expEarned);
	}
	
	// Update is called once per frame
	void Update () 
	{
//		//If the Left Mouse Button is clicked then reveal progression, like bars increasing etc
//		if(Input.GetMouseButtonDown (0) || Input.anyKeyDown)
//		{
//			//Start showing progression
//			revealProgression = true;
//
//			//Update the affinity levels
//			if(latestStats.earthAffinity > oldStats.earthAffinity)
//			{
//				affinityLevelUpSigns[0].gameObject.SetActive (true);
//			}
//			if(latestStats.fireAffinity > oldStats.fireAffinity)
//			{
//				affinityLevelUpSigns[1].gameObject.SetActive (true);
//			}
//			if(latestStats.lightningAffinity > oldStats.lightningAffinity)
//			{
//				affinityLevelUpSigns[2].gameObject.SetActive (true);
//			}
//			if(latestStats.waterAffinity > oldStats.waterAffinity)
//			{
//				affinityLevelUpSigns[3].gameObject.SetActive (true);
//			}
//
//			affinityLevels[0].text = latestStats.earthAffinity.ToString ();
//			affinityLevels[1].text = latestStats.fireAffinity.ToString ();
//			affinityLevels[2].text = latestStats.lightningAffinity.ToString ();
//			affinityLevels[3].text = latestStats.waterAffinity.ToString ();
//
//			currentExp = (float)latestStats.levelExperience;
//		}

		//When True start revealing everything
		if(revealProgression)
		{
			//Simulate Level Bar
			if(expEarned > 0)
			{
				expEarned -= (Time.deltaTime * lerpSpeed);
				currentExp += (Time.deltaTime * lerpSpeed);
				expSlider.fillAmount = currentExp / (float)latestStats.levelMaxExperience;

				//If Level Up
				if(latestStats.levelExperience >= latestStats.levelMaxExperience)
				{
					latestStats.LevelUp ();
					levelUpSign.gameObject.SetActive (true);
					Text sign;
					switch(latestStats.level)
					{
					case 2:
						sign = levelUpSign.GetComponentInChildren<Text>();
						sign.text = "Support Unlocked";
						break;
					case 3:
						sign = levelUpSign.GetComponentInChildren<Text>();
						sign.text = "Defend Unlocked";
						break;
					case 6:
						sign = levelUpSign.GetComponentInChildren<Text>();
						sign.text = "Curse Unlocked";
						break;
					}
					currentExp = 0;
				}

				oldExp.text = latestStats.levelExperience.ToString ();
				nextExp.text = latestStats.levelMaxExperience.ToString ();

				currentLevel.text = latestStats.level.ToString ();
				nextLevel.text = (latestStats.level + 1).ToString ();

				latestStats.levelExperience = (long)currentExp;

				if(revealRush)
				{
					//When LMB clicked again rush the reveal
					if(Input.GetMouseButtonDown(0) || Input.anyKeyDown)
					{
						lerpSpeed = 2000f;
					}
				}

				if(Input.GetMouseButtonDown(0) || Input.anyKeyDown)
				{
					revealRush = true;
				}
			}
			else
			{
				//Reveal Continue Button
				continueButton.gameObject.SetActive (true);
			}
		}
	}

	//Obtain the Latest Stats from the Combat Information Node
	void GetLatestStats(Character _latestStats)
	{
		latestStats = _latestStats;
		//print ("Acquired");
	}

	public void ContinueButton()
	{
		//To Continue we want to update the SaveLoadManager with the combat information node
		GameObject.FindGameObjectWithTag ("Combat Spawner").SendMessage ("SetLatestStats", SendMessageOptions.DontRequireReceiver);

		//Now we want to return to the overworld with the combat manager
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("ReturnToOverworld", SendMessageOptions.DontRequireReceiver);
	}
}
