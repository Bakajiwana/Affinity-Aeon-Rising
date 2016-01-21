using UnityEngine;
using System.Collections;

public class CreditTimer : MonoBehaviour 
{
	public float creditTimer = 40f;

	// Use this for initialization
	void Start () 
	{
		Invoke ("LoadMainMenu", creditTimer);
	}
	
	// Update is called once per frame
	void LoadMainMenu () 
	{
		Application.LoadLevel (1);
	}
}
