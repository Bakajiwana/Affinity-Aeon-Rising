using UnityEngine;
using System.Collections;

public class BombBotScript : MonoBehaviour 
{
	//Self Destruct

	private EnemyCombatActions combatAction;

	public Transform selfDestructParticles; 
	
	public int chargeMaxProgress = 3;
	private int chargeProgress = 0;
	
	void Start()
	{
		combatAction = transform.parent.gameObject.GetComponent<EnemyCombatActions>();
	}
	
	
	public void Charging()
	{
		//Increment Charge
		chargeProgress++;

		if(chargeProgress > chargeMaxProgress)
		{
			combatAction.anim.SetTrigger ("Next");

			//Set Global Message
			GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetGlobalMessage", "Self Destruct", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			//End Turn
			combatAction.EndTurnDelay (2f);

			if(chargeProgress == 1)
			{
				//Set Global Message
				GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetGlobalMessage", "Self Destruct in " + 
				                                                            ((chargeMaxProgress - chargeProgress) + 1).ToString () + " Turn", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				//Set Global Message
				GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetGlobalMessage", "Self Destruct in " + 
			                                                            ((chargeMaxProgress - chargeProgress) + 1).ToString () + " Turns", SendMessageOptions.DontRequireReceiver);
			}

			if(selfDestructParticles)
			{
				Instantiate (selfDestructParticles, transform.position, transform.rotation);
			}
		}
	}

	public void SelfDestruct()
	{
		//Self Destruct
		
		//Kill Self and damage all players
		Invoke ("KillSelf", 1f);

		
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			CombatManager.playerStats[i].SetDamage (combatAction.combatStats.stat, 0, (int)(((float)(100) + 1f) * combatAction.combatStats.stat.attack), 0f, 0f);
		}
	}

	void KillSelf()
	{
		combatAction.combatStats.EnemyDown ();
	}
}
