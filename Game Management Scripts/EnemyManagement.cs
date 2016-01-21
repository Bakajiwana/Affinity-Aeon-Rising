using UnityEngine;
using System.Collections;

//Manages the Enemies in the Overworld, takes into a Static Array and chooses whether they should active or not

public class EnemyManagement : MonoBehaviour 
{
	//Static Variables
	public static bool enemyUpdate = true; //Triggered by loading
	public static Transform[] worldEnemies;
	public static bool[] worldEnemiesActive;

	// Use this for initialization
	void Start () 
	{
		//When Scene Starts up Manage which enemy is active
		ManageEnemyExistence ();

		//print (worldEnemiesActive[1]);
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void MarkEnemy(Transform _object)
	{
		for(int i = 0; i < worldEnemies.Length; i++)
		{
			if(_object == worldEnemies[i])
			{
				worldEnemiesActive[i] = false;
				//print (i);
				break;
			}
		}
	}

	//This Procedure is called to re update and initialise the static variables 
	void UpdateEnemyManagement()
	{
		worldEnemiesActive = new bool[worldEnemies.Length];

		for(int i = 0; i < worldEnemies.Length; i++)
		{
			worldEnemiesActive[i] = true;
		}
	}

	//This Procedure is called to manage the existence of each enemies depending on the worldEnemiesActive bool array
	void ManageEnemyExistence()
	{
		//Get Every Child Object into the Static Combat Activator Array
		CombatActivator [] enemies = gameObject.GetComponentsInChildren<CombatActivator>();
		worldEnemies = new Transform[enemies.Length];
		for(int i = 0; i < enemies.Length; i++)
		{
			worldEnemies[i] = enemies[i].gameObject.transform;
		}

		//Enemy Update is called from the save load manager, it acts as a trigger to update the array of enemies
		if(enemyUpdate || worldEnemiesActive == null)
		{
//			print ("reset enemies");
			UpdateEnemyManagement ();
			enemyUpdate = false;
		}


		for(int i = 0; i < worldEnemies.Length; i++)
		{
			if(worldEnemies[i])
			{
			//	print(worldEnemies[i].name);
				worldEnemies[i].gameObject.SetActive (worldEnemiesActive[i]);
			}
		}
	}
}
