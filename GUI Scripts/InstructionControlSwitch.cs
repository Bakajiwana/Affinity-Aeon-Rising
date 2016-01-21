using UnityEngine;
using System.Collections;

public class InstructionControlSwitch : MonoBehaviour 
{
	public Transform controllerImage;
	public Transform pcImage;
	
	// Update is called once per frame
	void Update () 
	{
		if(InputController.usingController)
		{
			if(controllerImage)
			{
				controllerImage.gameObject.SetActive (true);
			}

			if(pcImage)
			{
				pcImage.gameObject.SetActive (false);
			}
		}
		else
		{
			if(controllerImage)
			{
				controllerImage.gameObject.SetActive (false);
			}

			if(pcImage)
			{
				pcImage.gameObject.SetActive(true);
			}
		}
	}
}
