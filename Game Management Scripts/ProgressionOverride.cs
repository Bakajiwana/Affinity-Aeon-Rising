using UnityEngine;
using System.Collections;

public class ProgressionOverride : MonoBehaviour 
{
	public int storyProgressionOverride;

	void Start()
	{
		if(SaveLoadManager.storyProgression < storyProgressionOverride)
		{
			SaveLoadManager.storyProgression = storyProgressionOverride;
		}
	}
}
