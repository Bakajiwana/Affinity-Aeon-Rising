using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//This script is going to update the UI screen

public class CombatUIManager : MonoBehaviour 
{
	//Player Party Layout
	public Transform partyLayoutPanel;
	public Transform partyMemberPanel;

	//Action Description
	public CanvasGroup actionDescription;

	//Current Player UI
	public Image currAPBar;
	public static float currentPlayerAP;
	public Image initialAPBar;
	public static float initialPlayerAP;
	public Text currentAPText;
	public Text costAP;

	private PartyMemberStatus[] memberStatus;

	//Level Bars
	public Transform gaugeOrganiser;
	public HorizontalLayoutGroup gaugeOrganiserGroup;
	public HorizontalLayoutGroup gaugeSliderGroup;
	public Transform levelBar;

	[HideInInspector]
	public Transform[] currentBars;
	private LevelBar[] bar;

	//Charge Slider
	public Transform chargeSliderManager;
	public Transform chargeSliderHolder;
	[HideInInspector]
	public Transform chargeSliderObject;
	[HideInInspector]
	public Slider chargeSlider;
	[HideInInspector]
	public float[] chargeCriticalZone = new float[3];

	//Canvas Combat UI fades
	private bool combatScreenFadeIn = false;
	public float fade = 0.5f;

	//When its the first players turn the alphas will become active or not depending on turn
	private float activeAlpha = 0f;
	private bool activeLayoutFadeIn = false;
	private bool activeLayoutFadeOut = false;

	//UI End Screens
	public Transform endScreenNode;
	public Transform winScreen;
	public Transform gameOverScreen;

	public CanvasGroup combatScreen;

	//Special Screen
	public CanvasGroup specialScreen;
	public Transform overlayScreen;
	public Transform[] teamParadoxFrames;
	public Transform paradoxDeathScreenEffect;
	public Transform paradoxLifeScreenEffect;
	private bool specialParadox = false;

	//Scan Values
	[HideInInspector]
	public int[] scanValues;

	//Scan Button Press
	public static bool scanMode = false;

	public ScanScreen scanScreen;

	public Transform objectHidden;

	//Main Player Script
	public static PlayerCombatActions mainPlayerScript;
	
	//Instruction Text Update
	public Text[] infoElementAP;

	//Main Character Codex Variables
	public Transform codexCharacterPanel;
	public Transform codexObjectPanel;

	[HideInInspector]
	public bool codexExitButton = false;

	public Text codexName;
	public Text codexLevel;
	public Transform[] codexMainBackdrop;
	public Transform[] codexShieldBackdrop;
	public Transform codexAvatar; 
	private Transform currentCodexAvatar;
	public Transform[] codexStatusImmunity;
	public Transform[] codexStatusImmunityDash;

	public Transform[] codexShieldEarthInfo;
	public Transform[] codexShieldFireInfo;
	public Transform[] codexShieldLightningInfo;
	public Transform[] codexShieldWaterInfo;

	public Transform[] codexMainEarthInfo;
	public Transform[] codexMainFireInfo;
	public Transform[] codexMainLightningInfo;
	public Transform[] codexMainWaterInfo; 

	public Text[] codexStatTexts; //0 - Attack, 1 - Defence, 2- Agility, 3 - Luck, 4 - Accuracy, 5 - Speed

	public Transform codexNotes;
	public Transform codexHidden;
	private Transform codexCurrentNotes;

	public Transform[] codexStatPointerUp;
	public Transform[] codexStatPointerDown;

	//Object Codex Variables
	public Text codexObjectName;
	public Text[] codexObjectRevealed;
	public Transform[] codexObjectHidden;

	//Object Variables
	private int[] codexObjectReactant;	//0 - Desired, 1 - Inapplicable, 2 - Undesired, 3 - Unknown
	private string[] codexObjectNotes;

	//Character Variables 
	private CodexCharacter codex;

	private bool codexPageReady = false;

	[HideInInspector]
	public bool codexMode = false;

	//Pause Variables
	public static bool combatPause = false; 
	public Transform pauseScreen;

	//Turn of Action Variables
	public Transform nodeOrder;
	public Transform avatarOrder;

	public Transform turnNode;

	private Transform[] turnOrder = new Transform[0];
	private Transform[] turnAvatar = new Transform[0];

	public CanvasGroup timeOfAction;
	private bool timeOfActionFadeIn = false;

	//Combat Intro
	public Animator actionIntroPanel;
	public CanvasGroup actionIntroCanvas;

	//Global Message Variables
	public Text globalMessage;
	public CanvasGroup globalMessageCanvas;
	public Animator globalMessageAnim;
	private float globalMessageTimer = 0f;
	public float globalMessageMaxTimer = 1f;

	//Sequencer UI
	public SequencerScript sequenceUI;

	//Instruction UI
	public UIFade confirmationPanel;

	public GameObject[] effectsUI;
	public GameObject[] levelUpUIEffects;

	public GameObject[] tutorialSlides;

	public GameObject skipPrompt;

	public AudioSource confirmSound;
	public AudioSource backSound;
	public AudioSource nextSound;
	public AudioSource hoverSound;

	public AudioClip nextAudioClip;
	public AudioClip backAudioClip;
	public AudioClip hoverAudioClip;
	public AudioClip confirmAudioClip;

	public Transform explosionScreenEffect;

	void Start ()
	{
		//Make sure combatScreen alpha is off
		combatScreen.alpha = 0f;
		activeAlpha = 0f;

		scanMode = false;
		codexMode = false;

		//Initialise Pause
		Time.timeScale = 1f;
		combatPause = false;
		pauseScreen.gameObject.SetActive (false);

		activeLayoutFadeIn = false;
		activeLayoutFadeOut = true;

		//EventSystem.current.gameObject.GetComponent<BaseInputModule>().enabled = false;

		//Lock and hide cursor
		if(!Application.isEditor)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//currHealthBar.fillAmount = currentPlayerHealth;
		//currShieldBar.fillAmount = currentPlayerShield;
		if(currAPBar)
		{
			currAPBar.fillAmount = currentPlayerAP;
		}
		//currHealthText.text = currentPlayerHealthText;
		//currAPText.text = currentPlayerAPText;

		//Combat Screen Fade In start of battle
		if(combatScreenFadeIn)
		{
			combatScreen.alpha += Time.deltaTime * fade;
			
			if(combatScreen.alpha >= 1f)
			{
				combatScreenFadeIn = false;
			}
		}		
		
		if(activeLayoutFadeIn)
		{
			//activeLayoutFadeOut = false; //Overwrite fade out

			activeAlpha += Time.deltaTime * fade;

			//Affected Alphas
			timeOfAction.alpha = 1f;

			
			if(activeAlpha >= 1f)
			{
				activeLayoutFadeIn = false;
			}
		}
		
		if(activeLayoutFadeOut)
		{
			activeAlpha -= Time.deltaTime * fade;

			//Affected Alphas

			if(!timeOfActionFadeIn)
			{
				timeOfAction.alpha = activeAlpha;
			}
			
			if(activeAlpha <= 0f)
			{
				activeLayoutFadeOut = false;
			}
		}

		if(timeOfActionFadeIn)
		{			
			timeOfAction.alpha += Time.deltaTime * fade;			
			
			if(timeOfAction.alpha >= 1f)
			{
				timeOfActionFadeIn = false;
			}
		}

		if(specialParadox && CombatManager.combatState == 0)
		{
			if(specialScreen.gameObject.activeInHierarchy)
			{
				if(Input.GetMouseButtonDown (0) || Input.GetButtonDown ("LeftButton"))
				{
					specialParadox = false;

					combatScreen.alpha = 1f;

					specialScreen.alpha = 0f;

					GameObject.FindGameObjectWithTag("Combat Manager").SendMessage ("LifeParadox", SendMessageOptions.DontRequireReceiver);
				}
				else if (Input.GetMouseButtonDown (1) || Input.GetButtonDown ("RightButton"))
				{
					specialParadox = false;

					combatScreen.alpha = 1f;
					
					specialScreen.alpha = 0f;

					GameObject.FindGameObjectWithTag("Combat Manager").SendMessage ("DeathParadox", SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		//Pause Function 
		//If no Special Paradox, scan mode and codex mode and not game over
		if(!specialParadox && !scanMode && !codexMode && CombatManager.combatState == 0)
		{
			if(Input.GetButtonDown ("Pause"))
			{
				//Pause the game
				PauseToggle ();
			}
		}

		//Global Message Fading
		if(globalMessageCanvas)
		{
			if(globalMessageTimer > 0f)
			{
				//Count down
				globalMessageTimer -= Time.deltaTime;

				if(globalMessageCanvas.alpha < 1f)
				{
					globalMessageCanvas.alpha += Time.deltaTime * fade;
				}
			}
			else
			{
				if(globalMessageCanvas.alpha > 0f)
				{
					globalMessageCanvas.alpha -= Time.deltaTime * fade;
				}
			}
		}
	}

	//This function is called to Hide the Actions Screens and Confirm attack
	public void ReadyForConfirmation()
	{
		actionIntroPanel.SetTrigger ("Confirm");
	}

	public void StartSequencer(string _text)
	{
		sequenceUI.ClearSequence ();
		sequenceUI.CreateSequence (_text);
	}

	public void PauseToggle()
	{
		combatPause = !combatPause;

		if(combatPause)
		{
			//Pause Game
			Time.timeScale = 0.0001f;

			//Reveal Pause Screen
			pauseScreen.gameObject.SetActive (true);

			//Hide Combat Screen 
			combatScreen.alpha = 0f;

			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;

			EventSystem.current.gameObject.GetComponent<BaseInputModule>().enabled = true;
		}
		else
		{
			//Unpause Game
			Time.timeScale = 1f;

			//Hide Pause Screen
			pauseScreen.gameObject.SetActive (false);
			
			//Reveal Combat Screen 
			combatScreen.alpha = 1f;

			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			EventSystem.current.gameObject.GetComponent<BaseInputModule>().enabled = false;
		}
	}

	void InitialisePartyPanel()
	{
		memberStatus = new PartyMemberStatus[CombatManager.players.Count];
		
		for(int i = 0; i < CombatManager.players.Count; i++)
		{
			Transform panel = Instantiate (partyMemberPanel, partyLayoutPanel.position, partyLayoutPanel.rotation) as Transform;
			panel.SetParent(partyLayoutPanel, true);
			
			memberStatus[i] = panel.gameObject.GetComponent<PartyMemberStatus>();

			//Initialise the UI and player together <3

			CombatManager.playerStats[i].SetPartyUI (memberStatus[i]);

			memberStatus[i].InitialiseStats();

			//The combat screen should appear
			combatScreenFadeIn = true;
		}

		//Find the main Player
		for(int i = 0; i < CombatManager.playerStats.Count; i++)
		{
			if(CombatManager.playerStats[i].isMain == true)
			{
				mainPlayerScript = CombatManager.players[i].GetComponent<PlayerCombatActions>();
				UpdateActionInfo();
				break;
			}
		}

		initialPlayerAP = (float)mainPlayerScript.combatStats.stat.actionPoints / (float)mainPlayerScript.combatStats.stat.actionPointMax;
		UpdateAPBar ();

		//print ("Success");
		//print (CombatManager.playerStats.Count);
	}

	public void UpdateAPBar()
	{
		initialAPBar.fillAmount = initialPlayerAP;
		initialPlayerAP = (float)mainPlayerScript.combatStats.stat.actionPoints /(float)mainPlayerScript.combatStats.stat.actionPointMax;
		currentAPText.text = "AP: " + mainPlayerScript.combatStats.stat.actionPoints.ToString() + "/" + mainPlayerScript.combatStats.stat.actionPointMax.ToString ();

		initialAPBar.color = Color.gray;
	}

	public void UpdateAPBar(int _element)
	{
		initialAPBar.fillAmount = initialPlayerAP;
		initialPlayerAP = (float)mainPlayerScript.combatStats.stat.actionPoints /(float)mainPlayerScript.combatStats.stat.actionPointMax;
		currentAPText.text = "AP: " + mainPlayerScript.combatStats.stat.actionPoints.ToString() + "/" + mainPlayerScript.combatStats.stat.actionPointMax.ToString ();
		
		switch(_element)
		{
		case 1:
			initialAPBar.color = Color.green;
			break;
		case 2:
			initialAPBar.color = Color.red;
			break;
		case 3:
			initialAPBar.color = Color.yellow;
			break;
		case 4:
			initialAPBar.color = Color.blue;
			break;
		}
	}

	public void SetInitialAPBar(GameObject _object)
	{
		initialAPBar = _object.GetComponent<Image>();
	}

	public void ActivateActionIntroMenu()
	{
		actionIntroPanel.SetTrigger ("AnimToggle");
		actionIntroCanvas.ignoreParentGroups = true;
	}

	public void UpdateActionInfo()
	{
		//Find the main Player
		for(int i = 0; i < CombatManager.playerStats.Count; i++)
		{
			if(CombatManager.playerStats[i].isMain == true)
			{
				//Character character = CombatManager.playerStats[i].character;
				//Update Element Text
				/*
				infoElementLevel[0].text = "Lv. " + character.earthAffinity.ToString () + " Earth";
				infoElementLevel[1].text = "Lv. " + character.fireAffinity.ToString ()  + " Fire";
				infoElementLevel[2].text = "Lv. " + character.lightningAffinity.ToString ()  + " Lightning";
				infoElementLevel[3].text = "Lv. " + character.waterAffinity.ToString ()  + " Water";
				*/
				//Update Element AP Text
				int index = 0;	//goes back to 0 when x = 3
				for(int x = 0; x < infoElementAP.Length; x++)
				{
					if(infoElementAP[x])
					{
						infoElementAP[x].text = CombatManager.playerStats[i].elementalAPCost[index].ToString ();
						index++;
						if(x % 3 == 0)
						{
							index = 0;
						}
					}

					//Overrides *** This is assuming you know where these values will go in the array
					//For the Lightning Support Action
					infoElementAP[10].text = "0";
				}
				break;
			}
		}
	}

	//This function is called to Change the Main Players Action Mode
	public void ActionModeShift(int _mode)
	{
		mainPlayerScript.SwitchToMode (_mode);
	}

	//This Function is called when an Elemental Button is pressed
	public void ElementalButtonTrigger(int _element)
	{
		mainPlayerScript.ElementalTrigger (_element);
	}

	public void ElementalButtonHold(int _element)
	{
		mainPlayerScript.ElementalChargeTrigger (_element);
	}

	public void ElementalButtonHoldCancel()
	{
		mainPlayerScript.ElementalChargeCancel ();
	}

	//This function creates bars depending on [0] elements and [1] how many bars to spawn
	public void CreateLevelBars(int _bars)
	{
		//Create new size of array 
		currentBars = new Transform[_bars]; 
		bar = new LevelBar[_bars]; 

		//For every bar that needs to be spawned
		for(int i = 0; i < _bars; i++)
		{
			//spawn bar
			currentBars[i] = Instantiate (levelBar, gaugeOrganiser.position, gaugeOrganiser.rotation) as Transform;
			//Parent Bar to UI Bar Organiser
			currentBars[i].SetParent (gaugeOrganiser, true);
			//Obtain Level Bar Script
			bar[i] = currentBars[i].gameObject.GetComponent<LevelBar>(); 
		}

		confirmationPanel.FadeTrigger (true);
	}

	public void ActivateLevelBar(int _element, int _index)
	{
		_index--; //Set to array readability
	
		bar[_index].RevealBar (_element);	//Reveal specified bar
	}

	public void ClearLevelBars()
	{
		//Destroy UI objects
		for(int i = 0; i < currentBars.Length; i++)
		{
			Destroy (currentBars[i].gameObject);
		}
		currentBars= new Transform[0];

		confirmationPanel.FadeTrigger (false);
	}

	public void HideLevelBars()
	{
		for(int i = 0; i < currentBars.Length; i++)
		{
			currentBars[i].gameObject.SetActive (false);
		}

		confirmationPanel.FadeTrigger (false);
	}

	public void RevealLevelBars()
	{
		for(int i = 0; i < currentBars.Length; i++)
		{
			currentBars[i].gameObject.SetActive (true);
		}

		confirmationPanel.FadeTrigger (true);
	}

	public void DeactivateLevelBar(int _index)
	{
		_index--; //Set to array readability

		bar[_index].HideBar ();	//Reveal specified bar
	}

	void SetElementSlider(GameObject _slider)
	{
		chargeSlider = _slider.GetComponent<Slider>();
	}

	void SetCriticalZones(float[] _critZone)
	{
		chargeCriticalZone = _critZone; 
	}

	public void CreateChargeSlider(int _element)
	{
		chargeSliderObject = Instantiate (chargeSliderManager, 
		                                  chargeSliderHolder.position, 
		                                  chargeSliderHolder.rotation) as Transform;
		chargeSliderObject.SetParent (chargeSliderHolder, true);
		chargeSliderObject.gameObject.SendMessage ("ActivateChargeSlider", _element, SendMessageOptions.DontRequireReceiver);
	}

	public void ClearChargeSlider()
	{
		Destroy (chargeSliderObject.gameObject);
		ClearLevelBars();
	}

	void SwitchToLoseScreen()
	{
		combatScreen.alpha = 0f;
		specialScreen.alpha = 0f;
		Transform screen = Instantiate (gameOverScreen, 
		                                endScreenNode.position, 
		                                endScreenNode.rotation) as Transform;
		screen.SetParent (endScreenNode, true);

		EventSystem.current.gameObject.GetComponent<BaseInputModule>().enabled = true;
	}

	void SwitchToWinScreen()
	{
		combatScreen.alpha = 0f;
		specialScreen.alpha = 0f;
		Transform screen = Instantiate (winScreen, 
		                                endScreenNode.position, 
		                                endScreenNode.rotation) as Transform;
		screen.SetParent (endScreenNode, true);

		EventSystem.current.gameObject.GetComponent<BaseInputModule>().enabled = true;
	}

	public void ActiveUISwitch(bool _active)
	{
		if(_active)
		{
			activeLayoutFadeIn = true;
			activeLayoutFadeOut = false;

			CombatInteractionSwitch(true);
		}
		else
		{
			activeLayoutFadeOut = true;
			activeLayoutFadeIn = false;

			CombatInteractionSwitch(false);
		}
	}

	public void CombatInteractionSwitch(bool _switch)
	{
		if(_switch)
		{
			//Turn on Interactivity
			combatScreen.blocksRaycasts = true;
		}
		else
		{
			//Turn off Interactivity
			combatScreen.blocksRaycasts = false;
		}
	}

	//The scan button uses this function to change scan mode to true
	public void ScanModeSwitch(bool _switch)
	{
		if(_switch)
		{
			scanMode = true;
			ActiveUISwitch(false);
		}
		else
		{
			scanMode = false;
			ActiveUISwitch (true);
		}
	}

	//This function is to activate the scan and store the values
	void SetScanValues(int[] _values)
	{
		//Top, Bottom, Right, Left
		scanValues = _values;
	}

	//This function is called from the player action script when the scan mode is on
	public void SubmitScan()
	{
		//This function will activate the scan:

		//Sending a message to Scan screen to send its scan values
		//Top, Bottom, Right, Left
		scanScreen.SubmitScanValues(); //The Player Action Script will access the scanValues values.

		//Turning off the Scan screen
		scanScreen.RevealScanScreen (false);

		ScanModeSwitch (false);

		//Scan all the enemies and objects
		//print ("Scanning Top = " + scanValues[0] + " Scanning Right = " + scanValues[1] + 
		//       " Scanning Bottom = " + scanValues[2] + " Scanning Left = " + scanValues[3]);

		//Scan All Enemies
		for(int i = 0; i < CombatManager.enemyStats.Count; i++)
		{
			CombatManager.enemyStats[i].SetScan(scanValues);
		}

		//Scan All Revealed Objects
		for (int i = 0; i < CombatManager.environmentObjects.Count; i++)
		{
			CombatManager.environmentObjects[i].SendMessage ("SetRevealedScan", scanValues, SendMessageOptions.DontRequireReceiver);
		}

		//Scan All Hidden Objects
		foreach(Transform child in objectHidden)
		{
			child.SendMessage ("SetHiddenScan", scanValues, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void ExitScan()
	{
		//Exit the scan mode
		ScanModeSwitch (false);
		scanScreen.RevealScanScreen (false);
	}

	public void ActivateSpecialParadoxScreen()
	{
		if(CombatManager.combatState == 0)
		{
			//Tutorial Segment
			if(SaveLoadManager.tutorialParadox == false)
			{
				specialScreen.gameObject.SetActive (false);
				tutorialSlides[6].gameObject.SetActive (true);
				SaveLoadManager.tutorialParadox = true;
			}

			specialScreen.alpha = 1f;
			combatScreen.alpha = 1f;
			specialParadox = true;
		}
	}


	//These Procedures are called to update the codex information
	public void SetCharacterCodex(CodexCharacter _codexInformation)
	{
		//Update the Character Summary Codex
		codex = _codexInformation;

		//If Enemy
		if(codex.isEnemy)
		{
			codexPageReady = false;
			
			//If Codex page still open update it
			if(codexCharacterPanel.gameObject.activeInHierarchy ||
			   codexObjectPanel.gameObject.activeInHierarchy)
			{
				OpenCharacterCodexPage();
			}
		}
		else
		{
			//Close the summary panels
			codexPageReady = false;

			//If Codex page still open update it
			if(codexCharacterPanel.gameObject.activeInHierarchy ||
			   codexObjectPanel.gameObject.activeInHierarchy)
			{
				OpenCharacterCodexPage();
			}
		}
	}

	public void ExitCodex()
	{
		codexExitButton = true;
	}


	//This function is called to Open Character Codex Page
	public void OpenCharacterCodexPage()
	{
		//Show Codex Page
		codexCharacterPanel.gameObject.SetActive (true);
		codexObjectPanel.gameObject.SetActive (false);

		//If the page has not been set up yet
		if(!codexPageReady)
		{
			//Set the page 

			//Look for by Name
			PlayerCombatCharacter character = null;
			
			EnemyCombatCharacter enemyCharacter = null;
			
			if(codex.isEnemy)
			{
				enemyCharacter = CombatManager.enemyStats[codex.index];
			}
			else
			{
				character = CombatManager.playerStats[codex.index];
			}

			//Update Title
			codexName.text = codex.name;

			//Update Level
			codexLevel.text = "Level " + codex.level;

			//Clean Up Affinity Backdrops
			for(int i = 0; i < codexMainBackdrop.Length; i++)
			{
				codexMainBackdrop[i].gameObject.SetActive (false);
				codexShieldBackdrop[i].gameObject.SetActive (false);
			}

			//Update Health Affinity Backdrop
			if(codex.healthReveal && codex.healthAffinity > 0)
			{
				codexMainBackdrop[codex.healthAffinity - 1].gameObject.SetActive (true);
			}

			//Update Shield Affinity Backdrop
			if(codex.shieldReveal && codex.shield > 0)
			{
				codexShieldBackdrop[codex.shieldAffinity - 1].gameObject.SetActive (true);
			}

			//Clean Up Status Immunity
			for(int i = 0; i < codexStatusImmunity.Length; i++)
			{
				codexStatusImmunity[i].gameObject.SetActive (false);
				codexStatusImmunityDash[i].gameObject.SetActive (false);
			}

			//Clean Up Affinity Information Panel
			for(int i = 0; i < codexShieldEarthInfo.Length; i++)
			{
				codexShieldEarthInfo[i].gameObject.SetActive (false);
				codexShieldFireInfo[i].gameObject.SetActive (false);
				codexShieldLightningInfo[i].gameObject.SetActive (false);
				codexShieldWaterInfo[i].gameObject.SetActive (false);
				codexMainEarthInfo[i].gameObject.SetActive (false);
				codexMainFireInfo[i].gameObject.SetActive (false);
				codexMainLightningInfo[i].gameObject.SetActive (false);
				codexMainWaterInfo[i].gameObject.SetActive (false);
			}


			//Update Affinity Information Panel, 0 - Strong, 1 - Weak, 2 - Resist, 3 - No Effect, 4 - Question

			//Update Shield Affinity Information
			if(codex.shieldReveal)
			{
				//This part is no longer supported
				codexShieldEarthInfo[3].gameObject.SetActive (false);
				codexShieldFireInfo[2].gameObject.SetActive (false);
				codexShieldLightningInfo[1].gameObject.SetActive (false);
				codexShieldWaterInfo[0].gameObject.SetActive (false);
//				switch(codex.shieldAffinity)
//				{
//				case 1: //If Shield is Earth
//					codexShieldEarthInfo[0].gameObject.SetActive (true);
//					codexShieldFireInfo[1].gameObject.SetActive (true);
//					codexShieldLightningInfo[2].gameObject.SetActive (true);
//					codexShieldWaterInfo[3].gameObject.SetActive (true);
//					break;
//				case 2: //If Shield is Fire
//					codexShieldEarthInfo[2].gameObject.SetActive (true);
//					codexShieldFireInfo[0].gameObject.SetActive (true);
//					codexShieldLightningInfo[3].gameObject.SetActive (true);
//					codexShieldWaterInfo[1].gameObject.SetActive (true);
//					break;
//				case 3: //If Shield is Lightning
//					codexShieldEarthInfo[1].gameObject.SetActive (true);
//					codexShieldFireInfo[3].gameObject.SetActive (true);
//					codexShieldLightningInfo[0].gameObject.SetActive (true);
//					codexShieldWaterInfo[2].gameObject.SetActive (true);
//					break;
//				case 4: //If Shield is Water
//					codexShieldEarthInfo[3].gameObject.SetActive (true);
//					codexShieldFireInfo[2].gameObject.SetActive (true);
//					codexShieldLightningInfo[1].gameObject.SetActive (true);
//					codexShieldWaterInfo[0].gameObject.SetActive (true);
//					break;
//				}
			}
			else
			{
				codexShieldEarthInfo[3].gameObject.SetActive (false);
				codexShieldFireInfo[2].gameObject.SetActive (false);
				codexShieldLightningInfo[1].gameObject.SetActive (false);
				codexShieldWaterInfo[0].gameObject.SetActive (false);
			}

			//Update Health Affinity Information
			if(codex.healthReveal)
			{
				switch(codex.healthAffinity)
				{
				case 0:
					codexMainEarthInfo[3].gameObject.SetActive (true);
					codexMainFireInfo[3].gameObject.SetActive (true);
					codexMainLightningInfo[3].gameObject.SetActive (true);
					codexMainWaterInfo[3].gameObject.SetActive (true);
					break; 
				case 1: //If Shield is Earth
					codexMainEarthInfo[0].gameObject.SetActive (true);
					codexMainFireInfo[1].gameObject.SetActive (true);
					codexMainLightningInfo[2].gameObject.SetActive (true);
					codexMainWaterInfo[3].gameObject.SetActive (true);
					break;
				case 2: //If Shield is Fire
					codexMainEarthInfo[2].gameObject.SetActive (true);
					codexMainFireInfo[0].gameObject.SetActive (true);
					codexMainLightningInfo[3].gameObject.SetActive (true);
					codexMainWaterInfo[1].gameObject.SetActive (true);
					break;
				case 3: //If Shield is Lightning
					codexMainEarthInfo[1].gameObject.SetActive (true);
					codexMainFireInfo[3].gameObject.SetActive (true);
					codexMainLightningInfo[0].gameObject.SetActive (true);
					codexMainWaterInfo[2].gameObject.SetActive (true);
					break;
				case 4: //If Shield is Water
					codexMainEarthInfo[3].gameObject.SetActive (true);
					codexMainFireInfo[2].gameObject.SetActive (true);
					codexMainLightningInfo[1].gameObject.SetActive (true);
					codexMainWaterInfo[0].gameObject.SetActive (true);
					break;
				}
			}
			else
			{
				codexMainEarthInfo[4].gameObject.SetActive (true);
				codexMainFireInfo[4].gameObject.SetActive (true);
				codexMainLightningInfo[4].gameObject.SetActive (true);
				codexMainWaterInfo[4].gameObject.SetActive (true);
			}

			//Update Notes Panel
			if(codexCurrentNotes)
			{
				Destroy(codexCurrentNotes.gameObject);
			}
			codexHidden.gameObject.SetActive (false);

			if(codex.noteRevealed)
			{
				codexCurrentNotes = Instantiate (codex.notes,codexNotes.position, codexNotes.rotation) as Transform;
				codexCurrentNotes.SetParent (codexNotes, true);
			}
			else
			{
				codexHidden.gameObject.SetActive (true);
			}

			//Update Stat Panel
			if(codex.noteRevealed)
			{
				codexStatTexts[0].text = codex.attack.ToString ();
				codexStatTexts[1].text = codex.defence.ToString ();
				codexStatTexts[2].text = codex.agility.ToString ();
				codexStatTexts[3].text = codex.luck.ToString ();
				codexStatTexts[4].text = codex.accuracy.ToString ();
				codexStatTexts[5].text = codex.speed.ToString ();

				//Update Pointers
				//First offs first clean up
				for(int i = 0; i < codexStatPointerUp.Length; i++)
				{
					codexStatPointerUp[i].gameObject.SetActive (false);
					codexStatPointerDown[i].gameObject.SetActive (false);
				}

				//Choose which pointer should be active
				
				//If Greater
				if(codex.isEnemy)
				{
					//Greater
					if(codex.attack > enemyCharacter.stat.attackBase)
					{
						codexStatPointerUp[0].gameObject.SetActive (true);
					}
					
					if(codex.defence > enemyCharacter.stat.defenceBase)
					{
						codexStatPointerUp[1].gameObject.SetActive (true);
					}
					
					if(codex.agility > enemyCharacter.stat.agilityBase)
					{
						codexStatPointerUp[2].gameObject.SetActive (true);
					}
					
					if(codex.luck > enemyCharacter.stat.luckBase)
					{
						codexStatPointerUp[3].gameObject.SetActive (true);
					}
					
					if(codex.accuracy > enemyCharacter.stat.accuracyBase)
					{
						codexStatPointerUp[4].gameObject.SetActive (true);
					}
					
					if(codex.speed > enemyCharacter.stat.speedBase)
					{
						codexStatPointerUp[5].gameObject.SetActive (true);
					}

					//Lesser
					if(codex.attack < enemyCharacter.stat.attackBase)
					{
						codexStatPointerDown[0].gameObject.SetActive (true);
					}
					
					if(codex.defence < enemyCharacter.stat.defenceBase)
					{
						codexStatPointerDown[1].gameObject.SetActive (true);
					}
					
					if(codex.agility < enemyCharacter.stat.agilityBase)
					{
						codexStatPointerDown[2].gameObject.SetActive (true);
					}
					
					if(codex.luck < enemyCharacter.stat.luckBase)
					{
						codexStatPointerDown[3].gameObject.SetActive (true);
					}
					
					if(codex.accuracy < enemyCharacter.stat.accuracyBase)
					{
						codexStatPointerDown[4].gameObject.SetActive (true);
					}
					
					if(codex.speed < enemyCharacter.stat.speedBase)
					{
						codexStatPointerDown[5].gameObject.SetActive (true);
					}
				}
				
				if(!codex.isEnemy)
				{
					//Greater
					if(codex.attack > character.stat.attackBase)
					{
						codexStatPointerUp[0].gameObject.SetActive (true);
					}
					
					if(codex.defence > character.stat.defenceBase)
					{
						codexStatPointerUp[1].gameObject.SetActive (true);
					}
					
					if(codex.agility > character.stat.agilityBase)
					{
						codexStatPointerUp[2].gameObject.SetActive (true);
					}
					
					if(codex.luck > character.stat.luckBase)
					{
						codexStatPointerUp[3].gameObject.SetActive (true);
					}
					
					if(codex.accuracy > character.stat.accuracyBase)
					{
						codexStatPointerUp[4].gameObject.SetActive (true);
					}
					
					if(codex.speed > character.stat.speedBase)
					{
						codexStatPointerUp[5].gameObject.SetActive (true);
					}
					
					//Lesser
					if(codex.attack < character.stat.attackBase)
					{
						codexStatPointerDown[0].gameObject.SetActive (true);
					}
					
					if(codex.defence < character.stat.defenceBase)
					{
						codexStatPointerDown[1].gameObject.SetActive (true);
					}
					
					if(codex.agility < character.stat.agilityBase)
					{
						codexStatPointerDown[2].gameObject.SetActive (true);
					}
					
					if(codex.luck < character.stat.luckBase)
					{
						codexStatPointerDown[3].gameObject.SetActive (true);
					}
					
					if(codex.accuracy < character.stat.accuracyBase)
					{
						codexStatPointerDown[4].gameObject.SetActive (true);
					}
					
					if(codex.speed < character.stat.speedBase)
					{
						codexStatPointerDown[5].gameObject.SetActive (true);
					}
				}
			}
			else
			{
				codexStatTexts[0].text = "?";
				codexStatTexts[1].text = "?";
				codexStatTexts[2].text = "?";
				codexStatTexts[3].text = "?";
				codexStatTexts[4].text = "?";
				codexStatTexts[5].text = "?";
			}

			//Update Portrait
			if(codex.portrait)
			{
				if(currentCodexAvatar)
				{
					Destroy (currentCodexAvatar.gameObject);
				}

				currentCodexAvatar = Instantiate (codex.portrait.transform,
				                                  codexAvatar.position,
				                                  codexAvatar.rotation) as Transform;
				currentCodexAvatar.SetParent (codexAvatar, true);
			}

			codexPageReady = true;
		}

		codexMode = true;
	}


	public void SetObjectCodex(int[] _reactants, string _name, string[] _notes)
	{
		//Update the Object Codex
		codexObjectReactant = _reactants;	//0 - Desired, 1 - Inapplicable, 2 - Undesired, 3 - Unknown
		codexObjectNotes = _notes;

		codexPageReady = false;

		//If Codex page still open update it
		if(codexCharacterPanel.gameObject.activeInHierarchy ||
		   codexObjectPanel.gameObject.activeInHierarchy)
		{
			codexPageReady = false;
			OpenObjectCodexPage();
		}
	}

	public void OpenObjectCodexPage()
	{
		//Show Object Codex Page
		codexCharacterPanel.gameObject.SetActive (false);
		codexObjectPanel.gameObject.SetActive (true);

		if(!codexPageReady)
		{
			//Clean Up
			for(int i = 0; i < codexObjectRevealed.Length; i++)
			{
				codexObjectRevealed[i].gameObject.SetActive (false);
				codexObjectHidden[i].gameObject.SetActive (false);
			}

			//Update Title
			//codexObjectName.text = sumObjectText.text;

			//Update Earth Info
			if(codexObjectReactant[0] != 3)	//If not unknown
			{
				//Reveal Information
				codexObjectRevealed[0].gameObject.SetActive (true);
				codexObjectRevealed[0].text = codexObjectNotes[0];
			}
			else
			{
				//Hidden Information
				codexObjectHidden[0].gameObject.SetActive (true);
			}

			//Update Fire Info
			if(codexObjectReactant[1] != 3)	//If not unknown
			{
				//Reveal Information
				codexObjectRevealed[1].gameObject.SetActive (true);
				codexObjectRevealed[1].text = codexObjectNotes[1];
			}
			else
			{
				//Hidden Information
				codexObjectHidden[1].gameObject.SetActive (true);
			}

			//Update Lightning Info
			if(codexObjectReactant[2] != 3)	//If not unknown
			{
				//Reveal Information
				codexObjectRevealed[2].gameObject.SetActive (true);
				codexObjectRevealed[2].text = codexObjectNotes[2];
			}
			else
			{
				//Hidden Information
				codexObjectHidden[2].gameObject.SetActive (true);
			}

			//Update Water Info
			if(codexObjectReactant[3] != 3)	//If not unknown
			{
				//Reveal Information
				codexObjectRevealed[3].gameObject.SetActive (true);
				codexObjectRevealed[3].text = codexObjectNotes[3];
			}
			else
			{
				//Hidden Information
				codexObjectHidden[3].gameObject.SetActive (true);
			}

			codexPageReady = true;
		}

		codexMode = true;
	}

	public void CloseCodexPages()
	{
		//Close Object Codex Page
		codexCharacterPanel.gameObject.SetActive (false);
		codexObjectPanel.gameObject.SetActive (false);

		codexMode = false;
	}


	//These functions are called to manage the turn of action
	void ResetTimeOfAction()
	{
		//This function will also be called when a character is added or removed so update everything

		//Delete child notes and create new arrays
		for (int i = 0; i < turnOrder.Length; i++)
		{
			Destroy (turnAvatar[i].gameObject);
			Destroy (turnOrder[i].gameObject);
		}

		int maxCharacters = CombatManager.players.Count + CombatManager.enemies.Count;
		turnAvatar = new Transform[maxCharacters];
		turnOrder = new Transform[maxCharacters];

		//Instantiate turn nodes and avatars
		for(int i = 0; i < turnOrder.Length; i++)
		{
			//Instantiate turn nodes and parent to noteOrder
			turnOrder[i] = Instantiate (turnNode, nodeOrder.position, nodeOrder.rotation) as Transform;
			turnOrder[i].SetParent (nodeOrder, true);
		}

		//Instantiate player avatars and parent to avatarOrder
		for(int p = 0; p < CombatManager.players.Count; p++)
		{
			turnAvatar[p] = Instantiate (CombatManager.playerStats[p].characterAvatar, 
			                             avatarOrder.position, avatarOrder.rotation) as Transform;
			turnAvatar[p].SetParent (avatarOrder, true);

			//Give the avatars a reference to the stat script to identify status effects
			turnAvatar[p].GetComponent<AvatarScript>().SetStatIndex (false, p);
		}
		
		//Instantiate enemy avatars and parent to avatarOrder, Special Note: this is adding on top of the Turn avatar array
		for(int e = 0; e < CombatManager.enemies.Count; e++)
		{
			turnAvatar[e + CombatManager.players.Count] = Instantiate (CombatManager.enemyStats[e].characterAvatar,
			                                                           avatarOrder.position, avatarOrder.rotation) as Transform;
			turnAvatar[e + CombatManager.players.Count].SetParent (avatarOrder, true);

			//Give the avatars a reference to the stat script to identify status effects
			turnAvatar[e + CombatManager.players.Count].GetComponent<AvatarScript>().SetStatIndex (true, e);
		}
	}

	public void TimeOfActionFadeIn()
	{
		timeOfActionFadeIn = true;
	}

	void UpdateTimeOfAction(int[] _speeds)
	{
		//After all speed accumulated, order the Time of Action UI

		/*
		for(int i = 0; i < _speeds.Length; i++)
		{
			print (_speeds[i]);
		}
		*/

		//For as many players and enemies
		for(int i = 0; i < turnAvatar.Length; i++)
		{
			int bestSpeed = 0;
			int bestIndex = 0;

			//Find out the highest speed
			for(int s = 0; s < turnAvatar.Length; s++)
			{
				if(_speeds[s] > bestSpeed)
				{
					//Record Index
					bestIndex = s;

					//Record Speed
					bestSpeed = _speeds[s];
				}
			}

			//Set the best index speed to -1, a cheap trick to get the other highest numbers on next loop....
			_speeds[bestIndex] = -1;

			//Send the winning avatar index the move to function
			turnAvatar[bestIndex].gameObject.SendMessage ("MoveToObject", turnOrder[i], SendMessageOptions.DontRequireReceiver);

			//Scale appropriately
			if(i == 0)
			{
				//If on top of the Time of Action set scale to something big
				turnAvatar[bestIndex].gameObject.SendMessage ("ScaleToLength", 1.3f, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				//If not on top of the Time of Action set scale to something small
				turnAvatar[bestIndex].gameObject.SendMessage ("ScaleToLength", 1f, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	//This procedure is called from the navigation buttons in the codex menu
	public void NextCodexSelection(int _direction)
	{
		CombatManager.currentPlayer.SendMessage ("NextSelection", _direction, SendMessageOptions.DontRequireReceiver);
	}

	public void NextCodexGroup()
	{
		CombatManager.currentPlayer.SendMessage ("NextGroup", SendMessageOptions.DontRequireReceiver);
	}

	//This procedure is called when the global message is to be revealed
	public void SetGlobalMessage(string _message)
	{
		//Reset Timer
		globalMessageTimer = globalMessageMaxTimer;

		//Set Text
		globalMessage.text = _message;

		globalMessageAnim.SetTrigger ("Reset");
	}

	public void SetSelectorCollider(bool _switch)
	{
		GameObject.FindGameObjectWithTag("Combat Selector").SendMessage ("ColliderSwitch", _switch, SendMessageOptions.DontRequireReceiver);
	}

	public void ClearUIEffects()
	{
		for(int i = 0; i < effectsUI.Length; i++)
		{
			effectsUI[i].SetActive (false);
		}	
	}
}
