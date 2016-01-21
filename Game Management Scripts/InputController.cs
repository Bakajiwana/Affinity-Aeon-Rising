using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//Determines controls, Place in a Manager or Event System

public class InputController : MonoBehaviour 
{
	//Using Controller or Mouse
	public static bool usingController = false;

	// Update is called once per frame
	void Update () 
	{
		//Identify if using Controller or Mouse
		float mouseDetection = Mathf.Abs (Input.GetAxis ("Mouse X") + Input.GetAxis ("Mouse Y"));
		//print (mouseDetection);
		float controllerDetection = Mathf.Abs (Input.GetAxis("ControllerDetection") + 
		                                       Input.GetAxis ("RightHorizontal") +
		                                       Input.GetAxis ("RightVertical"));
		//print (controllerDetection);

		if(controllerDetection > 0.1f || Input.GetButtonDown ("Fire") || Input.GetButtonDown ("Water") ||
		   Input.GetButtonDown ("Earth") || Input.GetButtonDown ("Lightning") || 
		   Input.GetButtonDown ("Switch"))
		{
			usingController = true;
		}
		else if(mouseDetection > 0.1f)
		{
			usingController = false;
		}

		if(!usingController)
		{
			EventSystem.current.sendNavigationEvents = false;
			EventSystem.current.SetSelectedGameObject (null);
		}
		else
		{
			EventSystem.current.sendNavigationEvents = true;
		}

		//print (usingController);
	}
}
