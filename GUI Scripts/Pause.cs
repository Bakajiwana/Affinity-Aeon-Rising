using UnityEngine;
using System.Collections;

//Script Objective: To pause the game and bring up the pause menu

public class Pause : MonoBehaviour 
{
	//Create a public static pause variable to be used by many objects 
	public static bool isPaused = false; 

	public Transform pauseScreen;

	// Use this for initialization
	void Start () 
	{
		//The game should not be paused on start up
		isPaused = false;
		Time.timeScale = 1f;
		pauseScreen.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If the Cancel or escape button is pressed
		if(Input.GetButtonDown ("Pause") && !GameOver.isGameOver)
		{
			//if the game is not paused, then pause the game
			if(!isPaused)
			{
				isPaused = true;
				pauseScreen.gameObject.SetActive (true);
				gameObject.SendMessage ("HideCursor", false, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				//but if it is paused then unpause the game
				Time.timeScale = 1f;
				isPaused = false;
				pauseScreen.gameObject.SetActive (false);
				gameObject.SendMessage ("HideCursor", true, SendMessageOptions.DontRequireReceiver);
				gameObject.SendMessage ("UpdateEnvironmentQuality", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void PauseSwitch()
	{
		//if the game is not paused, then pause the game
		if(!isPaused)
		{
			isPaused = true;
			pauseScreen.gameObject.SetActive (true);
			gameObject.SendMessage ("HideCursor", false, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			//but if it is paused then unpause the game
			Time.timeScale = 1f;
			isPaused = false;
			pauseScreen.gameObject.SetActive (false);
			gameObject.SendMessage ("HideCursor", true, SendMessageOptions.DontRequireReceiver);
		}
	}
}
