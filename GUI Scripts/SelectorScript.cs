using UnityEngine;
using System.Collections;

public class SelectorScript : MonoBehaviour 
{
	private CapsuleCollider col; 

	void Start()
	{
		col = gameObject.GetComponent<CapsuleCollider>();
		ColliderSwitch (false);
	}

	void ColliderSwitch(bool _switch)
	{
		if(!_switch)
		{
			//Move to the center of the map
			transform.position = Vector3.zero;

			Invoke ("TurnOffDelay", 0.5f);
		}
		else
		{
			Invoke ("TurnOnDelay", 0.5f);
		}
	}

	void TurnOffDelay()
	{
		col.enabled = false;
	}

	void TurnOnDelay()
	{
		col.enabled = true;
	}
}
