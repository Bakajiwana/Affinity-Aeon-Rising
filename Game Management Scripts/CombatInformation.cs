using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Script Objective: Obtain Enemy and Player Information and send it to the combat spawner at the start of combat

public class CombatInformation : MonoBehaviour 
{
	private CombatSpawner spawner;

	private bool hasInitialisedCombat = false;
	private bool hasEnemyInfo = false;
	private bool hasPlayerInfo = false;

	private GameObject [] playerCharacters;
	private List<Character> characterStats = new List<Character>();
	
	private GameObject [] enemyCharacters;
	private int [] enemyLevels;

	private List<Character> latestStats = new List<Character>();
	private int[] combatIndices;

	//Overworld Information
	[HideInInspector]
	public string overworldSceneName;

	// Use this for initialization
	void Start () 
	{
		//Obtain Overworld Scene Name
		overworldSceneName = Application.loadedLevelName;

		//Don't Destroy this object on Load
		DontDestroyOnLoad (gameObject);

		//When Spawned, Get Player Information
		GameObject.FindGameObjectWithTag ("Adventure Manager").SendMessage ("GetPlayerInformation", gameObject, SendMessageOptions.DontRequireReceiver);
	}

	//When Level is loaded
	void OnLevelWasLoaded(int level)
	{
		//Don't Destroy On Load, The Game Manager will destroy this object when the Overworld loads
		DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!hasInitialisedCombat)
		{
			if(hasEnemyInfo && hasPlayerInfo)
			{
				hasInitialisedCombat = true;

				//Activate Combat (this script was named for this moment)
				GameObject.FindGameObjectWithTag ("Game Manager").SendMessage ("CombatSceneActivate", SendMessageOptions.DontRequireReceiver);

			}
		}
	}

	//This function is called to obtain enemy information
	public void SetEnemyInformation(GameObject[] _enemyCharacters, int[] _enemyLevels)
	{
		enemyCharacters = _enemyCharacters;
		enemyLevels = _enemyLevels;

		//print (enemyCharacters.Length);

		//Mark as info obtained
		hasEnemyInfo = true;
	}

	public void SetPlayerInformation (GameObject[] _playerCharacters, int[] _indexHealth)
	{
		playerCharacters = _playerCharacters;

		//In case of null data
		if(SaveLoadManager.saveStats.level == 0)
		{
			SaveLoadManager.saveStats = new Character();

			for(int i = 0; i < SaveLoadManager.saveHealth.Length; i++)
			{
				SaveLoadManager.saveHealth[i] = 20000;
				SaveLoadManager.saveAP[i] = 20000;
				SaveLoadManager.saveShield[i] = 100;
			}
		}

		for(int i = 0; i < playerCharacters.Length; i++)
		{
			characterStats.Add (new Character(playerCharacters[i].name,
			                                  SaveLoadManager.saveStats.level,
			                                  SaveLoadManager.saveStats.currentShieldAffinity,
			                                  SaveLoadManager.saveShield[_indexHealth[i]],
			                                  SaveLoadManager.saveHealth[_indexHealth[i]],
			                                  SaveLoadManager.saveStats.levelExperience,
			                                  SaveLoadManager.saveStats.fireAffinity,
			                                  SaveLoadManager.saveStats.fireExperience,
			                                  SaveLoadManager.saveStats.waterAffinity,
			                                  SaveLoadManager.saveStats.waterExperience,
			                                  SaveLoadManager.saveStats.lightningAffinity,
			                                  SaveLoadManager.saveStats.lightningExperience,
			                                  SaveLoadManager.saveStats.earthAffinity,
			                                  SaveLoadManager.saveStats.earthExperience));  
			//print (SaveLoadManager.saveStats.level + " is the level");
			//print (SaveLoadManager.saveHealth[_indexHealth[i]] + " is the health");

			characterStats[i].currentAP = SaveLoadManager.saveAP[_indexHealth[i]];
		}

		//Set Current Indices
		combatIndices = _indexHealth;

		//Mark as info obtained
		hasPlayerInfo = true;
	}

	public void GetSpawnInformation(GameObject _spawner)
	{
		spawner = _spawner.GetComponent<CombatSpawner>();
		SetSpawnInformation ();
	}

	public void SetSpawnInformation()
	{
		spawner.AddPlayers(playerCharacters, characterStats);
		spawner.AddEnemies(enemyCharacters, enemyLevels);
		spawner.SpawnPlayers();
	}

	void GetCurrentPlayerInformation(GameObject _object)
	{
		_object.SendMessage ("GetLatestStats", latestStats[0], SendMessageOptions.DontRequireReceiver);
	}

	void SetCurrentPlayerInformation()
	{
		latestStats = new List<Character>();

		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			latestStats.Add (CombatManager.playerStats[i].character);
		}
	}

	void SetIndexPlayerInformation(int _currentIndex)
	{
		//Update Current Health Information
		latestStats[CombatManager.playerStats[_currentIndex].iniIndex].currentHealth = 
			CombatManager.playerStats[_currentIndex].stat.health;

		if(latestStats[CombatManager.playerStats[_currentIndex].iniIndex].currentHealth < 0)
		{
			latestStats[CombatManager.playerStats[_currentIndex].iniIndex].currentHealth = 1;
		}

		print (latestStats[CombatManager.playerStats[_currentIndex].iniIndex].currentHealth + " at index " + CombatManager.playerStats[_currentIndex].iniIndex);

		//Update Current AP Information
		latestStats[CombatManager.playerStats[_currentIndex].iniIndex].currentAP = 
			CombatManager.playerStats[_currentIndex].stat.actionPoints;
		
		if(latestStats[CombatManager.playerStats[_currentIndex].iniIndex].currentAP < 0)
		{
			latestStats[CombatManager.playerStats[_currentIndex].iniIndex].currentAP = 0;
		}
		
		print (latestStats[CombatManager.playerStats[_currentIndex].iniIndex].currentAP + " at index " + CombatManager.playerStats[_currentIndex].iniIndex);

	}

	//This function sets the latest stats to the SaveLoadManager
	void SetLatestStats()
	{
		//Just set the first character stat, the first character stat in the list is always main character
		SaveLoadManager.saveStats = latestStats[0];

		print ("Level Saved to SaveLoadManager is: " + latestStats[0].level + ". With " + latestStats[0].levelExperience + " experience.");

		for(int i = 0; i < combatIndices.Length; i++)
		{
			//Save the current healths
			SaveLoadManager.saveHealth[combatIndices[i]] = latestStats[i].currentHealth;
			//Save Current APs
			SaveLoadManager.saveAP[combatIndices[i]] = latestStats[i].currentAP;
			print (SaveLoadManager.saveHealth[combatIndices[i]] + " and " + latestStats[i].currentHealth + " at index " + combatIndices[i]);
		}
	}
}
