using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour 
{
	public Transform[] musicNodes;
	private Transform currentMusic;

	public bool activateRandomMusicOnStart = false;
	public int activateStartMusic = 0;
	private int currentIndex;

	public static bool musicOverride = false;
	public static int musicOverrideIndex = 1;

	// Use this for initialization
	void Start () 
	{
		Invoke ("StartMusic", 1.5f);

		currentIndex = activateStartMusic;
	}

	void Update()
	{
		/*
		if(Input.GetButtonDown ("Pause"))
		{
			Invoke ("StartMusic", 1.5f);
		}
		*/
	}

	void StartMusic()
	{
		if(musicOverride)
		{
			ActivateMusic (musicOverrideIndex);
			musicOverride = false;
		}
		else
		{
			if(activateRandomMusicOnStart)
			{
				ActivateRandomMusic ();
			}
			else
			{
				ActivateMusic (activateStartMusic);
			}
		}
	}


	public void ActivateRandomMusic()
	{
		if(currentMusic)
		{
			Destroy (currentMusic.gameObject);
		}

		currentMusic = Instantiate (musicNodes[Random.Range (0, musicNodes.Length - 1)], transform.position,
		                            transform.rotation) as Transform;
		currentMusic.gameObject.SetActive (false);
		Invoke ("ResetPlay", 0.5f);
	}

	public void ActivateMusic(int _index)
	{
		if(currentMusic)
		{
			Destroy (currentMusic.gameObject);
		}

		if(_index > 0)
		{
			currentMusic = Instantiate (musicNodes[_index], transform.position,
			                            transform.rotation) as Transform;
			currentMusic.gameObject.SetActive (false);
			Invoke ("ResetPlay", 0.5f);
		}
		else
		{
			currentMusic = Instantiate (musicNodes[0], transform.position,
			                            transform.rotation) as Transform;
			currentMusic.gameObject.SetActive (false);
			Invoke ("ResetPlay", 0.5f);
		}
	}

	//This Procedure is called because of some Sound Glitch where audio sources stop at the very start of the game
	void ResetPlay()
	{
		if(currentMusic)
		{
			currentMusic.gameObject.SetActive (true);
		}
	}

	public void NextMusic()
	{
		currentIndex++;

		if(currentIndex >= musicNodes.Length)
		{
			currentIndex = 0;
		}

		if(currentIndex < 0)
		{
			currentIndex = musicNodes.Length - 1;
		}

		ActivateMusic (currentIndex);
	}

	public void BackMusic()
	{
		currentIndex--;
		
		if(currentIndex >= musicNodes.Length)
		{
			currentIndex = 0;
		}
		
		if(currentIndex < 0)
		{
			currentIndex = musicNodes.Length - 1;
		}
		
		ActivateMusic (currentIndex);
	}
}
