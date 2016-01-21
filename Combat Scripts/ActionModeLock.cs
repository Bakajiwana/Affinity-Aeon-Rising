using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Lock Off Action Modes depending on level

public class ActionModeLock : MonoBehaviour 
{
	//1 - Attack, 2 - Defend, 3 - Support, 4 - Curse
	//5 - Earth, 6 - Fire, 7 - Lightning, 8 - Water
	public int levelUnlock;
	
	void Update () 
	{
		if(CombatUIManager.mainPlayerScript)
		{
			//Get Main Player Level
			int level = CombatUIManager.mainPlayerScript.combatStats.character.level;

			//Get the Button Component
			Button actionButton = gameObject.GetComponent<Button>();

			if(level >= levelUnlock)
			{
				actionButton.interactable = true;
			}
			else
			{
				actionButton.interactable = false;
			}

			this.enabled = false;
		}
	}
}
