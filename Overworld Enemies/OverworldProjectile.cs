using UnityEngine;
using System.Collections;

public class OverworldProjectile : MonoBehaviour 
{
	[HideInInspector]
	public bool hitPlayer = false;

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag ("Player"))
		{
			hitPlayer = true;
		}
	}
}
