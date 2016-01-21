using UnityEngine;
using System.Collections;

public class ProgressionObjectActivator : MonoBehaviour 
{
	public bool isManager = false;
	
	public GameObject[] objectComponent; 
	
	public int[] progressionPoint;
	
	public bool[] aheadOfStoryActive;
	public bool[] behindOfStoryActive;
	
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
		foreach(GameObject obj in objectComponent)
		{
			//If the mono script is ahead of story point
			if(SaveLoadManager.storyProgression >= progressionPoint[index])
			{
				if(obj)
				{
					obj.SetActive (aheadOfStoryActive[index]);
				}
			}
			
			//If the mono script is behind story point
			if(SaveLoadManager.storyProgression < progressionPoint[index])
			{
				if(obj)
				{
					obj.SetActive (behindOfStoryActive[index]);
				}
			}				                    
			
			index++;
		}
	}
}
