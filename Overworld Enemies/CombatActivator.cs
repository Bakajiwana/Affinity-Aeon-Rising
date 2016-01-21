using UnityEngine;
using System.Collections;

//Script Objective: When the player touches enemy, obtain all information required to start combat

public class CombatActivator : MonoBehaviour 
{
	//Combat Information Node
	public GameObject combatInformationNode;
	private GameObject combatInfo;

	//Enemy Spawn Variables
	public bool randomSpawn = false;

	[Tooltip("If Random Spawn is true. " +
			"Array of Spawnable enemies that will be randomly spawned. " +
	         "If Random Spawn is false. " +
	         "Array of Spawnable enemies that will be precisely spawned")]
	public GameObject[] spawnEnemies;
	private GameObject[] randomisedEnemies;
	private int[] randomisedLevels;

	[Tooltip("The Length of this array equal the length of the Spawn Enemy Array. "
	         + "Difference to Main Character Level.For auto-levelling, e.g. 2 or -3. " +
				"Writing -3 will have the Enemy Level equal Character Level - 3.")]
	public int[] levelDifference;
	private int[] enemyLevels;

	public int maxLevel;
	public int minLevel;

	public int maxEnemies;
	public int minEnemies;

	void InitialiseEnemies()
	{
		//Initialise Level
		enemyLevels = new int[levelDifference.Length];
		for(int i = 0; i < enemyLevels.Length; i++)
		{
			enemyLevels[i] = SaveLoadManager.saveStats.level + levelDifference[i];

			if(enemyLevels[i] < minLevel)
			{
				enemyLevels[i] = minLevel;
			}

			if(enemyLevels[i] > maxLevel)
			{
				enemyLevels[i] = maxLevel;
			}
		}
		
		//If Random Spawn is True, Randomly generate
		if(randomSpawn)
		{
			//Create a random Quantity of enemies
			int enemyQuantity = Random.Range (minEnemies, maxEnemies);
			
			//Create the randomised Enemies Arrays
			randomisedEnemies = new GameObject[enemyQuantity];
			randomisedLevels = new int[enemyQuantity];
			
			//Put Random Enemies and their levels in the randomised arrays
			for(int i = 0; i < randomisedEnemies.Length; i++)
			{
				int randomIndex = Random.Range (0, spawnEnemies.Length);
				
				randomisedEnemies[i] = spawnEnemies[randomIndex];
				randomisedLevels[i] = enemyLevels[randomIndex];
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		//If Collided with Player
		if(other.CompareTag ("Player"))
		{
			StartBattle(other.gameObject);
		}
	}

	public void StartBattle(GameObject other)
	{
		if(!combatInfo)
		{
			//Initialise Enemy Levels and Prefabs
			InitialiseEnemies ();
			
			//Instantiate Combat Information Node
			combatInfo = Instantiate(combatInformationNode,
			                                    transform.position, transform.rotation) as GameObject;
			
			CombatInformation info = combatInfo.gameObject.GetComponent<CombatInformation>();
			
			//Send Enemy Information
			if(randomSpawn)
			{
				info.SetEnemyInformation (randomisedEnemies, randomisedLevels);
			}
			else
			{
				info.SetEnemyInformation (spawnEnemies, enemyLevels);
			}
			
			//Update the Enemy Management
			EnemyManagement.enemyUpdate = false;
			//Update Character Management
			CharacterManager.onLoaded = false;
			
			//Send Information to Enemy Management
			transform.parent.gameObject.SendMessage("MarkEnemy", transform, SendMessageOptions.DontRequireReceiver);
			//Send Information to Character Manager
			CharacterManager characterManager = GameObject.FindGameObjectWithTag ("Adventure Manager").GetComponent<CharacterManager>();
			
			characterManager.UpdatePlayerPosition (other.transform.position, other.transform.eulerAngles);
			
			
			//Destroy Enemy
			//Destroy (gameObject);
		}
	}
}
