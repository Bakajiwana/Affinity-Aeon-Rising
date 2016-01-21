using UnityEngine;
using System.Collections;

public class SerplexScript : MonoBehaviour 
{
	private EnemyCombatActions combatAction;

	public Transform shedSkinEffect;

	void Start()
	{
		combatAction = transform.parent.gameObject.GetComponent<EnemyCombatActions>();
	}

	public void DecideAction()
	{
		//If Inflicted with 2 or more debuffs
		if(CountDebuff() >= 2)
		{
			//Activate this animation 
			combatAction.attackNumber = 3;
			combatAction.anim.SetInteger ("Attack Number", 3);

			GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetGlobalMessage", "Shed Skin", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			combatAction.attackNumber = 2;
			combatAction.anim.SetInteger ("Attack Number", 2);

			GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetGlobalMessage", "Boa Strike", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void ShedSkin()
	{
		//Remove all debuff
		combatAction.combatStats.stat.attack = combatAction.combatStats.stat.attackBase;			
		combatAction.combatStats.stat.defence = combatAction.combatStats.stat.defenceBase;			
		combatAction.combatStats.stat.agility = combatAction.combatStats.stat.agilityBase;			
		combatAction.combatStats.stat.luck = combatAction.combatStats.stat.luckBase;			
		combatAction.combatStats.stat.accuracy = combatAction.combatStats.stat.accuracyBase;			
		combatAction.combatStats.stat.speed = combatAction.combatStats.stat.speedBase;	
		
		if(shedSkinEffect)
		{
			Instantiate (shedSkinEffect, transform.position, transform.rotation);
		}

		combatAction.combatStats.ShowDamageText ("Stat Restored", Color.white, 1f);
	}

	int CountDebuff()
	{
		int count = 0;

		if(combatAction.combatStats.stat.attack < combatAction.combatStats.stat.attackBase)
		{
			count++;
		}
		
		if(combatAction.combatStats.stat.defence < combatAction.combatStats.stat.defenceBase)
		{
			count++;
		}
		
		if(combatAction.combatStats.stat.agility < combatAction.combatStats.stat.agilityBase)
		{
			count++;
		}
		
		if(combatAction.combatStats.stat.luck < combatAction.combatStats.stat.luckBase)
		{
			count++;
		}
		
		if(combatAction.combatStats.stat.accuracy < combatAction.combatStats.stat.accuracyBase)
		{
			count++;
		}
		
		if(combatAction.combatStats.stat.speed < combatAction.combatStats.stat.speedBase)
		{
			count++;
		}

		return count;
	}

	public void CameraShot()
	{
		CombatCamera cam = CombatCamera.control;

		//Call Camera
		cam.CameraReset ();
		cam.ScreenEffect (0);
		cam.SetTransform (CombatManager.players[combatAction.targetIndex].transform.position);
		cam.SetRotateTowards (gameObject);
		cam.SetDistance (1.2f);
		cam.DelayStop (1.2f);
		cam.SetPosition (10);
		cam.SetMoveSpeed (5f);
		cam.increaseMoveSpeed = true;
		cam.Zoom (-1);
	}
}
