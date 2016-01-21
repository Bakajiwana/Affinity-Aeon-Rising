using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour 
{
	public static bool tutorialUI;
	
	public static string tutorialString;
	public Text tutorialText;
	
	public UIFade fadeScript;
	
	// Update is called once per frame
	void Update ()
	{
		if(tutorialUI)
		{
			tutorialText.text = tutorialString;
			fadeScript.FadeTrigger (true);
			tutorialUI = false;
		}
	}
}
