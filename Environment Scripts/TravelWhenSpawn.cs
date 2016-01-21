using UnityEngine;
using System.Collections;

public class TravelWhenSpawn : MonoBehaviour 
{
	public string levelToLoad;

	void OnEnable()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		Application.LoadLevel (levelToLoad);
	}
}
