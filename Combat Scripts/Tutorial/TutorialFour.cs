using UnityEngine;
using System.Collections;

public class TutorialFour : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		TutorialUI.tutorialString = "Some enemies have high stats that have a statistical advantage over you. Use curse to debilitate.";
		TutorialUI.tutorialUI = true;

		Invoke ("EndTutorial", 30f);
	}

	void EndTutorial()
	{
		TutorialUI.tutorialString = "";
		TutorialUI.tutorialUI = false;
	}
}
