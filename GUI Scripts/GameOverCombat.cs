using UnityEngine;
using System.Collections;

//Game Over Event at the end of Combat

public class GameOverCombat : MonoBehaviour
{
	public GameObject retryBattleButton;

	void Start()
	{
		//To determine whether boss fight using the Music Manager (Dirty cheat method)
		if(MusicManager.musicOverrideIndex >= 1)
		{
			if(retryBattleButton)
			{
				retryBattleButton.SetActive (true);
			}
		}
	}

	public void RetryButton()
	{
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("RetryBattle", gameObject.GetComponent<TravelManager>(), SendMessageOptions.DontRequireReceiver);
	}

	public void RetryBattle()
	{
		if(SaveLoadManager.saveHealth != null)
		{
			for(int i = 0; i < SaveLoadManager.saveHealth.Length; i++)
			{
				SaveLoadManager.saveHealth[i] = 20000;
				SaveLoadManager.saveAP[i] = 20000;
			}
		}
		
		Application.LoadLevel (Application.loadedLevel);
	}

	public void MainMenuButton()
	{
		Application.LoadLevel ("Main Menu");
	}
}
