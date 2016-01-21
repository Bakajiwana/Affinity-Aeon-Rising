using UnityEngine;
using System.Collections;

//This script will recieve orders from the manager to activate action when its the characters turn

public class CombatActionActivator : MonoBehaviour 
{
	public Component[] actionComponents;


	public void Activate()
	{
		Invoke ("EnableTurn", 0.5f);
	}

	//This function is called to prevent that glitch that freezes the battle
	// This function is delayed when the player or enemy is activated because if it is the 
	// players turn again, then the battle will freeze because at the same frame the turn is ended the turn is 
	// activated, causing the battle to stop.
	void EnableTurn()
	{
		foreach(MonoBehaviour actionScripts in actionComponents)
		{
			actionScripts.enabled = true;
		}
	}

	public void DisableTurn()
	{
		foreach(MonoBehaviour actionScripts in actionComponents)
		{
			if(actionScripts.enabled == true)
			{
				gameObject.SendMessage ("EndTurn", SendMessageOptions.DontRequireReceiver);
			}
			actionScripts.enabled = false;
			//Cancel all invokes
			actionScripts.CancelInvoke ();
			CancelInvoke ();
		}
	}

	public bool IsActionScriptActive()
	{
		foreach(MonoBehaviour actionScripts in actionComponents)
		{
			if(actionScripts.enabled == true)
			{
				return true;
			}
		}

		return false;
	}
}
