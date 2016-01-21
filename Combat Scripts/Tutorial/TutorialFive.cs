using UnityEngine;
using System.Collections;

public class TutorialFive : MonoBehaviour
{

	public EnemyCombatCharacter combatStats;
	
	private int currentIndex = 0;
	
	// Update is called once per frame
	public void UpdateTutorialUI () 
	{
		currentIndex++;

		if(currentIndex >= 10)
		{
			if(CombatManager.playerStats[0].stat.attack < CombatManager.playerStats[0].stat.attackBase)
			{
				TutorialUI.tutorialString = "Your attacks are innefective. Your attack stat has been weakened by Exzalia's Corruption. Use Support - Fire to increase your attack.";
				TutorialUI.tutorialUI = true;
			}
			else
			{
				TutorialUI.tutorialString = "";
				TutorialUI.tutorialUI = false;
			}
		}
	}
}
