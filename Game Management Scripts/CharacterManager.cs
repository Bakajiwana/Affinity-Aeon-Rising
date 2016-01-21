using UnityEngine;
using System.Collections;

//Script Objective: This script will manage the characters in the game

public class CharacterManager : MonoBehaviour 
{
	//Character Variables
	private GameObject characterAeon;
	private GameObject characterIona;
	private GameObject characterTaven;
	private GameObject characterAiren;

	//Character Navigator Variables
	private GameObject aeonController;
	private GameObject ionaController;
	private GameObject tavenController;
	private GameObject airenController;

	private CharacterNavigator aeonNavigator;
	private CharacterNavigator ionaNavigator;
	private CharacterNavigator tavenNavigator;
	private CharacterNavigator airenNavigator;

	[HideInInspector]
	public static bool onLoaded = false; 

	private static Vector3 currentPosition;
	private static Vector3 currentRotation;

	public Transform startPosition;
	public bool inBase = false;

	//Busy Static Bool to be used during "Busy" moments to prevent certain interactions
	public static bool isBusy = false;

	//Combat Character Variables
	//0 - Aeon, 1 - Iona, 2- Taven, 3 - Airen
	public GameObject[] combatCharacters;
	public bool[] companions;

	// Use this for initialization
	void Start () 
	{
		SaveLoadManager.selectedCharacters[0] = true; //Must always be true

		isBusy = false;

		//Initialise Characters
		characterAeon = GameObject.Find("Main Characters/characterAeon01");
		characterIona = GameObject.Find("Main Characters/characterIona01");
		characterTaven = GameObject.Find ("Main Characters/characterTaven01");
		characterAiren = GameObject.Find ("Main Characters/characterAiren01");

		//print (characterIona.name);
	
		aeonController = characterAeon.transform.FindChild("Controller/Aeon's Navigator").gameObject;
		ionaController = characterIona.transform.FindChild("Controller/Iona's Navigator").gameObject;
		tavenController = characterTaven.transform.FindChild ("Controller/Taven's Navigator").gameObject;
		airenController = characterAiren.transform.FindChild ("Controller/Airen's Navigator").gameObject;

		if(aeonController)
		{
			aeonNavigator = aeonController.GetComponent<CharacterNavigator>();
		}

		if(ionaController)
		{
			ionaNavigator = ionaController.GetComponent<CharacterNavigator>();
		}

		if(tavenController)
		{
			tavenNavigator = tavenController.GetComponent<CharacterNavigator>();
		}

		if(airenController)
		{
			airenNavigator = airenController.GetComponent<CharacterNavigator>();
		}

		/*
		//Debugging: Check if characters are found and initialised
		print ("Iona = " + (bool)characterIona);
		print ("Taven = " + (bool)characterTaven);
		print ("Airen = " + (bool)characterAiren);

		print ("Iona's Controller = " + (bool)ionaController);
		print ("Taven's Controller = " + (bool)tavenController);
		print ("Airen's Controller = " + (bool)airenController);

		print ((bool)ionaNavigator);
		print ((bool)tavenNavigator);
		print ((bool)airenController);
		*/

		//Later on during story: This will be case switched depending on story level
		ActivateCharacters ();

		//If scene was called from a load
		if(!inBase)
		{
			if(onLoaded)
			{
				//Then Spawn at the start/ entrance of the level set by the startPosition Node
				if(startPosition)
				{
					LoadPlayerPosition (startPosition.position, startPosition.eulerAngles);
					//print ("Spawned at start position node");
				}
				else
				{
					LoadPlayerPosition (Vector3.zero);
				}
			}
			else
			{
				//If not loaded then 
				LoadPlayerPosition (currentPosition, currentRotation);
//				print ("Spawned at current position");
			}
		}
		else
		{
			//Reset Enemy Management
			EnemyManagement.enemyUpdate = true;
			//Heal all players 
			for(int i = 0; i < SaveLoadManager.saveHealth.Length; i++)
			{
				SaveLoadManager.saveHealth[i] = 100000;
				SaveLoadManager.saveAP[i] = 100000;
			}


			UpdatePlayerPosition (startPosition.position, startPosition.eulerAngles);
			LoadPlayerPosition (startPosition.position, startPosition.eulerAngles);
			UpdatePlayerPosition (Vector3.zero, Vector3.zero);
		}
	}
	
	public void UpdatePlayerPosition(Vector3 _location, Vector3 _rotation)
	{
		currentPosition = _location;
		currentRotation = _rotation;

//		print ("Updating current position on " + currentPosition);
	}

	void GetPlayerInformation(GameObject _object)
	{
		//Count Number of Players
		int numberOfPlayers = 1;
		int[] playerIndex = new int[combatCharacters.Length];
		playerIndex[0] = 0;

		for(int i = 0; i < companions.Length; i++)
		{
			if(companions[i] == true && i != 0)
			{
				playerIndex[numberOfPlayers] = i;
				numberOfPlayers++;
			}
		}

		//Add Players into arrays
		GameObject[] currentPlayers = new GameObject[numberOfPlayers];
		int[] currentHealths = new int[numberOfPlayers];

		for(int i = 0; i < currentPlayers.Length; i++)
		{
			if(i == 0)
			{
				currentPlayers[i] = combatCharacters[i];
				currentHealths[i] = playerIndex[i];
			}
			else
			{
				currentPlayers[i] = combatCharacters[playerIndex[i]];
				currentHealths[i] = playerIndex[i];
			}
		}

		//Send Players to Combat Information Node
		CombatInformation info = _object.GetComponent<CombatInformation>();
		info.SetPlayerInformation (currentPlayers, currentHealths);
	}

	public void ActivateCharacters()
	{
		if(SaveLoadManager.storyProgression >= 4)
		{
			if(!inBase)
			{
				companions = SaveLoadManager.selectedCharacters;
			}
			else
			{
				companions = new bool[4];
			}
		}

		companions[0] = true;

		int maxCharacters = 0;

		for(int i = 0; i < companions.Length; i++)
		{
			if(companions[i])
			{
				maxCharacters ++;
			}
		}

//		print (SaveLoadManager.selectedCharacters[0]);
//		print (SaveLoadManager.selectedCharacters[1]);
//		print (SaveLoadManager.selectedCharacters[2]);
//		print (SaveLoadManager.selectedCharacters[3]);

		characterAeon.SetActive (companions[0]);
		characterIona.SetActive (companions[1]);
		characterTaven.SetActive (companions[2]);
		characterAiren.SetActive (companions[3]);

		if(characterAeon.activeInHierarchy)
		{
			aeonNavigator.SetCharacter (maxCharacters,true);
		}

		if(characterIona.activeInHierarchy)
		{
			ionaNavigator.SetCharacter (maxCharacters, false);
		}

		if(characterTaven)
		{
			tavenNavigator.SetCharacter (maxCharacters, false);
		}

		if(characterAiren)
		{
			airenNavigator.SetCharacter (maxCharacters, false);
		}

		/*If Airen, Iona and Taven is active
		if(airenActive && ionaActive && tavenActive)
		{
			int maxCharacters = 3;

			characterAiren.SetActive (true);
			characterIona.SetActive (true);
			characterTaven.SetActive (true);

			airenNavigator.SetCharacter (0, maxCharacters, true);
			ionaNavigator.SetCharacter (1, maxCharacters, false);
			tavenNavigator.SetCharacter (2, maxCharacters, false);
		}
		//If Airen and Iona is active
		if(airenActive && ionaActive && !tavenActive)
		{
			int maxCharacters = 2;

			characterAiren.SetActive (true);
			characterIona.SetActive (true);
			characterTaven.SetActive (false);
			
			airenNavigator.SetCharacter (0, maxCharacters, true);
			ionaNavigator.SetCharacter (1, maxCharacters, false);
		}
		//If Airen and Taven is active
		if(airenActive && !ionaActive && tavenActive)
		{
			int maxCharacters = 2;
			
			characterAiren.SetActive (true);
			characterIona.SetActive (false);
			characterTaven.SetActive (true);
			
			airenNavigator.SetCharacter (0, maxCharacters, true);
			tavenNavigator.SetCharacter (1, maxCharacters, false);
		}
		//If Iona and Taven is active
		if(!airenActive && ionaActive && tavenActive)
		{
			int maxCharacters = 2;
			
			characterAiren.SetActive (false);
			characterIona.SetActive (true);
			characterTaven.SetActive (true);

			ionaNavigator.SetCharacter (0, maxCharacters, true);
			tavenNavigator.SetCharacter (1, maxCharacters, false);
		}
		//If Airen is active only
		if(airenActive && !ionaActive && !tavenActive)
		{
			int maxCharacters = 1;
			
			characterAiren.SetActive (true);
			characterIona.SetActive (false);
			characterTaven.SetActive (false);
			
			airenNavigator.SetCharacter (0, maxCharacters, true);
		}
		//If Iona is Active only
		if(!airenActive && ionaActive && !tavenActive)
		{
			int maxCharacters = 1;
			
			characterAiren.SetActive (false);
			characterIona.SetActive (true);
			characterTaven.SetActive (false);

			ionaNavigator.SetCharacter (0, maxCharacters, true);
		}
		//If Taven is Active only
		if(!airenActive && !ionaActive && tavenActive)
		{
			int maxCharacters = 1;
			
			characterAiren.SetActive (false);
			characterIona.SetActive (false);
			characterTaven.SetActive (true);

			tavenNavigator.SetCharacter (0, maxCharacters, true);
		}*/
	}

	public void LoadPlayerPosition(Vector3 _location)
	{
		characterAeon.SetActive (false);
		characterAiren.SetActive (false);
		characterIona.SetActive (false);
		characterTaven.SetActive (false);
		
		//Update characters position
		characterAeon.transform.position = _location;
		characterAiren.transform.position = _location;
		characterIona.transform.position = _location;
		characterTaven.transform.position = _location;

		//Later on during story: This will be case switched depending on story level
		ActivateCharacters ();
	}

	public void LoadPlayerPosition(Vector3 _location, Vector3 _rotation)
	{
		characterAeon.SetActive (false);
		characterAiren.SetActive (false);
		characterIona.SetActive (false);
		characterTaven.SetActive (false);

		//Update characters position
		characterAeon.transform.position = _location;
		characterAiren.transform.position = _location;
		characterIona.transform.position = _location;
		characterTaven.transform.position = _location;

		characterAeon.transform.eulerAngles = _rotation;
		characterAiren.transform.eulerAngles = _rotation;
		characterIona.transform.eulerAngles = _rotation;
		characterTaven.transform.eulerAngles = _rotation;

		//Later on during story: This will be case switched depending on story level
		ActivateCharacters ();
	}

	//Save the players position during save
	public void SavePlayerPosition()
	{
		if(aeonController.gameObject.activeInHierarchy)
		{
			SaveLoadManager.savePosition = aeonController.transform.position;
		}
		if(airenController.gameObject.activeInHierarchy)
		{
			SaveLoadManager.savePosition = airenController.transform.position;
		}
		else if(ionaController.gameObject.activeInHierarchy)
		{
			SaveLoadManager.savePosition = ionaController.transform.position;
		}
		else if (tavenController.gameObject.activeInHierarchy)
		{
			SaveLoadManager.savePosition = tavenController.transform.position;
		}
	}
}
