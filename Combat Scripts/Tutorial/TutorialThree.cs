using UnityEngine;
using System.Collections;

public class TutorialThree : MonoBehaviour 
{
	public EnemyCombatCharacter combatStats;
	private int currentTut;
	
	void Awake()
	{
		
	}
	
	// Use this for initialization
	void Start () 
	{
		TutorialUI.tutorialString = "You require Health to live and AP to continue fighting. Use Support to buff or replenish";
		TutorialUI.tutorialUI = true;

		ShowHelp (4);
	}
	
	// Update is called once per frame
	public void UpdateTutorialUI () 
	{

	}
	
	void ShowHelp(int _index)
	{
		currentTut = _index;
		Invoke ("Reveal", 5f);
	}
	
	void Reveal()
	{
		GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<CombatUIManager>().tutorialSlides[currentTut].SetActive (true);
	}
}
