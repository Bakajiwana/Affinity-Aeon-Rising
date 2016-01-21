using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This script will handle the spawning of the characters when combat begins.
//Also send stat information from previous scene

public class CombatSpawner : MonoBehaviour 
{
	//The Characters to spawn
	private GameObject [] playerCharacters;
	private GameObject [] enemyCharacters;

	//Stats of the characters
	private int[] enemyLevel;
	public List<Character> character = new List<Character>();

	//Reference the spawned characters
	private GameObject[] player;
	private GameObject[] enemy;

	// Use this for initialization
	void Start () 
	{
		//SpawnPlayers ();

		//Obtain Player and Enemy Information from the Combat Information Node 
		GameObject.FindGameObjectWithTag ("Combat Spawner").SendMessage ("GetSpawnInformation", gameObject, SendMessageOptions.DontRequireReceiver);
	}

	void OnLevelWasLoaded(int level)
	{
		//print ("Hello I'm ready to spawn things");
		//SpawnPlayers();
	}


	//This function is to spawn the characters into the battlefield
	public void SpawnPlayers()
	{
		//Spawn the Players
		GameObject playerSide = GameObject.Find ("Player Combat Side");
		GameObject playerPositions = GameObject.Find ("Player Spawn Positions");

		//Firstly spawn all the players into the battlefield using thier array and positon arrays
		for(int i = 0; i < playerCharacters.Length; i++)
		{
			player[i] = Instantiate (playerCharacters[i], playerPositions.transform.GetChild(i).position, playerPositions.transform.GetChild (i).rotation) as GameObject;
			player[i].transform.SetParent (playerSide.transform, true);
			
			//Send the character list to the player character
			player[i].SendMessage ("InitiatePlayerStats", character[i], SendMessageOptions.DontRequireReceiver);

			//print (character[i].currentHealth);
			//Debug.Log (character[i].currentHealth);
		}

		//Spawn the Enemies
		GameObject enemySide = GameObject.Find ("Enemy Combat Side"); 		
		GameObject enemyPositions = GameObject.Find ("Enemy Spawn Positions"); 		
		
		//Secondly Spawn all enemies into the battlefield
		//If More than one enemy
		if(enemyCharacters.Length > 1)
		{
			for (int i = 0; i < enemyCharacters.Length; i++)
			{
				enemy[i] = Instantiate (enemyCharacters[i], enemyPositions.transform.GetChild (i).position, enemyPositions.transform.GetChild(i).rotation) as GameObject;
				enemy[i].transform.SetParent (enemySide.transform, true);
				
				//Send the enemy its specified level

				//Use Playerprefs Difficulty to change the level
				int finalEnemyLevel = enemyLevel[i];
				
				switch(PlayerPrefs.GetInt ("Difficulty"))
				{
				case 1: //Easy
					finalEnemyLevel -= 2;
					break;
				case 3: //Hard
					finalEnemyLevel += 2;
					break;
				case 4: //Synergist ultra hard
					finalEnemyLevel += 4;
					break;
				}

				if(finalEnemyLevel <= 0)
				{
					finalEnemyLevel = 1;
				}

				enemy[i].SendMessage ("InitiateEnemyStats", finalEnemyLevel, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			enemy[0] = Instantiate (enemyCharacters[0], enemyPositions.transform.GetChild (1).position, enemyPositions.transform.GetChild(1).rotation) as GameObject;
			enemy[0].transform.SetParent (enemySide.transform, true);

			//Send the enemy its specified level
			//Use Playerprefs Difficulty to change the level
			int finalEnemyLevel = enemyLevel[0];

			switch(PlayerPrefs.GetInt ("Difficulty"))
			{
			case 1: //Easy
				finalEnemyLevel -= 2;
				break;
			case 3: //Hard
				finalEnemyLevel += 2;
				break;
			case 4: //Synergist ultra hard
				finalEnemyLevel += 4;
				break;
			}

			if(finalEnemyLevel <= 0)
			{
				finalEnemyLevel = 1;
			}


			enemy[0].SendMessage ("InitiateEnemyStats", finalEnemyLevel, SendMessageOptions.DontRequireReceiver);
		}
	}

	//This function is for the player to communicate to this spawner its array that will spawn in the battlefield
	public void AddPlayers(GameObject[] players, List<Character> characterStat)
	{
		playerCharacters = players;
		//print ("characterStat contains Health " + characterStat[0].currentHealth);
		character = new List<Character>();
		character = characterStat;
		player = new GameObject[playerCharacters.Length];
	}

	//This function is for the enemy to communicate to this spawner its array that will spawn in the battlefield
	public void AddEnemies (GameObject[] enemies, int[] levels)
	{
		enemyCharacters = enemies;
		enemyLevel = levels;
		enemy = new GameObject[enemyCharacters.Length];

		//print (enemyCharacters.Length);
	}
}
