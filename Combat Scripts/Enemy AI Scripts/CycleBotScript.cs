using UnityEngine;
using System.Collections;

public class CycleBotScript : MonoBehaviour 
{
	public EnemyCombatActions combatAction;

	[Range(0f,1f)]
	public float healthRegenDetection = 0.70f;

	public bool debuffParty = false;
	[Range(0f,1f)]
	public float debuffStrength = 0.5f;
	
	public Transform debuffParticles;

	[Range(0f,1f)]
	public float healthRestore = 0.4f;
	public Transform healthRestoreParticles;
	
	public void DebuffDefence()
	{
		if(debuffParty)
		{
			//debuff Every Enemy
			for(int i = 0; i < CombatManager.players.Count; i++)
			{
				CombatManager.playerStats[i].SetCondemned (1);
				//Set Global Message
				GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetGlobalMessage", "Condemn", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			//debuff self
			CombatManager.playerStats[combatAction.targetIndex].SetCondemned (1);
			//Set Global Message
			GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetGlobalMessage", "Condemn", SendMessageOptions.DontRequireReceiver);
		}
		
		//Instantiate Effect
		if(debuffParticles)
		{
			Instantiate (debuffParticles, transform.position, transform.rotation);
		}
	}

	public void DecideAction()
	{
		//If HP over 70% - Do Basic Attack
		if(!combatAction.combatStats.affinityRevealed)
		{
			if(combatAction.combatStats.stat.health >= (combatAction.combatStats.stat.healthBase * healthRegenDetection))
			{
				combatAction.attackNumber = 1;
				combatAction.anim.SetInteger ("Attack Number", 1);

				//Set Global Message
				GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetGlobalMessage", "Tri Beam", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				//If HP under 70% 

				//Go into Defend Mode
				combatAction.anim.SetTrigger ("Defending");
				combatAction.EndTurnDelay (0.5f);

				//Set Global Message
				GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetGlobalMessage", "Defend Mode", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			combatAction.attackNumber = 1;
			combatAction.anim.SetInteger ("Attack Number", 1);

			//Set Global Message
			GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetGlobalMessage", "Tri Beam", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void Regenerate()
	{
		//Then Next Turn Restore HP
		combatAction.combatStats.SetHeal (true, healthRestore);
		if(healthRestoreParticles)
		{
			Instantiate (healthRestoreParticles, transform.position, transform.rotation);
		}

		//Set Global Message
		GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetGlobalMessage", "Heal", SendMessageOptions.DontRequireReceiver);
	}

	public void CameraShot(int _index)
	{
		CombatCamera cam = CombatCamera.control;

		if(_index == 0)
		{
			cam.SpawnGlobalAnimation (null, "View Enemies");
		}

		if(_index == 1)
		{
			//Call Camera
			cam.CameraReset ();
			cam.ScreenEffect (0);
			cam.SetTransform (CombatManager.players[combatAction.targetIndex].transform.position);
			cam.SetRotateTowards (gameObject);
			cam.SetDistance (1.5f);
			//cam.DelayStop (1.2f);
			cam.SetPosition (1);
			cam.Truck (1);
			cam.SetMoveSpeed (0.5f);
			cam.DelayStop (1f);
			//cam.SetMoveSpeed (5f);
			//cam.increaseMoveSpeed = true;
			//cam.Zoom (-1);
		}

		if(_index == 2)
		{
			cam.SpawnGlobalAnimation (null, "View Players");
		}
	}
}
