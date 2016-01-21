using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialOne : MonoBehaviour 
{
	public EnemyCombatCharacter combatStats;

	private int currentIndex = 0;

	private bool tutorialOneShot = false;
	private int currentTut;

	void Awake()
	{

	}
	
	// Use this for initialization
	void Start () 
	{
		TutorialUI.tutorialString = "Use the directional wheel at the bottom right to select your action.";
		TutorialUI.tutorialUI = true;
	}
	
	// Update is called once per frame
	public void UpdateTutorialUI () 
	{
		if(currentIndex == 0)
		{
			TutorialUI.tutorialString = "Use the directional wheel at the bottom right to select your action.";
			TutorialUI.tutorialUI = true;

			currentIndex ++;
		}

		if(currentIndex == 1)
		{
			if(!combatStats.affinityRevealed)
			{
				TutorialUI.tutorialString = "Use different elements to shatter the enemy's affinity";
				TutorialUI.tutorialUI = true;
				CombatManager.playerStats[0].RegenAP (true, 1f);

				ShowHelp (0);
			}
			else
			{
				currentIndex++;
			}
		}

		if(currentIndex == 2)
		{
			if(combatStats.affinityRevealed)
			{
				TutorialUI.tutorialString = "Affinity Shattered! Choose the correct element to ensure victory!";
				TutorialUI.tutorialUI = true;
				CombatManager.playerStats[0].RegenAP (true, 1f);
				if(!tutorialOneShot)
				{
					ShowHelp (1);
					tutorialOneShot = true;
				}
			}
		}
	}

	void ShowHelp(int _index)
	{
		currentTut = _index;
		Invoke ("Reveal", 1f);
	}

	void Reveal()
	{
		GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<CombatUIManager>().tutorialSlides[currentTut].SetActive (true);
	}
}
