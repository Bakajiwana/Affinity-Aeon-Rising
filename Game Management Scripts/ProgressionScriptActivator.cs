using UnityEngine;
using System.Collections;

//Script Objective: Set the necessary scripts active depending on Story Progression Chapter.

public class ProgressionScriptActivator : MonoBehaviour 
{
	public bool isManager = false;

	public Component[] monoScripts; 

	public int[] progressionPoint;

	public bool[] aheadOfStoryActive;
	public bool[] behindOfStoryActive;
	public bool[] OnlyTimeActive;

	// Use this for initialization
	void Awake ()
	{
		if(isManager)
		{
			//Start coroutine for performance
			StartCoroutine (ProgressionUpdate ());
		}
		else
		{
			UpdateObjects ();
			this.enabled = false;
		}
	}
	
	IEnumerator ProgressionUpdate()
	{
		while(true) //loop forever
		{
			UpdateObjects ();

			//Delay
			yield return new WaitForSeconds(5f);
		}
	}

	void UpdateObjects()
	{
		//Index variable 
		int index = 0;
		
		//For each Monobehaviour script
		foreach(MonoBehaviour scriptObject in monoScripts)
		{
			//If the mono script is ahead of story point
			if(progressionPoint[index] >= SaveLoadManager.storyProgression)
			{
				if(scriptObject)
				{
					scriptObject.enabled = aheadOfStoryActive[index];
				}
			}
			
			//If the mono script is behind story point
			if(progressionPoint[index] < SaveLoadManager.storyProgression)
			{
				if(scriptObject)
				{
					scriptObject.enabled = behindOfStoryActive[index];
				}
			}

			if(progressionPoint[index] == SaveLoadManager.storyProgression)
			{
				if(OnlyTimeActive[index])
				{
					scriptObject.enabled = true;
				}
			}
			
			index++;
		}
	}
}
