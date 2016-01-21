using UnityEngine; 
using System.Collections; 
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text; 
using UnityEngine.UI;
using UnityEngine.EventSystems;


//Script Objective: Save and Load. Managers and Objects will obtain information from this script.
//Source: Using the code from http://wiki.unity3d.com/index.php/Save_and_Load_from_XML. Tweaked to my preference

public class SaveLoadManager : MonoBehaviour 
{
	private string _FileLocation;
	private string _FileName; 
	UserData myData; 
	string _data; 
	private int saveNumber;	
	
	//Static variables
	public static Vector3 savePosition; 	//Posiion of player
	public static string saveTime;			//Time
	public static string saveDate;			//Date
	public static float savePlayTime;		//Play Time
	public static string saveLevelName;		//Location Name
	public static int latestSave;
	public static bool latestQuicksave;
	public static bool latestAutosave;

	public static Character saveStats;
	public static int saveHealthMax;
	public static int saveAPMax;
	public static int[] saveShield;
	public static int[] saveHealth;
	public static int[] saveAP;

	public static int storyProgression;
	public static int battlesCompleted;

	public static bool tutorialAPLow;
	public static bool tutorialParadox;

	public static bool[] selectedCharacters;
	
	//Independant Variables
	public long saveLatestTime;				//Latest Time
	
	private CharacterManager characterManager;
	
	//This Object can be a button too
	public Text buttonFileName;
	public Text buttonLocation;
	public Text buttonPlayTime;
	public Text buttonDate;
	public Text buttonTime;
	public Text buttonLatest;
	
	public bool isLoadButton;
	public bool isSaveButton;
	public bool isContinueButton;
	
	public Transform loadGameTravel;
	
	//Hovering
	private bool isHover;
	
	//Quick Save and Autosave booleans
	public bool isQuicksave;
	public bool isAutosave;
	
	void Awake()
	{
		// Where we want to save and load to and from 
		_FileLocation= Application.dataPath; 
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
		
		/* This Fails to work on build
		string filePath = Application.dataPath + System.IO.Path.GetDirectoryName ("Save Games");

		if(!Directory.Exists (filePath))
		{
			//print ("It Is Here");
			//AssetDatabase.CreateFolder ("Assets", "Save Games");
			Directory.CreateDirectory (filePath);
		}
		*/
		
		// we need something to store the information into 
		myData = new UserData(); 
		//saveStats = new Character();
	}
	
	// When the EGO is instansiated the Start will trigger 
	// so we setup our initial values for our local members 
	void Start () 
	{ 		
		if(GameObject.FindGameObjectWithTag("Adventure Manager"))
		{
			characterManager = GameObject.FindGameObjectWithTag("Adventure Manager").GetComponent<CharacterManager>();
		}

		//Debug logs for team health
//		print ("Aeon's Health is " + saveHealth[0]);
//		print ("Iona's Health is " + saveHealth[1]);
//		print ("Taven's Health is " + saveHealth[2]);
//		print ("Airen's Health is " + saveHealth[3]);

		if(saveShield == null)
		{
			saveShield = new int[4];
			for(int i = 0; i < saveShield.Length; i++)
			{
				saveShield[i] = 100;
			}
		}

		if(saveHealth == null)
		{
			saveHealth = new int[4];
			for(int i = 0; i < saveHealth.Length; i++)
			{
				saveHealth[i] = 20000;
//				print ("Resetting health");
			}
		}

		if(saveAP == null)
		{
			saveAP = new int[4];
			for(int i = 0; i < saveAP.Length; i++)
			{
				saveAP[i] = 20000;
//				print ("Resetting AP");
			}
		}

		//Save Character Stats
		if(saveStats == null)
		{
			saveStats = new Character();
			SaveData ();
	//		print ("Calling new thing here with the level of " + saveStats.level);
		}

		if(selectedCharacters == null)
		{
			selectedCharacters = new bool[4];
		}

		//Debug logs for team health
//		print ("Aeon's Health is " + saveHealth[0]);
//		print ("Iona's Health is " + saveHealth[1]);
//		print ("Taven's Health is " + saveHealth[2]);
//		print ("Airen's Health is " + saveHealth[3]);
//
//		print ("Aeon's AP is " + saveAP[0]);
//		print ("Iona's AP is " + saveAP[1]);
//		print ("Taven's AP is " + saveAP[2]);
//		print ("Airen's AP is " + saveAP[3]);

//		print (saveStats.level);

		saveHealthMax = (int)((float)279 * Mathf.Sqrt (saveStats.level));
		saveAPMax = (int)((float)22 * Mathf.Sqrt (saveStats.level));
	} 
	
	void Update()
	{
		if(isHover && Input.GetKeyDown (KeyCode.Delete) && !isQuicksave && !isAutosave)
		{
			DeleteSave ();
		}
		
		//Quick Save
		if(Input.GetKeyDown (KeyCode.F5) && !Pause.isPaused && !CharacterManager.isBusy && !GameOver.isGameOver)
		{
			Quicksave ();
		}
		
		//Quick Load
		if(Input.GetKeyDown (KeyCode.F9) && !Pause.isPaused && !CharacterManager.isBusy && !GameOver.isGameOver)
		{
			GameObject loadTravel;
			loadTravel = Instantiate (loadGameTravel.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
			LoadGameTravel loadGame = loadTravel.GetComponent<LoadGameTravel>();
			Quickload ();
			loadGame.SetSaveNumber (saveNumber, saveLevelName);
			loadGame.quickload = true;
			DontDestroyOnLoad (loadTravel);
			Application.LoadLevel ("Loading Scene");
		}

		//Story Progression Test
		if(Application.isEditor)
		{
			if(Input.GetKeyDown (KeyCode.L))
			{
				storyProgression++;
				print ("Story Progressed to Chapter " + storyProgression);
			}
		}
	}
	
	//This function is used to 
	public void LoadButtonUpdate(int _saveFileNumber)
	{
		saveNumber = _saveFileNumber;
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
		// Load our UserData into myData 
		if(isQuicksave)
		{
			Quickload ();
		}
		else if (isAutosave)
		{
			Autoload  ();
		}
		else
		{
			LoadXML(); 
		}
		if(_data.ToString() != "") 
		{ 
			// notice how I use a reference to type (UserData) here, you need this 
			// so that the returned object is converted into the correct type 
			myData = (UserData)DeserializeObject(_data); 
			
			//Name of Save File
			if(isAutosave)
			{
				buttonFileName.text = "Autosave";
			}
			else if(isQuicksave)
			{
				buttonFileName.text = "Quicksave";
			}
			else
			{
				buttonFileName.text = "Save Game "+saveNumber.ToString ();
			}
			
			//Load the Time
			buttonTime.text = "Time: " + myData._iUser.time;
			
			
			//Load the Date
			buttonDate.text = "Date: " + myData._iUser.date;
			
			//Load the Play Time
			//Display time
			float min = (myData._iUser.playTime/60f);
			float sec = (myData._iUser.playTime % 60f);
			float hour = min/60f;
			buttonPlayTime.text = "Hours Played: "+string.Format("{0:00}:{1:00}:{2:00}",hour,min,sec);
			//buttonPlayTime.text = "Hours Played: " + myData._iUser.playTime.ToString ();
			
			
			//Load Level Name
			buttonLocation.text = myData._iUser.levelName;
			
			saveLatestTime = myData._iUser.latestTime;
			
			// just a way to show that we loaded in ok 
			//Debug.Log(myData._iUser.x); 
		}
	}
	
	//This function marks this Save as the latest
	public void MarkLatest()
	{
		buttonLatest.gameObject.SetActive (true);
		EventSystem.current.SetSelectedGameObject (gameObject);
	}
	
	public void SaveLoadButtonClick()
	{
		_FileLocation= Application.dataPath; 
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
		
		DirectoryInfo  di = new DirectoryInfo (_FileLocation);		
		int numXML = di.GetFiles("*.xml", SearchOption.TopDirectoryOnly).Length;
		
		if(numXML > 0)
		{
			if(isLoadButton)
			{
				GameObject loadTravel;
				loadTravel = Instantiate (loadGameTravel.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
				LoadGameTravel loadGame = loadTravel.GetComponent<LoadGameTravel>();
				if(isAutosave)
				{
					Autoload ();
					loadGame.SetSaveNumber (saveNumber, saveLevelName);
					loadGame.autoload = true;
				}
				else if(isQuicksave)
				{
					Quickload ();
					loadGame.SetSaveNumber (saveNumber, saveLevelName);
					loadGame.quickload = true;
				}
				else
				{
					Load (saveNumber);
					loadGame.SetSaveNumber (saveNumber, saveLevelName);
				}
				DontDestroyOnLoad (loadTravel);
				Application.LoadLevel ("Loading Scene");
			}
			if(isSaveButton)
			{
				Save (saveNumber);
				LoadButtonUpdate(saveNumber);
				latestSave = saveNumber;
				print (saveLatestTime);
				GameObject.FindGameObjectWithTag("Adventure Manager").SendMessage ("PauseSwitch");
				//Debug.Log("Saved to Game Save " +saveNumber);
			}
			
			if(isContinueButton)
			{
				GameObject loadTravel;
				loadTravel = Instantiate (loadGameTravel.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
				LoadGameTravel loadGame = loadTravel.GetComponent<LoadGameTravel>();
				if(latestQuicksave)
				{
					Quickload ();
					loadGame.quickload = true;
				}
				else if (latestAutosave)
				{
					Autoload ();
					loadGame.autoload = true;
				}
				else
				{
					Load (latestSave);
				}
				loadGame.SetSaveNumber (latestSave, saveLevelName);
				DontDestroyOnLoad (loadTravel);
				Application.LoadLevel ("Loading Scene");
			}
		}
	}
	
	/* The following metods came from the referenced URL */ 
	string UTF8ByteArrayToString(byte[] characters) 
	{      
		UTF8Encoding encoding = new UTF8Encoding(); 
		string constructedString = encoding.GetString(characters); 
		return (constructedString); 
	} 
	
	byte[] StringToUTF8ByteArray(string pXmlString) 
	{ 
		UTF8Encoding encoding = new UTF8Encoding(); 
		byte[] byteArray = encoding.GetBytes(pXmlString); 
		return byteArray; 
	} 
	
	// Here we serialize our UserData object of myData 
	string SerializeObject(object pObject) 
	{ 
		string XmlizedString = null; 
		MemoryStream memoryStream = new MemoryStream(); 
		XmlSerializer xs = new XmlSerializer(typeof(UserData)); 
		XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
		xs.Serialize(xmlTextWriter, pObject); 
		memoryStream = (MemoryStream)xmlTextWriter.BaseStream; 
		XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray()); 
		return XmlizedString; 
	} 
	
	// Here we deserialize it back into its original form 
	object DeserializeObject(string pXmlizedString) 
	{ 
		XmlSerializer xs = new XmlSerializer(typeof(UserData)); 
		MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString)); 
		//XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
		return xs.Deserialize(memoryStream); 
	} 
	
	// Finally our save and load methods for the file itself 
	void CreateXML() 
	{ 
		StreamWriter writer; 
		FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName); 
		if(!t.Exists) 
		{ 
			writer = t.CreateText(); 
		} 
		else 
		{ 
			t.Delete(); 
			writer = t.CreateText(); 
		} 
		writer.Write(_data); 
		writer.Close(); 
//		Debug.Log("File written."); 
	} 
	
	void CreateNewXML() 
	{ 
		StreamWriter writer; 
		/*
		DirectoryInfo  di = new DirectoryInfo (_FileLocation);

		int numXML = di.GetFiles("*.xml", SearchOption.AllDirectories).Length;

		saveNumber = numXML + 1;
		*/
		saveNumber = 1;
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
		
		FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName);
		while(t.Exists)
		{
			saveNumber++;
			_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
			t = new FileInfo(_FileLocation+"\\"+ _FileName);
		}
		
		writer = t.CreateText(); 
		
		writer.Write(_data); 
		writer.Close(); 
		
		Debug.Log("File written."); 
	} 
	
	void LoadXML() 
	{ 
		_FileLocation= Application.dataPath; 
		FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName); 
		DirectoryInfo  f = new DirectoryInfo (_FileLocation);		
		int numXML = f.GetFiles("*.xml", SearchOption.AllDirectories).Length;
		int currentSave = saveNumber;
		int search = 0;
		while (!t.Exists)	//If File does not exist, because it was deleted search next files
		{
			search++;
			saveNumber ++;
			_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
			t = new FileInfo(_FileLocation+"\\"+ _FileName); 
			
			if(search > numXML)
			{
				saveNumber = currentSave - 1;
				_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
				t = new FileInfo(_FileLocation+"\\"+ _FileName); 
				
				for(int i = currentSave; i > 0 && !t.Exists; i--)
				{
					saveNumber --;
					_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
					t = new FileInfo(_FileLocation+"\\"+ _FileName); 
				}
				
				if(!t.Exists)
				{
					Debug.Log ("Failed to find load file.... shit");
					break;
				}
			}
		}
		
		if(t.Exists)
		{
			StreamReader r = File.OpenText(_FileLocation+"\\"+_FileName);
			string _info = r.ReadToEnd(); 
			r.Close(); 
			_data=_info; 
			//Debug.Log("File Read"); 
		}
		else
		{
			_data = "";
		}
	} 
	
	void QuickLoadXML() 
	{ 
		_FileLocation= Application.dataPath; 
		FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName); 	
		
		if(t.Exists)
		{
			StreamReader r = File.OpenText(_FileLocation+"\\"+_FileName);
			string _info = r.ReadToEnd(); 
			r.Close(); 
			_data=_info; 
			//Debug.Log("File Read"); 
		}
		else
		{
			_data = "";
		}
	} 
	
	public void SetHover(bool _hover)
	{
		isHover = _hover;
	}
	
	public void DeleteSave()
	{
		_FileLocation= Application.dataPath; 
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
		FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName); 
		DirectoryInfo  f = new DirectoryInfo (_FileLocation);		
		int numXML = f.GetFiles("*.xml", SearchOption.AllDirectories).Length;
		int currentSave = saveNumber;
		int search = 0;
		while (!t.Exists)	//If File does not exist, because it was deleted search next files
		{
			search++;
			saveNumber ++;
			_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
			t = new FileInfo(_FileLocation+"\\"+ _FileName); 
			
			if(search > numXML)
			{
				saveNumber = currentSave - 1;
				_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
				t = new FileInfo(_FileLocation+"\\"+ _FileName); 
				
				for(int i = currentSave; i > 0 && !t.Exists; i--)
				{
					saveNumber --;
					_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
					t = new FileInfo(_FileLocation+"\\"+ _FileName); 
				}
				
				if(!t.Exists)
				{
					saveNumber=currentSave;
					Debug.Log ("Failed to find Save File " +saveNumber+ ".... shit");
					break;
				}
			}
		}
		
		//Delete File
		if(t.Exists)
		{
			//Also delete Meta file
			FileInfo m = new FileInfo(_FileLocation+"\\SaveGame"+saveNumber.ToString()+".xml.meta"); 
			t.Delete(); 
			m.Delete ();
			Destroy (gameObject);
		}
	}

	public void NewGameLoad()
	{	
		myData = new UserData();

		LoadData ();			
	}

	//Save Function called from save button
	public void Save(int _saveFileNumber)
	{
		//Show Save Icon
		AdventureInterface.showSaveIcon = true;
		
		saveNumber = _saveFileNumber;
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 

		characterManager.SavePlayerPosition ();
		
		SaveData ();
		
		// Time to creat our XML! 
		_data = SerializeObject(myData); 
		// This is the final resulting XML from the serialization process 
		CreateXML(); 
		//Debug.Log(_data); 
	}
	
	//This function creates a new save
	public void NewSave()
	{
		//Show Save Icon
		AdventureInterface.showSaveIcon = true;

		characterManager.SavePlayerPosition ();
		
		SaveData ();
		
		// Time to creat our XML! 
		_data = SerializeObject(myData); 
		// This is the final resulting XML from the serialization process 	
		CreateNewXML ();
		
		//Debug.Log(_data); 
	}

	//Autosave function
	public void Quicksave()
	{
		//Show Save Icon
		AdventureInterface.showSaveIcon = true;

		characterManager.SavePlayerPosition ();
		
		_FileName="Quicksave.xml"; 
		
		SaveData ();
		
		// Time to creat our XML! 
		_data = SerializeObject(myData); 
		// This is the final resulting XML from the serialization process 
		CreateXML(); 
		//Debug.Log(_data); 
	}

	
	//Autosave function
	public void Autosave()
	{
		//Show Save Icon
		AdventureInterface.showSaveIcon = true;
		
		_FileName="Autosave.xml"; 
		
		SaveData ();
		
		// Time to creat our XML! 
		_data = SerializeObject(myData); 
		// This is the final resulting XML from the serialization process 
		CreateXML(); 
		//Debug.Log(_data); 
	}	

	//Load Function called from start of scene to Initialise level/ game
	public void Load(int _saveFileNumber)
	{
		saveNumber = _saveFileNumber;
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
		// Load our UserData into myData 
		LoadXML(); 
		if(_data.ToString() != "") 
		{ 
			// notice how I use a reference to type (UserData) here, you need this 
			// so that the returned object is converted into the correct type 
			myData = (UserData)DeserializeObject(_data); 
			
			LoadData ();
		} 
	}
	
	//Load Function called from start of scene to Initialise level/ game
	public void Quickload()
	{
		_FileName="Quicksave.xml"; 
		// Load our UserData into myData 
		QuickLoadXML(); 
		if(_data.ToString() != "") 
		{ 
			// notice how I use a reference to type (UserData) here, you need this 
			// so that the returned object is converted into the correct type 
			myData = (UserData)DeserializeObject(_data); 
			
			LoadData ();
		} 
	}

	//Load Function called from start of scene to Initialise level/ game
	public void Autoload()
	{
		_FileName="Autosave.xml"; 
		// Load our UserData into myData 
		QuickLoadXML(); 
		if(_data.ToString() != "") 
		{ 
			// notice how I use a reference to type (UserData) here, you need this 
			// so that the returned object is converted into the correct type 
			myData = (UserData)DeserializeObject(_data); 
			
			LoadData ();
		} 
	}

	void LoadData()
	{
		//Load the save position
		savePosition = new Vector3(myData._iUser.x,myData._iUser.y,myData._iUser.z);
		
		//Load the Time
		saveTime = myData._iUser.time;
		
		//Load the Date
		saveDate = myData._iUser.date;
		
		//Load the Play Time
		savePlayTime = myData._iUser.playTime;
		
		//Load Level Name
		saveLevelName = myData._iUser.levelName;
		
		//Load Latest Time
		saveLatestTime = myData._iUser.latestTime;
		
		//Load Character Stat
		saveStats = new Character(myData._iUser.newName,
		                          myData._iUser.newLevel,
		                          myData._iUser.newShieldAffinity,
		                          myData._iUser.newShield,
		                          myData._iUser.newHealth,
		                          myData._iUser.newLevelExp,	           
		                          myData._iUser.fire, 
		                          myData._iUser.fireExp,
		                          myData._iUser.water,
		                          myData._iUser.waterExp, 
		                          myData._iUser.lightning, 
		                          myData._iUser.lightningExp, 
		                          myData._iUser.earth,
		                          myData._iUser.earthExp);

		saveShield = new int[4];
		saveShield[0] = myData._iUser.shieldAeon;
		saveShield[1] = myData._iUser.shieldIona;
		saveShield[2] = myData._iUser.shieldTaven;
		saveShield[3] = myData._iUser.shieldAiren;

		saveHealth = new int[4];
		saveHealth[0] = myData._iUser.healthAeon;
		saveHealth[1] = myData._iUser.healthIona;
		saveHealth[2] = myData._iUser.healthTaven;
		saveHealth[3] = myData._iUser.healthAiren;

		saveAP = new int[4];
		saveAP[0] = myData._iUser.apAeon;
		saveAP[1] = myData._iUser.apIona;
		saveAP[2] = myData._iUser.apAeon;
		saveAP[3] = myData._iUser.apAiren;
//		print ("set healths with save data");
		

		//Load Progression Variables
		storyProgression = myData._iUser.progression;
		battlesCompleted = myData._iUser.battles;

		//Update the Enemy Management
		EnemyManagement.enemyUpdate = true;
		//Update Character Management
		CharacterManager.onLoaded = true;

		selectedCharacters = new bool[4];	//0 - Aeon, 2 - Iona, 3 - Taven, 4 - Airen
		selectedCharacters[0] = myData._iUser.activeAeon;
		selectedCharacters[1] = myData._iUser.activeIona;
		selectedCharacters[2] = myData._iUser.activeTaven;
		selectedCharacters[3] = myData._iUser.activeAiren;

		//Tutorial
		tutorialAPLow = myData._iUser.tutAPLow;
		tutorialParadox = myData._iUser.tutParadox;

		
		// just a way to show that we loaded in ok 
		//Debug.Log(myData._iUser.x); 
		//Debug.Log (saveStats.level);
		//Debug.Log (saveHealth[0]);
		//Debug.Log (storyProgression);
	}

	void SaveData()
	{
		//Save Position
		myData._iUser.x = savePosition.x; 
		myData._iUser.y = savePosition.y; 
		myData._iUser.z = savePosition.z;  
		
		//Time
		saveTime = System.DateTime.Now.ToString("HH:mm:ss tt");
		myData._iUser.time = saveTime; 
//		print (saveTime);
		
		//Date
		saveDate = System.DateTime.Now.ToString("dd/MM/yyyy");
		myData._iUser.date = saveDate;
//		print (saveDate);
		
		//Play Time
		savePlayTime += Time.realtimeSinceStartup;
		myData._iUser.playTime = savePlayTime; 
		
		//Level Name
		saveLevelName = Application.loadedLevelName;
		myData._iUser.levelName = saveLevelName; 
		
		//Latest Time
		saveLatestTime = long.Parse (System.DateTime.Now.ToString ("yyyyMMddHHmmss"));
		myData._iUser.latestTime = saveLatestTime; 

		//Character Stats
		myData._iUser.newName = saveStats.name;
		myData._iUser.newLevel = saveStats.level;
		myData._iUser.newShieldAffinity = saveStats.currentShieldAffinity;
		myData._iUser.newShield = saveStats.currentShield;
		myData._iUser.newHealth = saveStats.currentHealth;
		myData._iUser.newLevelExp = saveStats.levelExperience;	           
		myData._iUser.fire = saveStats.fireAffinity;
		myData._iUser.fireExp = saveStats.fireExperience;
		myData._iUser.water = saveStats.waterAffinity;
		myData._iUser.waterExp = saveStats.waterExperience; 
		myData._iUser.lightning = saveStats.lightningAffinity;
		myData._iUser.lightningExp = saveStats.lightningExperience; 
		myData._iUser.earth = saveStats.earthAffinity;
		myData._iUser.earthExp = saveStats.earthExperience;

		myData._iUser.shieldAeon = saveShield[0];
		myData._iUser.shieldIona = saveShield[1];
		myData._iUser.shieldTaven = saveShield[2];
		myData._iUser.shieldAiren = saveShield[3];

		myData._iUser.healthAeon = saveHealth[0];
		myData._iUser.healthIona = saveHealth[1];
		myData._iUser.healthTaven = saveHealth[2];
		myData._iUser.healthAiren = saveHealth[3];

		myData._iUser.apAeon = saveAP[0];
		myData._iUser.apIona = saveAP[1];
		myData._iUser.apTaven = saveAP[2];
		myData._iUser.apAiren = saveAP[3];

		//Progression
		myData._iUser.progression = storyProgression;
		myData._iUser.battles = battlesCompleted;

		if(selectedCharacters != null)
		{
			myData._iUser.activeAeon = selectedCharacters[0];
			myData._iUser.activeIona = selectedCharacters[1];
			myData._iUser.activeTaven = selectedCharacters[2];
			myData._iUser.activeAiren = selectedCharacters[3];
		}

		//Tutorial
		myData._iUser.tutAPLow = tutorialAPLow;
		myData._iUser.tutParadox = tutorialParadox;
	}
} 

// UserData is our custom class that holds our defined objects we want to store in XML format 
public class UserData 
{ 
	// We have to define a default instance of the structure 
	public DemoData _iUser; 
	// Default constructor doesn't really do anything at the moment 
	public UserData() { } 
	
	// Anything we want to store in the XML file, we define it here 
	public struct DemoData 
	{ 
		//Overworld Stats
		public float x; 
		public float y; 
		public float z; 
		public string time;
		public string date;
		public float playTime;
		public string levelName;
		public long latestTime;

		//Combat Stats
		public string newName;
		public int newLevel;
		public int newShieldAffinity;
		public int newShield;
		public int newHealth;
		public long newLevelExp;	           
		public int fire; 
		public long fireExp;
		public int water; 
		public long waterExp; 
		public int lightning; 
		public long lightningExp; 
		public int earth; 
		public long earthExp;

		public int shieldAeon;
		public int shieldIona;
		public int shieldTaven;
		public int shieldAiren;

		public int healthAeon;
		public int healthIona;
		public int healthTaven;
		public int healthAiren;

		public int apAeon;
		public int apIona;
		public int apTaven;
		public int apAiren;

		public int progression;
		public int battles;

		public bool activeAeon;
		public bool activeIona;
		public bool activeTaven;
		public bool activeAiren;

		//Tutorials
		public bool tutAPLow;
		public bool tutParadox;
	} 
}
