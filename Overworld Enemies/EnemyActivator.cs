using UnityEngine;
using System.Collections;

public class EnemyActivator : MonoBehaviour 
{
	public bool randomEncounter = false;
	[Range(0f,1f)]
	public float randomEncounterChance = 0.5f;

	public Transform[] environmentActivate;
	public float speedOfEnvironmentSpawn = 0.1f;
	private int currentObject = 0;
	public Transform[] enemies;
	public float speedOfEnemySpawn = 0.4f;
	private int currentEnemy = 0;

	void Awake()
	{
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		gameObject.GetComponent<Collider>().isTrigger = true;

		for(int i = 0; i < environmentActivate.Length; i++)
		{
			environmentActivate[i].gameObject.SetActive (false);
		}
	}

	//If Player runs on trigger activate everything
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag ("Player"))
		{
			if(randomEncounter)
			{
				float randomChance = Random.Range (0f,1f);
				if(randomChance <= randomEncounterChance)
				{
					SpawnEverything ();
				}
			}
			else
			{
				SpawnEverything ();
			}

			gameObject.GetComponent<Collider>().enabled = false;
		}
	}

	void SpawnEverything()
	{
		//Repeatedly Spawn Objects and enemies
		InvokeRepeating ("SpawnObject", 0f, speedOfEnvironmentSpawn);
		InvokeRepeating ("SpawnEnemy", 0f, speedOfEnemySpawn);
	}

	void SpawnObject()
	{
		if(currentObject < environmentActivate.Length)
		{
			environmentActivate[currentObject].gameObject.SetActive (true);
			currentObject++;
		}
		else
		{
			CancelInvoke ("SpawnObject");
		}
	}

	void SpawnEnemy()
	{
		if(currentEnemy < enemies.Length)
		{
			enemies[currentEnemy].gameObject.SetActive (true);
			currentEnemy++;
		}
		else
		{
			CancelInvoke ("SpawnEnemy");
		}
	}
}
