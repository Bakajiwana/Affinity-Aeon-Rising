using UnityEngine;
using System.Collections;

//Script Objective: A field that drains the AP of Enemies and Provides to player

public class IonaIonicField : MonoBehaviour 
{
	public Transform startParticles;
	public Transform drainParticles;
	public Transform destroyParticles;

	[Range(1,30)]
	public int accPercentage = 5;

	void Start()
	{
		if(startParticles)
		{
			Instantiate (startParticles, transform.position, transform.rotation);
		}
	}

	void DrainEnemies()
	{
		if(drainParticles)
		{
			Instantiate (drainParticles,Vector3.zero, Quaternion.identity);
		}

		int powerPercentage = 0;

		for(int i = 0; i < CombatManager.enemies.Count; i++)
		{
			CombatManager.enemyStats[i].APCost (15);
			CombatManager.enemyStats[i].ShowDamageText ("AP Drained", Color.white, 0.75f);
			powerPercentage += accPercentage;
		}

		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			CombatManager.playerStats[i].RegenAP (true, (float)powerPercentage/100f);
		}
	}

	void DestroyField()
	{
		if(destroyParticles)
		{
			Instantiate (destroyParticles, transform.position, transform.rotation);
		}

		Destroy (gameObject);
	}
}
