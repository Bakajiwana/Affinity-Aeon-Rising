using UnityEngine;
using System.Collections;

public class ClawBotScript : MonoBehaviour 
{
	private EnemyCombatActions combatAction;

	public int chargeMaxProgress = 2;
	[HideInInspector]
	public int chargeProgress = 0;

	void Start()
	{
		combatAction = transform.parent.gameObject.GetComponent<EnemyCombatActions>();
	}


	public void Charging()
	{
		//Increment Charge
		chargeProgress++;

		//Set Global Message
		GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetGlobalMessage", "Charging" + " " + chargeProgress.ToString () + 
		                                                            " / " + chargeMaxProgress.ToString (), SendMessageOptions.DontRequireReceiver);

		//End Turn
		combatAction.EndTurnDelay (2f);

		if(chargeProgress == chargeMaxProgress)
		{
			chargeProgress = 0;
		}
	}

	public void CameraAction(int _index)
	{
		switch(_index)
		{
		case 0:
			break;
		}
	}

	void ViewThePlayers()
	{
		CombatCamera.control.SpawnGlobalAnimation (null, "View Players");
	}
}
