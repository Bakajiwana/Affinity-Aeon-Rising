using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatTest : MonoBehaviour {

	public CombatSpawner spawner;

	public int[] playerLevels;

	public GameObject [] playerCharacters;
	public List<Character> characterStats = new List<Character>();

	public GameObject [] enemyCharacters;
	public int [] enemyLevels;

	// Use this for initialization
	void Awake () 
	{
		GameObject[] combatSpawners = GameObject.FindGameObjectsWithTag ("Combat Spawner");
		if(combatSpawners.Length > 1)
		{
			gameObject.SetActive (false);
		}
		else
		{
			characterStats.Add (new Character("Aeon", playerLevels[0], 2, 300, 50000, 0, 7, 0, 7, 0, 7, 0, 7, 0));
			characterStats.Add (new Character("Iona",playerLevels[1], 2, 300, 50000, 0, 7, 0, 7, 0, 7, 0, 7, 0));
			characterStats.Add (new Character("Airen",playerLevels[2], 2, 300, 50000, 0, 7, 0, 7, 0, 7, 0, 7, 0));
			
			spawner.AddPlayers(playerCharacters, characterStats);
			spawner.AddEnemies(enemyCharacters, enemyLevels);
			
			spawner.SpawnPlayers();
		}
	}
}
