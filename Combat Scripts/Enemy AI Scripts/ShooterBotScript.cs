using UnityEngine;
using System.Collections;

//Script for the Shooter Bot Enemy

public class ShooterBotScript : MonoBehaviour 
{
	private EnemyCombatActions combatAction;

	public int APStealPoints = 20;

	void Start()
	{
		combatAction = transform.parent.gameObject.GetComponent<EnemyCombatActions>();
	}

	public void APDrain()
	{
		//Drain AP
		CombatManager.playerStats[combatAction.targetIndex].APCost (APStealPoints, 0);
		//Show Text
		CombatManager.playerStats[combatAction.targetIndex].ShowDamageText ("AP Drained", Color.white, 0.7f);
		CombatManager.playerStats[combatAction.targetIndex].ShowDamageText (APStealPoints.ToString (), 
		                                                                    Color.white, 0.7f);

		if(CombatManager.playerStats[combatAction.targetIndex].stat.actionPoints <= 0)
		{
			CombatManager.playerStats[combatAction.targetIndex].stat.actionPoints = 0;
		}
	}

	public void APStealDelay(float _time)
	{
		Invoke ("APDrain", _time);
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
