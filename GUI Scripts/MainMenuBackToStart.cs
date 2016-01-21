using UnityEngine;
using System.Collections;

public class MainMenuBackToStart : MonoBehaviour 
{	
	public float maxIdleTime = 60f;
	private float currentTime;

	void Start()
	{
		currentTime = maxIdleTime;
	}

	// Update is called once per frame
	void Update () 
	{
		if(Input.GetAxis ("Horizontal") > 0.15f || Input.GetAxis ("Horizontal") < -0.15f || 
		   Input.GetAxis ("Vertical") > 0.15f || Input.GetAxis ("Vertical") < -0.15f ||
		   Input.GetButton ("Submit") ||Input.GetButton ("Cancel") || 
		   Input.GetAxis ("Mouse X") > 0.15f || Input.GetAxis ("Mouse Y") < -0.15f)
		{
			print ("Button Pressed");
			currentTime = maxIdleTime;
		}
		else
		{
			currentTime -= Time.deltaTime;

			if(currentTime <= 0f)
			{
				Application.LoadLevel ("Start Scene");
			}
		}
	}
}
