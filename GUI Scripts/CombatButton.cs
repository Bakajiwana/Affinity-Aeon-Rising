using UnityEngine;
using System.Collections;

//Script Objective: This script will control the UI Combat Buttons

public class CombatButton : MonoBehaviour 
{
	public int actionMode = 0;

	public void ActionModeActivate()
	{
		GameObject.FindGameObjectWithTag ("Game Manager").SendMessage ("ActivateNextPlayer", actionMode, SendMessageOptions.DontRequireReceiver);
	}
}
