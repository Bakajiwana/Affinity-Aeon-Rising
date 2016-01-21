using UnityEngine;
using System.Collections;

public class SprintPrompt : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown (KeyCode.LeftShift) || Input.GetAxis("Triggers") < -0.2f)
		{
			gameObject.SetActive (false);
		}
	}
}
