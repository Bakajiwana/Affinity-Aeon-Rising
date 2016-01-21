using UnityEngine;
using System.Collections;

public class AirBotScript : MonoBehaviour 
{
	public EnemyCombatActions combatAction;

	public bool buffParty = false;
	[Range(0f,1f)]
	public float buffStrength = 0.5f;

	public Transform buffParticles;

	public void AttackBuff()
	{
		if(buffParty)
		{
			//Buff Every Enemy
			for(int i = 0; i < CombatManager.enemies.Count; i++)
			{
				CombatManager.enemyStats[i].stat.attack = (int)((float)CombatManager.enemyStats[i].stat.attackBase * 
				                                                (1 + buffStrength));
			}
		}
		else
		{
			//Buff self
			combatAction.combatStats.stat.attack = (int)((float)combatAction.combatStats.stat.attackBase * 
			                                             (1 + buffStrength));
		}

		//Show Text
		combatAction.combatStats.ShowDamageText ("Attack Up", Color.white, 0.8f);
		
		//Instantiate Effect
		Instantiate (buffParticles, transform.position, transform.rotation);
	}

	public void CameraShot()
	{
		CombatCamera cam = CombatCamera.control;
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
}
