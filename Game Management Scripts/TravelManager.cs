using UnityEngine;
using System.Collections;

//Script Objective: Used to transport player across levels and fast travel.

public class TravelManager : MonoBehaviour 
{
	public GameObject levelTraveller;
	public string destination;
	public string levelName;
	private bool readyToGo = false;

	public bool isBaseTeleporter = false;
	public Transform partySelect;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(readyToGo && Input.GetKeyUp (KeyCode.E) || readyToGo && Input.GetButtonUp ("Earth")
		   || readyToGo && Input.GetButtonUp ("Submit"))
		{
			if(SaveLoadManager.storyProgression < 4 || !isBaseTeleporter)
			{
				InitiateTravel (destination, levelName);
			}
			else
			{
				//Turn on Party Select
				partySelect.gameObject.SetActive(true);
				partySelect.gameObject.GetComponent<PartySelectionScript>().SetCurrentTeleport(this);
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
		}
	}

	public void InitiateTravel(string _destination, string _levelName)
	{
		GameObject levelTravel;
		levelTravel = Instantiate (levelTraveller.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
		FastTravel travel = levelTravel.GetComponent<FastTravel>();
		travel.SetDestination (_levelName, _destination);
		Application.LoadLevel ("Loading Scene");
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag ("Player"))
		{
			readyToGo = true;
			if(!InputController.usingController)
			{
				AdventureInterface.helpText = "Press E to go to " + destination;
			}
			else
			{
				AdventureInterface.helpText = "Press A to go to " + destination;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.CompareTag ("Player"))
		{
			readyToGo = false;
			AdventureInterface.helpText = "";
		}
	}
}
