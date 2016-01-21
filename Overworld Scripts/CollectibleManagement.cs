using UnityEngine;
using System.Collections;

public class CollectibleManagement : MonoBehaviour 
{
	//Static Variables
	public static Transform[] collectibles;
	public static bool[] collectiblesActive;
	
	// Use this for initialization
	void Start () 
	{
		//When Scene Starts up Manage which enemy is active
		ManageExistence ();
		
		//print (collectiblesActive[1]);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	void MarkObject(Transform _object)
	{
		for(int i = 0; i < collectibles.Length; i++)
		{
			if(_object == collectibles[i])
			{
				collectiblesActive[i] = false;
				//print (i);
				break;
			}
		}
	}
	
	//This Procedure is called to re update and initialise the static variables 
	void UpdateManagement()
	{
		collectiblesActive = new bool[collectibles.Length];
		
		for(int i = 0; i < collectibles.Length; i++)
		{
			collectiblesActive[i] = true;
		}
	}
	
	//This Procedure is called to manage the existence of each enemies depending on the collectiblesActive bool array
	void ManageExistence()
	{
		//Get Every Child Object into the Static Combat Activator Array
		CollectiblesScript [] enemies = gameObject.GetComponentsInChildren<CollectiblesScript>();
		collectibles = new Transform[enemies.Length];
		for(int i = 0; i < enemies.Length; i++)
		{
			collectibles[i] = enemies[i].gameObject.transform;
		}
		
		//Enemy Update is called from the save load manager, it acts as a trigger to update the array of enemies
		if(EnemyManagement.enemyUpdate || collectiblesActive == null)
		{
			//			print ("reset enemies");
			UpdateManagement ();
			//EnemyManagement.enemyUpdate = false;
		}
		
		for(int i = 0; i < collectibles.Length; i++)
		{
			if(collectibles[i])
			{
				collectibles[i].gameObject.SetActive (collectiblesActive[i]);
			}
		}
	}
}
