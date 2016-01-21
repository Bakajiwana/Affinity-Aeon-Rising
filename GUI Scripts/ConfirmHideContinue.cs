using UnityEngine;
using System.Collections;

public class ConfirmHideContinue : MonoBehaviour 
{
	public Transform optionalReveal;

	void Start()
	{
		Time.timeScale = 0.0001f;
	}

	// Update is called once per frame
	void Update () 
	{
		Time.timeScale = 0.0001f;
		if(Input.GetMouseButtonDown (0) || Input.GetButtonDown ("Earth") || Input.GetButtonDown ("Submit"))
		{
			gameObject.SetActive (false);
			Time.timeScale = 1f;
			if(optionalReveal)
			{
				optionalReveal.gameObject.SetActive (true);
			}
		}
	}
}
