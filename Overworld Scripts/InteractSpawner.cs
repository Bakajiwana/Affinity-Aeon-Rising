using UnityEngine;
using System.Collections;

public class InteractSpawner : MonoBehaviour 
{
	private bool interacted = false;
	private bool interactible = false;

	public string controllerInputText = "Press A to Interact";
	public string keyboardInputText = "Press E to Interact";

	public Transform spawnObject;
	public Transform spawnObjectTo;

	private Transform currentObject;

	public bool stopWhenSpawned;

	public bool activateObjectsAfterSpawn = false;
	public bool activateObjectsOnInteract = false;
	private bool oneShotSpawnActivate = true;

	public GameObject[] revealObjects;
	public bool oneTime = false;
	private bool onlyTime = false;

	public bool noButtonInteraction = false;

	void Start()
	{
		activateObjectsAfterSpawn = false;
	}

	void Update()
	{
		if(interactible && !onlyTime)
		{
			if(Input.GetButtonUp ("Submit") || Input.GetButtonUp ("Earth") || Input.GetKeyUp (KeyCode.E) ||
			   noButtonInteraction)
			{
				if(!interacted)
				{
					if(spawnObjectTo)
					{
						currentObject = Instantiate (spawnObject, spawnObjectTo.position, spawnObjectTo.rotation) as Transform;
						currentObject.SetParent (spawnObjectTo, true);

						if(stopWhenSpawned)
						{
							CharacterManager.isBusy = true;
						}

						if(activateObjectsOnInteract)
						{
							ActivateObjects ();
						}
					}
					else
					{
						if(spawnObject)
						{
							currentObject = Instantiate (spawnObject, transform.position, transform.rotation) as Transform; 
						}

						if(stopWhenSpawned)
						{
							CharacterManager.isBusy = true;
						}

						if(activateObjectsOnInteract)
						{
							ActivateObjects ();
						}
					}
					interacted = true;
					CharacterManager.isBusy = false;
					if(oneShotSpawnActivate)
					{
						activateObjectsAfterSpawn = true;
						oneShotSpawnActivate = false;
					}
					AdventureInterface.helpText = "";
				}

				if(oneTime)
				{
					onlyTime = true;
				}
			}
		}



		if(activateObjectsAfterSpawn && !activateObjectsOnInteract)
		{
			if(!currentObject)
			{
				for(int i = 0; i < revealObjects.Length; i++)
				{
					revealObjects[i].SetActive (true);
				}
				activateObjectsAfterSpawn = false;
			}
		}
	}

	void ActivateObjects()
	{
		for(int i = 0; i < revealObjects.Length; i++)
		{
			revealObjects[i].SetActive (true);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag ("Player") && !onlyTime)
		{
			if(interacted == false || !currentObject)
			{
				if(!noButtonInteraction)
				{
					if(InputController.usingController)
					{
						AdventureInterface.helpText = controllerInputText;
					}
					else
					{
						AdventureInterface.helpText = keyboardInputText;
					}
				}
			}
			if(spawnObjectTo)
			{
				if(!spawnObjectTo.transform.name.Equals ("Dialogue Panel") ||
				   spawnObjectTo.transform.name.Equals ("Dialogue Panel") && spawnObjectTo.childCount <= 0)
				{

					interactible = true;
				}
			}
			else
			{
				interactible = true;
			}
		}

	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.CompareTag ("Player"))
		{
			interacted = false;
			interactible = false;
			AdventureInterface.helpText = "";
		}
	}
}
