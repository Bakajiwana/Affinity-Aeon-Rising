using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//This script will manage the overall combat

//Manager tells character whether its their turn.

public class CombatManager : MonoBehaviour 
{
	//Start Battle Timer
	public float battleStartTime = 3f;

	//Player and Enemy Side Nodes
	public GameObject playerSide;
	public GameObject enemySide;
	public GameObject environmentInteractionSide; //Environment Interaction objects zone
	public GameObject environmentCompleted;

	//Current Player
	[HideInInspector]
	public static GameObject currentPlayer;

	//Reference to players and enemies, using lists because it will be dynamic, so adds and removals etc
	public static List<GameObject> players = new List<GameObject>();
	public static List<GameObject> enemies = new List<GameObject>();

	//Reference Environment Interaction
	public static List<GameObject> environmentObjects = new List<GameObject>();

	//The stats of the players and enemies
	public static List<PlayerCombatCharacter> playerStats = new List<PlayerCombatCharacter>();
	public static List<EnemyCombatCharacter> enemyStats = new List<EnemyCombatCharacter>();

	//Accumulative Speeds - Determines the Time of Action
	private List<int> accPlayerSpeeds = new List<int>();
	private List<int> accEnemySpeeds = new List<int>();

	//Combat state 0=The combat is still going, 1=The player has won, 2=the player has lost
	public static int combatState = 0;

	//Applies to to all timescale lines except for pausing
	public static float currentTimeScale = 1f; 
	public static float skipTimeScale = 10f;

	//Special Paradox - Life and Death Elemental Paradox
	public static bool specialParadox = false;
	private bool performingSpecial = false;

	//Experience
	public static long experienceEarned;
	public int experienceElementEarned = 200;
	public static long experienceElemental;

	//Overworld Loading Variables
	AsyncOperation async;
	private bool allowAsync = false;
	private CombatInformation combatInfo;

	private float turnTime = 0f;

	// Use this for initialization
	void Awake()
	{		
		playerStats = new List<PlayerCombatCharacter>();
		enemyStats = new List<EnemyCombatCharacter>();
		
		players = new List<GameObject>();
		enemies = new List<GameObject>();

		playerStats = new List<PlayerCombatCharacter>();
		enemyStats = new List<EnemyCombatCharacter>();

		environmentObjects = new List<GameObject>();

		accPlayerSpeeds = new List<int>();
		accEnemySpeeds = new List<int>();

		specialParadox = false;
		currentTimeScale = 1f;

		combatState = 0;

		//Experience Elemental Update
		experienceElemental = experienceElementEarned;

		experienceEarned = 0;
	}

	void Update()
	{
		turnTime += Time.deltaTime;
		//print (turnTime);

		if(turnTime > 100f && combatState == 0)
		{
			ResetBattleTurns();
		}

		if(combatState != 0)
		{
			Time.timeScale = 1f;	//Reset Timescale
		}
	}

	void Start () 
	{
		//Acquire the players and enemies, then acquire their speeds and begin Time of Action.
		//Using Invoke because there should be a little break when battle begins to set up stats and everything
		Invoke ("InitialiseBattle", battleStartTime);

		//Obtain Overworld name from Combat Spawner
		combatInfo = GameObject.FindGameObjectWithTag ("Combat Spawner").GetComponent<CombatInformation>();

		if(!Application.isEditor)
		{
			allowAsync = true;
		}

		PrepareOverworldLoad ();

		InvokeRepeating ("CheckPlayersAndEnemies", 40f, 40f);
	}

	//This enumerator checks if the players or enemies have been defeated.
	IEnumerator CheckCombatState()
	{
		while(combatState == 0)
		{
			if(enemies.Count == 0)
			{
				combatState = 1;
				CombatCamera.control.SpawnGlobalAnimation (null, "Player Victory");
				Invoke("EndBattle", battleStartTime);
			}			
			else if(players.Count == 0)
			{
				combatState = 2;
				CombatCamera.control.SpawnGlobalAnimation (null, "View Players");
				Invoke("EndBattle", battleStartTime);
			}
			else
			{
				combatState = 0;
			}

			yield return new WaitForSeconds(1f);
		}
	}



	//This function is used to acquire players and enemies into their respective lists
	void InitialiseBattle()
	{
		//Obtain player and enemy objects - Fill the players and enemies lists
		foreach (Transform child in playerSide.transform)
		{
			players.Add (child.gameObject);
		}

		foreach (Transform child in enemySide.transform)
		{
			enemies.Add (child.gameObject);
		}

		//Obtain Environment objects - Fill EnvironmentObjects list
		foreach (Transform child in environmentInteractionSide.transform)
		{
			environmentObjects.Add (child.gameObject);
		}

		//Obtain the player and enemy stats - Fill the player and enemy stats lists
		for(int i = 0; i < players.Count; i++)
		{
			playerStats.Add (players[i].GetComponent<PlayerCombatCharacter>());	//Obtain the stat components

			//Initialise index
			playerStats[i].iniIndex = i;
		}

		for(int i = 0; i < enemies.Count; i++)
		{
			enemyStats.Add (enemies[i].GetComponent<EnemyCombatCharacter>()); 	//Obtain the stat components
		}

		//Initialise the time of action - Fill the accumulative speed lists
		for(int i = 0; i < players.Count; i++)
		{
			accPlayerSpeeds.Add (0);	//Everyone starts at 0, over turns they accumulate
		}

		for(int i = 0; i < enemies.Count; i++)
		{
			accEnemySpeeds.Add (0);
		}

		//Initialise UI Panel
		GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("InitialisePartyPanel", SendMessageOptions.DontRequireReceiver);

		//Initialise Time of Action
		GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("ResetTimeOfAction", SendMessageOptions.DontRequireReceiver);

		//Set the current player information
		GameObject.FindGameObjectWithTag ("Combat Spawner").SendMessage ("SetCurrentPlayerInformation", SendMessageOptions.DontRequireReceiver);


		NextTurn ();		//Determine the next characters turn
		StartCoroutine (CheckCombatState ());	//Activate the Enumerator that checks who will win
	}

	//This function is to add the speeds and update the time of action
	void UpdateTimeOfAction()
	{		
		for (int i = 0; i < players.Count; i++)
		{
			accPlayerSpeeds[i] += playerStats[i].stat.speed;
			//print ("Player" + i + " = " + accPlayerSpeeds[i]);
		}
		
		for (int i = 0; i < enemies.Count; i++)
		{
			accEnemySpeeds[i] += enemyStats[i].stat.speed;
			//print ("Enemy" + i + " = " + accEnemySpeeds[i]);
		}

		//After all speed calculated, Send the speed values to the CombatUIManager to update Time of Action

		//Create a temporary array 
		int maxCharacters = CombatManager.players.Count + CombatManager.enemies.Count;
		int[] speeds = new int[maxCharacters];

		//Add the player speeds into the speeds array
		for(int i = 0; i < players.Count; i++)
		{
			speeds[i] = accPlayerSpeeds[i];
		}

		//Add the enemy speeds into the speeds array
		for(int i = 0; i < enemies.Count; i++)
		{
			speeds[i + players.Count] = accEnemySpeeds[i];
		}

		//Send to CombatUIManager to update Time of Action UI
		GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("UpdateTimeOfAction", speeds, SendMessageOptions.DontRequireReceiver);
	}

	//This function will update the time of action
	void NextTurn()
	{
		Time.timeScale = 1f;	//Reset Timescale
		turnTime = 0f;

		//If not performing a special then continue to next turn
		if(!performingSpecial)
		{
			if(players.Count > 0 && enemies.Count > 0)
			{
				//Update the Time of Action 
				UpdateTimeOfAction ();

				//Calculate the accumulated values and determine the highest turn
				int _nextPlayerSpeed = accPlayerSpeeds[0];
				int _nextPlayer = 0;
				for (int i = 0; i < accPlayerSpeeds.Count; i++)
				{
					if(accPlayerSpeeds[i] > _nextPlayerSpeed)
					{
						_nextPlayerSpeed = accPlayerSpeeds[i];
						_nextPlayer = i;
					}
				}

				int _nextEnemySpeed = accEnemySpeeds[0];
				int _nextEnemy = 0;
				for (int i = 0; i < accEnemySpeeds.Count; i++)
				{
					if(accEnemySpeeds[i] > _nextEnemySpeed)
					{
						_nextEnemySpeed = accEnemySpeeds[i];
						_nextEnemy = i;
					}
				}

				//Then compare the players highest value and the enemies highest value 
				//print (_nextPlayer + " " + _nextPlayerSpeed);
				//print (_nextEnemy + " " + _nextEnemySpeed);
				if(_nextPlayerSpeed >= _nextEnemySpeed)
				{
					//Set Player as current, but don't activate just yet, mode must be selected first
					currentPlayer = players[_nextPlayer];

					//Activate Specified Player
					players[_nextPlayer].SendMessage ("Activate", SendMessageOptions.DontRequireReceiver);


					//Reset that specific player's accumulated speed
					accPlayerSpeeds[_nextPlayer] = 0; 
				}
				else
				{
					//Activate Specified Enemy
					enemies[_nextEnemy].SendMessage ("Activate", SendMessageOptions.DontRequireReceiver);

					//Reset that specific Enemy's accumulated speed
					accEnemySpeeds[_nextEnemy] = 0;
				}
			}
		}
		else
		{
			//If performing special delay turn and show special screen
			GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("ActivateSpecialParadoxScreen", SendMessageOptions.DontRequireReceiver);
		}
	}

	/*This function is called to activate the player
	public void ActivateNextPlayer(int _mode)
	{
		//Activate The player
		currentPlayer.SendMessage ("Activate", SendMessageOptions.DontRequireReceiver);
		//Set the Mode of action
		currentPlayer.SendMessage ("ActivateMode", _mode, SendMessageOptions.DontRequireReceiver);
	}
	*/

	//These functions are called to remove and insert players and enemies to the list this manager depends on
	public void RemovePlayer(int _index)
	{
		players.RemoveAt (_index);
		playerStats.RemoveAt (_index);
		accPlayerSpeeds.RemoveAt (_index);

		//Reset Time of Action
		GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("ResetTimeOfAction", SendMessageOptions.DontRequireReceiver);
	}

	public void InsertPlayer(GameObject _object)
	{
		//Parent to Player Side
		_object.transform.SetParent (playerSide.transform, true);
		_object.SetActive (true);

		//Obtain the Character script
		PlayerCombatCharacter playerStat = _object.GetComponent <PlayerCombatCharacter>();

		//Calculate Legible Index
		int legibleIndex = 0;

		if(playerStat.iniIndex > playerStats.Count)
		{
			legibleIndex = playerStats.Count;
		}
		else
		{
			legibleIndex = playerStat.iniIndex;
		}

		//Add to GameObject List
		players.Insert (legibleIndex, _object);

		//Add to Stat List
		playerStats.Insert (legibleIndex, playerStat);
		playerStat.stat.health = playerStat.stat.healthBase/2;
		playerStat.stat.actionPoints = playerStat.stat.actionPointBase/2;
		playerStat.statusUI.UpdateStatHealth();
		playerStat.statusUI.UpdateStatActionPoints();

		//Add to Speed List
		accPlayerSpeeds.Insert (legibleIndex, 0);


		//Reset Time of Action
		GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("ResetTimeOfAction", SendMessageOptions.DontRequireReceiver);
	}

	public void RemoveEnemy(int _index)
	{
		enemies.RemoveAt (_index);
		enemyStats.RemoveAt (_index);
		accEnemySpeeds.RemoveAt (_index);

		//Reset Time of Action
		GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("ResetTimeOfAction", SendMessageOptions.DontRequireReceiver);
	}

	public void InsertEnemy(GameObject _object)
	{
		//Parent to Enemy Side
		_object.transform.SetParent (enemySide.transform, true);
		
		//Obtain the Character script
		EnemyCombatCharacter enemyStat = _object.GetComponent <EnemyCombatCharacter>();
		
		//Calculate Legible Index
		int legibleIndex = 0;
		
		if(enemyStat.iniIndex > enemyStats.Count)
		{
			legibleIndex = enemyStats.Count;
		}
		else
		{
			legibleIndex = enemyStat.iniIndex;
		}
		
		//Add to GameObject List
		enemies.Insert (legibleIndex, _object);
		
		//Add to Stat List
		enemyStats.Insert (legibleIndex, enemyStat);
		
		//Add to Speed List
		accEnemySpeeds.Insert (legibleIndex, 0);

		//Reset Time of Action
		GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("ResetTimeOfAction", SendMessageOptions.DontRequireReceiver);
	}

	//This function removes environment interactibles
	public void RemoveEnvironmentObject(int _index)
	{
		//Parent to completed object
		environmentObjects[_index].transform.SetParent (environmentCompleted.transform, false);

		//Remove from list
		environmentObjects.RemoveAt (_index);
	}

	public void AddEnvironmentObject(GameObject _object)
	{
		//Parent to Object Revealed
		_object.transform.SetParent (environmentInteractionSide.transform, true);

		//Obtain Script
		CombatEnvironmentInteraction objectStat = _object.GetComponent<CombatEnvironmentInteraction>();

		//Calculate Legible Index
		int legibleIndex = 0;

		if(objectStat.index > environmentObjects.Count)
		{
			legibleIndex = environmentObjects.Count;
		}
		else
		{
			legibleIndex = objectStat.index; 
		}

		//Add to GameObject List
		environmentObjects.Insert (legibleIndex, _object);
	}

	//This function is called when special paradox is ready
	public void SpecialParadoxActivate()
	{
		print ("activate paradox");
		performingSpecial = true;
		CombatCamera.control.SpawnGlobalAnimation (null, "View Enemies");
	}

	public void LifeParadox()
	{
		CombatUIManager ui = GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<CombatUIManager>();

		int scrollCount = 0;

		for(int i = 0; i < players.Count; i++)
		{
			switch(players[i].name)
			{
			case "Aeon(Clone)":
				Transform spawn = Instantiate (ui.teamParadoxFrames[0], ui.overlayScreen.position,  ui.overlayScreen.rotation) as Transform;
				spawn.SetParent (ui.overlayScreen);

				if(scrollCount == 0)
				{
					spawn.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Up");
					scrollCount++;
				}
				else
				{
					spawn.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Down");
					scrollCount = 0;
				}

				break;
			case "Iona(Clone)":
				Transform spawn1 = Instantiate (ui.teamParadoxFrames[1],  ui.overlayScreen.position,  ui.overlayScreen.rotation) as Transform;
				spawn1.SetParent (ui.overlayScreen);

				if(scrollCount == 0)
				{
					spawn1.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Up");
					scrollCount++;
				}
				else
				{
					spawn1.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Down");
					scrollCount = 0;
				}

				break;
			case "Taven(Clone)":
				Transform spawn2 = Instantiate (ui.teamParadoxFrames[2],  ui.overlayScreen.position,  ui.overlayScreen.rotation) as Transform;
				spawn2.SetParent (ui.overlayScreen);

				if(scrollCount == 0)
				{
					spawn2.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Up");
					scrollCount++;
				}
				else
				{
					spawn2.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Down");
					scrollCount = 0;
				}

				break;
			case "Airen(Clone)":
				Transform spawn3 = Instantiate (ui.teamParadoxFrames[3],  ui.overlayScreen.position,  ui.overlayScreen.rotation) as Transform;
				spawn3.SetParent (ui.overlayScreen);

				if(scrollCount == 0)
				{
					spawn3.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Up");
					scrollCount++;
				}
				else
				{
					spawn3.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Down");
					scrollCount = 0;
				}
				break;
			}
		}

		Invoke ("ParadoxHeal", 7f);
		Invoke("NextTurn", 7f);

		Invoke ("LifeEffect", 2f);
	}

	void LifeEffect()
	{
		CombatUIManager ui = GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<CombatUIManager>();

		Transform spawn = Instantiate (ui.paradoxLifeScreenEffect, transform.position, transform.rotation) as Transform;
		spawn.SetParent (ui.overlayScreen);

		CombatCamera.control.SpawnGlobalAnimation (null, "View Players");
		CombatCamera.control.DelayStop (2f);
	}

	public void DeathParadox()
	{
		CombatUIManager ui = GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<CombatUIManager>();
		
		int scrollCount = 0;
		
		for(int i = 0; i < players.Count; i++)
		{
			switch(players[i].name)
			{
			case "Aeon(Clone)":
				Transform spawn = Instantiate (ui.teamParadoxFrames[0], ui.overlayScreen.position,  ui.overlayScreen.rotation) as Transform;
				spawn.SetParent (ui.overlayScreen);
				
				if(scrollCount == 0)
				{
					spawn.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Up");
					scrollCount++;
				}
				else
				{
					spawn.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Down");
					scrollCount = 0;
				}
				
				break;
			case "Iona(Clone)":
				Transform spawn1 = Instantiate (ui.teamParadoxFrames[1],  ui.overlayScreen.position,  ui.overlayScreen.rotation) as Transform;
				spawn1.SetParent (ui.overlayScreen);
				
				if(scrollCount == 0)
				{
					spawn1.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Up");
					scrollCount++;
				}
				else
				{
					spawn1.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Down");
					scrollCount = 0;
				}
				
				break;
			case "Taven(Clone)":
				Transform spawn2 = Instantiate (ui.teamParadoxFrames[2],  ui.overlayScreen.position,  ui.overlayScreen.rotation) as Transform;
				spawn2.SetParent (ui.overlayScreen);
				
				if(scrollCount == 0)
				{
					spawn2.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Up");
					scrollCount++;
				}
				else
				{
					spawn2.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Down");
					scrollCount = 0;
				}
				
				break;
			case "Airen(Clone)":
				Transform spawn3 = Instantiate (ui.teamParadoxFrames[3],  ui.overlayScreen.position,  ui.overlayScreen.rotation) as Transform;
				spawn3.SetParent (ui.overlayScreen);
				
				if(scrollCount == 0)
				{
					spawn3.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Up");
					scrollCount++;
				}
				else
				{
					spawn3.gameObject.GetComponent<Animator>().SetTrigger ("Scroll Down");
					scrollCount = 0;
				}
				break;
			}
		}

		Invoke ("ParadoxDamage", 7f);
		Invoke("NextTurn", 7f);

		Invoke ("DeathEffect", 2f);
	}

	void DeathEffect()
	{
		CombatUIManager ui = GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<CombatUIManager>();

		Transform spawn = Instantiate (ui.paradoxDeathScreenEffect, transform.position, transform.rotation) as Transform;
		spawn.SetParent (ui.overlayScreen);

		CombatCamera.control.SpawnGlobalAnimation (null, "View Enemies");
		CombatCamera.control.DelayStop (2f);
	}

	void ParadoxDamage()
	{
		//All enemies are damaged and all debuffs applied
		for(int i = 0; i < enemyStats.Count; i++)
		{
			//Damage
			float damage = (((float)playerStats[0].character.level) + 1f) * playerStats[0].stat.attack;
			enemyStats[i].SetDamage (playerStats[0].stat, 0, (int)damage, 0f, 0f);
		}

		specialParadox = true;
		performingSpecial = false;
	}

	void ParadoxHeal()
	{
		//Check Downed players
		PlayerCombatCharacter[] allPlayers = playerSide.GetComponentsInChildren <PlayerCombatCharacter>(true); 

		for(int i = 0; i < allPlayers.Length; i++)
		{
			print(allPlayers[i].name);
			if(allPlayers[i].stat.health <= 0)
			{
				InsertPlayer (allPlayers[i].gameObject);
			}
		}

		//All players are healed and AP restured
		for(int i = 0; i < playerStats.Count; i++)
		{
			//Heal
			playerStats[i].SetHeal (true, 0.5f);
			//AP
			playerStats[i].RegenAP (true, 0.5f);
		}
		
		specialParadox = true;
		performingSpecial = false;
	}

	void CheckPlayersAndEnemies()
	{
		if(combatState == 0)
		{
			//Make sure no one is stuck
			bool isPlayerActive = false;
			bool isEnemyActive = false;
			
			for(int i = 0; i < players.Count; i++)
			{
				if(players[i].GetComponent<CombatActionActivator>().IsActionScriptActive())
				{
					isPlayerActive = true;
				}
			}
			
			//This is for enemies
			for(int i = 0; i < enemies.Count; i++)
			{
				if(enemies[i].GetComponent<CombatActionActivator>().IsActionScriptActive())
				{
					isEnemyActive = true;
				}
			}
			
			if(!isPlayerActive && !isEnemyActive)
			{
				if(!performingSpecial)
				{
					ResetBattleTurns ();
				}
				print ("Combat is stuck attempting to fix");
			}
			else
			{
				print ("All good");
			}
		}
	}

	//This procedure is called if combat gets stuck, the skip speed up should determine whether something is taking too long
	void ResetBattleTurns()
	{
		//Deactivate everyones Combat Action Scripts
		//This is for the Players lol
		for(int i = 0; i < players.Count; i++)
		{
			players[i].GetComponent<CombatActionActivator>().DisableTurn ();
		}
		
		//This is for enemies
		for(int i = 0; i < enemies.Count; i++)
		{
			enemies[i].GetComponent<CombatActionActivator>().DisableTurn ();
		}
		
		NextTurn ();
	}

	//These functions are called if the player or enemy has won
	void EndBattle()
	{
		//1 = win, 2 = lost
		if(combatState == 1)
		{
			GameObject.FindGameObjectWithTag("Combat UI").SendMessage("SwitchToWinScreen", SendMessageOptions.DontRequireReceiver);
		}
		else if (combatState == 2)
		{
			GameObject.FindGameObjectWithTag("Combat UI").SendMessage("SwitchToLoseScreen", SendMessageOptions.DontRequireReceiver);
		}

		Time.timeScale = 1f;	//Reset Timescale

		//Unlock Mouse
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	public void PrepareOverworldLoad()
	{
		if(allowAsync)
		{
			StartCoroutine (LoadOverworldLevel(combatInfo.overworldSceneName));
			print (combatInfo.overworldSceneName);
		}
		Application.backgroundLoadingPriority = ThreadPriority.Normal;
	}
	
	IEnumerator LoadOverworldLevel(string level)
	{
		async = Application.LoadLevelAsync (level);
		async.allowSceneActivation = false;
		yield return async;
	}
	
	public void OverworldSceneActivate()
	{
		//Application.LoadLevel ("Combat Prototype Scene");
		if(allowAsync)
		{
			async.allowSceneActivation = true;
		}
		else
		{
			Application.LoadLevel (combatInfo.overworldSceneName);
		}
	}

	//This function is called when Returning to Overworld
	void ReturnToOverworld()
	{
		OverworldSceneActivate ();
	}

	//This function is called when Retrying Battle
	void RetryBattle(TravelManager _travel)
	{
		//Application.LoadLevel (Application.loadedLevel);
		for(int i = 0; i < SaveLoadManager.saveHealth.Length; i++)
		{
			SaveLoadManager.saveHealth[i] = 20000;
			SaveLoadManager.saveAP[i] = 20000;
		}

		CharacterManager.onLoaded = true;
		EnemyManagement.enemyUpdate = true;
		_travel.InitiateTravel ("Destination", combatInfo.overworldSceneName);
	}
}
