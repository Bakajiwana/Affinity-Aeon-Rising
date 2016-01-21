using UnityEngine;
using System.Collections;

public class EnableDestination : MonoBehaviour 
{
	public TravelManager travel;

	void OnEnable()
	{
		travel.InitiateTravel(travel.destination, travel.levelName);
	}
}
