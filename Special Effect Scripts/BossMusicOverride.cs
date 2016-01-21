using UnityEngine;
using System.Collections;

public class BossMusicOverride : MonoBehaviour 
{
	public int bossIndex = 1;

	// Use this for initialization
	void Awake () 
	{
		MusicManager.musicOverride = true;
		MusicManager.musicOverrideIndex = bossIndex;
		this.enabled = false;
	}
}
