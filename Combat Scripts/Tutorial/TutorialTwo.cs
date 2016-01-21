using UnityEngine;
using System.Collections;

public class TutorialTwo : MonoBehaviour 
{
	public EnemyCombatCharacter combatStats;
	public ClawBotScript claw;

	private int currentTut;
	private bool oneShot = true;

	private bool charging = false;
	
	// Use this for initialization
	void Start () 
	{
		//TutorialUI.tutorialString = "This Enemy is a hard hitter. Keeping your defense up will allow you to heal more than it can damage you.";
		//TutorialUI.tutorialUI = true;

		Invoke ("ShowTip" , 3f);
	}

	void Update()
	{
		if(claw.chargeProgress > 0 && oneShot)
		{
			charging = true;
		}

		if(oneShot && charging)
		{
			if(claw.chargeProgress == 0)
			{
				ShowHelp(5);
				oneShot = false;			
			}
		}
	}
	
	public void UpdateTutorialUI () 
	{
		
	}

	void ShowTip()
	{
		TutorialUI.tutorialString = "Tip: Use Support - Earth to buff your defense. Note: support buffs do not stack";
		TutorialUI.tutorialUI = true;
	}
	
	void ShowHelp(int _index)
	{
		currentTut = _index;
		Invoke ("Reveal", 3.5f);
	}
	
	void Reveal()
	{
		GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<CombatUIManager>().tutorialSlides[currentTut].SetActive (true);
	}
}
