using UnityEngine;
using System.Collections;

//Script Objective: Communicate with the Hover Manager 

public class HoverInfoActivate : MonoBehaviour 
{
	private HoverInfoManager hoverInfo;

	public string hoverText;
	public int hoverPivot;

	// Use this for initialization
	void Start () 
	{
		hoverInfo = GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<HoverInfoManager>();
	}
	
	public void ActivateHoverInfo()
	{
		//Set Text
		hoverInfo.SetText (hoverText);

		//Activate Hover 
		hoverInfo.HoverActivate ();

		//Set Pivot
		hoverInfo.SetPivot (hoverPivot);

		//Set Current Position
		hoverInfo.SetCurrentPosition (transform);
	}

	public void DeactivateHoverInfo()
	{
		hoverInfo.HoverDeactivate();
	}
}
