using UnityEngine;
using System.Collections;

public class TeleportSwitch : MonoBehaviour 
{
	public int areaTeleport;

	public GameObject[] destinations;

	public GameObject teleporterActivate;

	void OnEnable()
	{
		for(int i = 0; i < destinations.Length; i++)
		{
			destinations[i].SetActive (false);
		}

		teleporterActivate.SetActive (true);

		destinations[areaTeleport].SetActive (true);

		gameObject.SetActive (false);
	}
}
