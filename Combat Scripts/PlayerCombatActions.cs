using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//Player Actions.... Can be re used to specify different animator, time etc. Don't need stuff like EinarAction etc

public class PlayerCombatActions : MonoBehaviour 
{
	//Animator
	public Animator anim;

	//This variable determines and initialises combat variables only once when combat has properly been setup
	private bool start = false;

	private Transform selector;
	private bool isSelecting;
	private int objectSelection = 1; //0 = Players, 1 = Enemies, 2 = Environment
	private int [] objectCurrentSelection = new int[3];
	private int selectDirection = 0;
	private float selectionDelay = 0.5f;

	//Move to and Rotate Functions
	private bool lookAtTarget = false;
	private bool moveToTarget = false;
	private bool lookAtInitialRotation = false;
	private bool moveToInitialPosition = false;

	private float lookAtTargetTimer = 1f;
	private float targetWidth;

	private float rotateSpeed;
	private float moveSpeed;
	public float teleportDashSpeed = 50f;
	public float teleportRotateSpeed = 5f;

	[HideInInspector]
	public PlayerCombatCharacter combatStats;

	//Player button feedback variables
	/*
	 * 0 = Idle
	 * 1 = Ready
	 * 2 = Single Attack
	 * 3 = Flurry Attack
	 * 4 = Charge Attack
	*/
	private int attackPhase; 
	
	public float readyMaxTimer = 0.5f;
	private float readyTimer;
	private float idleDelay = 0f; //This function is used to create a delay if reset to idle

	private bool singleShot;
	private bool charging;
	private bool environmentInteract;

	public float flurryMaxTimer = 1f;
	private float flurryTimer;
	private int flurryNextEnemy = 0;
	private int[] flurryNextElements; 
	private int flurryCurrentButton = 0;

	private float charge = 0f; 
	private float chargeMax;
	public float chargeSpeed = 2f;
	private float chargeBarCount = 0f;
	private float chargeAPCount = 0f;
	private int chargeCurrentBar = 0;
	private bool chargeInstantCritical = false;

	//Attack Accuracy
	[Range(0f, 1f)]
	public float flurryAccuracy = 1f;
	[Range(0f, 1f)]
	public float chargeAccuracy = 1f;

	//Status Effect Chances
	[Range(0f, 1f)]
	public float singleShotStatusChance = 0.5f;
	[Range(0f, 1f)]
	public float flurryStatusChance = 0.10f;
	[Range(0f, 1f)]
	public float chargeStatusChance = 0.25f;
	[Range(0f, 1f)]
	public float curseStatusChance = 0.80f;

	//Critical Chances
	[Range(0f, 1f)]
	public float singleShotCritChance = 0.40f;
	[Range(0f, 1f)]
	public float flurryCritChance = 0.10f;
	[Range(0f, 1f)]
	public float chargeCritChance = 0.75f;

	//Attack Phase Initiators
	private int elementalStance = 0; //1 = Earth, 2 = Fire, 3 = Lightning, 4 = Water
	private bool flurryActivate = false;
	private bool flurryReady = false;

	//Combat Effects
	public Transform[] projectileNode;

	//Single Shot Projectiles
	public Transform[] singleShotProjectiles;

	//Charge Effect
	public Transform chargeNode;
	public Transform[] chargeEffect;
	private Transform currentCharge;

	//Curse Effects
	public Transform [] curseEffect;

	//Level Bar Limits
	private int[] levelBarLimit = new int[5]; //0 - normal, 1 - earth, 2 - fire, 3 - lightning, 4 - water
	[HideInInspector]
	public int currentLevelBar = 0;

	//Attack Mode Level Unlock
	public int flurryLevelUnlock = 2;
	public int chargeLevelUnlock = 3; 

	//Sound Effects
	public Transform[] flurrySound;

	//Renderers To Hide and Effects/ Objects to Show during teleport 
	public GameObject[] teleportHideObjects;
	public Renderer[] teleportHideRenderers;
	public GameObject[] teleportRevealEffects;

	//Scan Mode Variables
	private float scanModeDelay = 0.5f;
	private bool scanModeActivate = false;

	//Action Modes: 0 - Attack, 1 - Defend, 2 - Support, 3 - Curse
	[HideInInspector]
	public int actionMode = 0;
	private bool actionModeLock = false;
	private bool modeSelection = true;
	
	private bool defendMode = false;
	private bool supportMode = false;
	private bool curseMode = false;

	//Action Button Presses
	private bool[] elementalButton = new bool[4];
	private bool[] elementalCharge = new bool[4];

	//Emit Action
	public Transform emitAction;
	private Transform currentEmit;

	//Confirmation before action
	private bool confirmed = false;

	//Codex Trigger Delay, because of the use of controller trigger axis
	private float codexActivateDelay = 0f;

	void Awake()
	{
		//Connect to stats
		combatStats = gameObject.GetComponent<PlayerCombatCharacter>();

		//Find and reference the selector
		selector = GameObject.FindGameObjectWithTag("Combat Selector").transform;

		//Turn this script off in case its on at the start
		this.enabled = false;
	}

	void OnEnable()
	{
		//Initialise Once
		if(!start)
		{
			//Calculate the middle Selections
			//Player Selection
			objectCurrentSelection[0] = 0;
			//Enemy Selection
			objectCurrentSelection[1] = (CombatManager.enemies.Count / 2);
			//Object Selection
			objectCurrentSelection[2] = 0;

			SetLevelBarSizes ();

			start = true;	//End Initialisation
		}

		//If Battered
		if(combatStats.elementalReaction.elementalEffect[3] > 0)
		{
			float batteredChance = Random.Range (0f, 1f);

			if(batteredChance > 0.5f)
			{
				print ("I'm battered bruh");
				combatStats.ShowDamageText ("Battered", Color.white, 1f);
				EndTurnDelay (0.11f);
			}
			else
			{
				//Snapped out of it
				combatStats.elementalReaction.elementalEffect[3] = 0;

				//Re Call OnEnable
				OnEnable ();
			}
		}
		else
		{
			print ("I'm the Player and its my turn HUZZAH.. ");

			//Just to be safe 
			CancelInvoke ();

			//Make sure Selector Collider is off
			selector.gameObject.SendMessage ("ColliderSwitch", false, SendMessageOptions.DontRequireReceiver);

			//Set Confirmation to false
			confirmed = false;

			//As this will be the Main Player Script activate show the scan button
			combatStats.ui.ActiveUISwitch (true);	//Turn on UI elements
			combatStats.statusUI.SetCurrent (true);	//Set the UI that this character is the current

			//Correct Position
			anim.transform.position = transform.position;

			//Default Action Variables
			lookAtTarget = false;
			moveToTarget = false;	
			lookAtInitialRotation = false;
			moveToInitialPosition = false;
			lookAtTargetTimer = 1f;
			
			//Default Attack phase 
			attackPhase = 0; //Idle
			readyTimer = readyMaxTimer;
			idleDelay = 0f;
			flurryTimer = flurryMaxTimer;
			elementalStance = 0;
			flurryActivate = false; 
			flurryReady = false;
			flurryNextEnemy = 0;
			flurryCurrentButton = 0;
			currentLevelBar = 0;

			singleShot = false;
			charging = false;
			environmentInteract = false;
			charge = 0f; 
			chargeBarCount = 0f;
			chargeAPCount = 0f;
			chargeCurrentBar = 0;
			chargeInstantCritical = false;

			//Animation Default
			anim.SetInteger ("Attack Phase", 0);
			anim.SetInteger ("Index", 0);
			if(combatStats.defend)
			{
				combatStats.ui.actionIntroPanel.SetTrigger ("Exit");
			}

			//Set up the Object Selection
			UpdateObjectSelection();

			//Default Scan Mode Variables
			scanModeDelay = 0.5f;
			scanModeActivate = false;

			//Action Mode Default Variables
			actionModeLock = false;
			actionMode = 0;
			modeSelection = true;
			defendMode = false;
			supportMode = false;
			curseMode = false;

			//Set the Defend Mode Boolean back to false
			combatStats.defend = false;
			combatStats.statusUI.protectedPanel.gameObject.SetActive (false);

			//Turn off any button presses
			TurnOffElementalButtons ();
			ElementalChargeCancel ();

			if(modeSelection)
			{
				combatStats.ui.ActivateActionIntroMenu ();
				combatStats.ui.CombatInteractionSwitch (true);
				combatStats.ui.TimeOfActionFadeIn ();
			}

			//Mark as Current
			combatStats.currentTurn = true;

			//Update Sequencer
			combatStats.ui.StartSequencer(combatStats.character.name);

			//Reveal Emit Action
			if(emitAction && !combatStats.isMain)
			{
				if(currentEmit)
				{
					Destroy (currentEmit.gameObject);
				}

				currentEmit = Instantiate (emitAction, anim.transform.position, anim.transform.rotation) as Transform;
				currentEmit.SetParent (anim.transform, true);
			}

			IntroCameraSettings();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//When this script is updating press a button to do something
		//If there are still enemies and not in scan mode and not activating scan mode (stop buttons being pressed)
		if(CombatManager.enemies.Count > 0 && 
		   !CombatUIManager.scanMode && !scanModeActivate) 
		{
			if(selectionDelay <= 0f)
			{
				//Add Extra Selection Controls
				if(Input.GetAxis("Mouse ScrollWheel") < -0f || Input.GetKeyDown(KeyCode.A) ||
				   Input.GetKeyDown (KeyCode.LeftArrow) || 
				   Input.GetAxis("HorizontalWithMouse") < -0.85f && !combatStats.ui.codexMode || 
				   Input.GetAxis("Horizontal") < -0.85f)
				{
					selectDirection = -1;
					selectionDelay = 0.3f;
				}
				if(Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.D) ||
				   Input.GetKeyDown (KeyCode.RightArrow) || 
				   Input.GetAxis("HorizontalWithMouse") > 0.85f && !combatStats.ui.codexMode || 
				   Input.GetAxis("Horizontal") > 0.85f)
				{
					selectDirection = 1;
					selectionDelay = 0.3f;
				}
			}
			else
			{
				selectionDelay -= Time.deltaTime;
			}

			//-------------------------------------Increase Time during action---------------------------
			if(Time.timeScale == 1f && confirmed)
			{
				//If time scale is normal and attack confirmed
				if(Input.GetButtonDown ("Earth") || Input.GetButtonDown ("Submit") ||
				   Input.GetMouseButtonDown (0))
				{
					if(combatStats.ui.skipPrompt.activeInHierarchy)
					{
						Time.timeScale = CombatManager.skipTimeScale;
						combatStats.ui.skipPrompt.SetActive (true);
					}
					else
					{
						Time.timeScale = 1f;
						combatStats.ui.skipPrompt.SetActive (true);
					}
				}
			}

			//-------------------------------------Selection---------------------------------------------
			ControlSelection ();	

			//-------------------------------------Action Phases------------------------------------------
			switch(attackPhase)
			{
			case 0:	//Idle
				if(!combatStats.ui.codexMode)
				{
					//When Mode is selected
					if(!modeSelection)
					{
						//Make sure when restarting to idle because reasons there is a delay 
						if(idleDelay <= 0f)
						{
							//If Defend Mode
							if(actionMode == 1)
							{
								combatStats.ui.ReadyForConfirmation();

								elementalStance = 0;
								
								//Set Defensive Camera
								DefensiveCameraSettings ();
								
								UpdateObjectSelection ();
								
								attackPhase = 6;
							}

							//Activate Correct Action
							if(Input.GetKeyDown (KeyCode.E) || elementalButton[0] || elementalCharge[0])
							{
								elementalStance = 1; //Earth Elemental Stance

								switch(actionMode)
								{
								case 0: //Attack Mode									
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[0])
									{
										combatStats.ui.ReadyForConfirmation ();

										//AP Cost
										combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);
										
										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Offensive Camera
										OffensiveCameraSettings ();

										UpdateObjectSelection ();

										//Move into Attack
										attackPhase = 2;
										anim.SetInteger ("Attack Phase", 1);	//Ready Animation

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								case 1: //Defend Mode
									//Reveal Collider Trigger
									//selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[0])
									{
										combatStats.ui.ReadyForConfirmation();

										//AP Cost
										combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);

										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Defensive Camera
										DefensiveCameraSettings ();

										UpdateObjectSelection ();

										attackPhase = 6;

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								case 2:	//Support Mode
									//Reveal Collider Trigger
									//selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[0])
									{
										combatStats.ui.ReadyForConfirmation();

										//AP Cost - if not the lightning buff
										if(elementalStance != 3)
										{
											combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);
										}

										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Defensive Camera
										DefensiveCameraSettings ();

										UpdateObjectSelection ();

										attackPhase = 7;

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								case 3:	//Curse Mode
									//Reveal Collider Trigger
									//selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[0])
									{
										combatStats.ui.ReadyForConfirmation();

										//AP Cost
										combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);

										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Offensive Camera
										OffensiveCameraSettings ();

										UpdateObjectSelection ();

										attackPhase = 8;	

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;								
								}
							}
							else if(Input.GetKeyDown (KeyCode.Q) || elementalButton[1] || elementalCharge[1])
							{
								elementalStance = 2; //Fire Elemental Stance
								
								switch(actionMode)
								{
								case 0: //Attack Mode
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[1])
									{
										combatStats.ui.ReadyForConfirmation ();

										//AP Cost
										combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);
										
										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Offensive Camera
										OffensiveCameraSettings ();

										UpdateObjectSelection ();
										
										//Move into Attack
										attackPhase = 2;
										anim.SetInteger ("Attack Phase", 1);	//Ready Animation

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								case 1: //Defend Mode
									//Reveal Collider Trigger
									//selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[1])
									{
										combatStats.ui.ReadyForConfirmation();

										//AP Cost
										combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);
										
										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Defensive Camera
										DefensiveCameraSettings ();

										UpdateObjectSelection ();

										attackPhase = 6;

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								case 2:	//Support Mode
									//Reveal Collider Trigger
									//selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[1])
									{
										combatStats.ui.ReadyForConfirmation();

										//AP Cost - if not the lightning buff
										if(elementalStance != 3)
										{
											combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);
										}
										
										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Defensive Camera
										DefensiveCameraSettings ();

										UpdateObjectSelection ();

										attackPhase = 7;

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								case 3:	//Curse Mode
									//Reveal Collider Trigger
									//selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[1])
									{
										combatStats.ui.ReadyForConfirmation();

										//AP Cost
										combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);
										
										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Offensive Camera
										OffensiveCameraSettings ();

										UpdateObjectSelection ();

										attackPhase = 8;	

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								}
							}
							else if(Input.GetKeyDown (KeyCode.R) || elementalButton[2] || elementalCharge[2])
							{
								elementalStance = 3; //Lightning Elemental Stance
								
								switch(actionMode)
								{
								case 0: //Attack Mode
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[2])
									{
										combatStats.ui.ReadyForConfirmation ();

										//AP Cost
										combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);
										
										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Offensive Camera
										OffensiveCameraSettings ();

										UpdateObjectSelection ();
										
										//Move into Attack
										attackPhase = 2;
										anim.SetInteger ("Attack Phase", 1);	//Ready Animation

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								case 1: //Defend Mode
									//Reveal Collider Trigger
									//selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[2])
									{
										combatStats.ui.ReadyForConfirmation();

										//AP Cost
										combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);

										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Defensive Camera
										DefensiveCameraSettings ();

										UpdateObjectSelection ();

										attackPhase = 6;

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								case 2:	//Support Mode
									//Reveal Collider Trigger
									//selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);

									combatStats.ui.ReadyForConfirmation();

									//AP Cost - if not the lightning buff
									if(elementalStance != 3)
									{
										combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);
									}
									
									//Create Sequence 
									//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

									//Set Defensive Camera
									DefensiveCameraSettings ();

									UpdateObjectSelection ();

									attackPhase = 7;	

									combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");

									break;
								case 3:	//Curse Mode
									//Reveal Collider Trigger
									//selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[2])
									{
										combatStats.ui.ReadyForConfirmation();

										//AP Cost
										combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);
										
										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Offensive Camera
										OffensiveCameraSettings ();

										UpdateObjectSelection ();

										attackPhase = 8;	

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								}
							}
							else if(Input.GetKeyDown (KeyCode.W) || elementalButton[3] || elementalCharge[3])
							{
								elementalStance = 4; //Water Elemental Stance
								
								switch(actionMode)
								{
								case 0: //Attack Mode
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[3])
									{
										combatStats.ui.ReadyForConfirmation ();

										//AP Cost
										combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);
										
										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");									

										//Set Offensive Camera
										OffensiveCameraSettings ();

										UpdateObjectSelection ();
										
										//Move into Attack
										attackPhase = 2;
										anim.SetInteger ("Attack Phase", 1);	//Ready Animation

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								case 1: //Defend Mode
									//Reveal Collider Trigger
									//selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[3])
									{
										combatStats.ui.ReadyForConfirmation();

										//AP Cost
										combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);

										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Defensive Camera
										DefensiveCameraSettings ();

										UpdateObjectSelection ();

										attackPhase = 6;

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								case 2:	//Support Mode
									//Reveal Collider Trigger
									//selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[3])
									{
										combatStats.ui.ReadyForConfirmation();

										//AP Cost - if not the lightning buff
										if(elementalStance != 3)
										{
											combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);
										}

										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Defensive Camera
										DefensiveCameraSettings ();

										UpdateObjectSelection ();

										attackPhase = 7;

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								case 3:	//Curse Mode
									//Reveal Collider Trigger
									//selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
									if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[3])
									{
										combatStats.ui.ReadyForConfirmation();

										//AP Cost
										combatStats.APCost (combatStats.elementalAPCost[elementalStance-1], elementalStance);									

										//Create Sequence 
										//combatStats.ui.sequenceUI.CreateSequence ("Confirm");

										//Set Offensive Camera
										OffensiveCameraSettings ();

										UpdateObjectSelection ();

										attackPhase = 8;	

										combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
									}
									else
									{
										combatStats.ui.SetGlobalMessage ("Not Enough AP");
									}
									break;
								}
							}
						}
						else
						{
							//Count down through idle time
							idleDelay -= Time.deltaTime;
						}
					}
					else
					{
						//If in Mode Selection Make sure Action Screen Stays
						//Glitch Fix, when Action Screen just doesn't appear when this script is on
						if(combatStats.ui.actionIntroPanel.GetCurrentAnimatorStateInfo (0).IsName ("Action Intro Exit") &&
						   modeSelection)
						{
							combatStats.ui.actionIntroPanel.SetTrigger ("AnimToggle");
						}
					}

					if(Input.GetMouseButton (2) || Input.GetAxis ("Triggers") > 0.6f)
					{
						if(codexActivateDelay <= 0f)
						{
							//Move to Codex Mode
							if(objectSelection == 0)
							{
								combatStats.ui.OpenCharacterCodexPage ();
							}
							else if(objectSelection == 1)
							{
								combatStats.ui.OpenCharacterCodexPage ();
							}
							else if(objectSelection == 2)
							{
								combatStats.ui.OpenObjectCodexPage ();
							}
							combatStats.ui.codexMode = true;
							Cursor.visible = true;
							Cursor.lockState = CursorLockMode.None;
							isSelecting = true;
							actionModeLock = false;

							EventSystem.current.gameObject.GetComponent<BaseInputModule>().enabled = true;

							//Reveal Collider Trigger
							selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
							SwitchToMode (0);

							//Start Delay
							codexActivateDelay = 0.5f;

							UpdateObjectSelection();
						}
						else
						{
							codexActivateDelay -= Time.deltaTime;
						}
					}
				}
				else
				{
					//Close Codex Screen
					if(Input.GetMouseButton (2) || Input.GetAxis ("Triggers") > 0.6f||
					   Input.GetKey (KeyCode.Escape) || Input.GetButton ("Cancel") ||
					   combatStats.ui.codexExitButton || Input.GetButton ("Fire") || Input.GetButton ("Back"))
					{
						if(codexActivateDelay <= 0f)
						{
							combatStats.ui.codexExitButton = false;

							combatStats.ui.CloseCodexPages ();
							//Lock and hide cursor
							if(!Application.isEditor)
							{
								Cursor.visible = false;
								Cursor.lockState = CursorLockMode.Locked;
							}
							combatStats.ui.codexMode = false;
							actionModeLock = true;
							isSelecting = false;
							ReturnToIdle ();

							EventSystem.current.gameObject.GetComponent<BaseInputModule>().enabled = false;

							//Hide Collider Trigger
							selector.gameObject.SendMessage ("ColliderSwitch", false, SendMessageOptions.DontRequireReceiver);

							combatStats.ui.actionIntroPanel.SetTrigger ("Exit");

							combatStats.ui.sequenceUI.ClearSequence ();
							combatStats.ui.sequenceUI.CreateSequence (combatStats.character.name);

							codexActivateDelay = 0.5f;
						}
						else
						{
							codexActivateDelay -= Time.deltaTime;
						}
						
					}
					if(Input.GetKeyUp (KeyCode.Tab) || Input.GetButtonUp ("Switch"))
					{
						NextGroup ();
					}
				}
				/*
				if(!combatStats.ui.codexMode)
				{
					if(!modeSelection)
					{
						if(idleDelay <= 0f)
						{
							if(Input.GetKeyDown (KeyCode.E) || elementalButton[0] || elementalCharge[0])
							{
								elementalStance = 1; //Earth Elemental Stance

								switch(actionMode)
								{
								case 0:	//Attack Mode
									if(objectSelection == 1)	//If in Enemy Selection
									{
										attackPhase = 1;
										anim.SetInteger ("Attack Phase", 1);	//Set Attack Phase Animation to Ready
									}
									else if(objectSelection == 2)//If in environment selection
									{
										attackPhase = 5;
									}
									break;
								case 1: //Defend Mode
									attackPhase = 6;
									break;
								case 2:	//Support Mode
									attackPhase = 7;

									//Set Global Message
									combatStats.ui.SetGlobalMessage ("Aura");

									break;
								case 3:	//Curse Mode
									attackPhase = 8;

									//Set Global Message
									combatStats.ui.SetGlobalMessage ("Condemn");

									break;
								}
							}
							else if(Input.GetKeyDown (KeyCode.Q) || elementalButton[1] || elementalCharge[1])
							{
								elementalStance = 2; //Fire Elemental Stance

								switch(actionMode)
								{
								case 0: //Attack Mode
									if(objectSelection == 1)
									{
										attackPhase = 1;
										anim.SetInteger ("Attack Phase", 1);	//Set Attack Phase Animation to Ready
									}
									else if(objectSelection == 2)
									{
										attackPhase = 5;
									}
									break;
								case 1: //Defend Mode
									attackPhase = 6;
									break;
								case 2:	//Support Mode
									attackPhase = 7;

									//Set Global Message
									combatStats.ui.SetGlobalMessage ("Blazing Spirit");

									break;
								case 3:	//Curse Mode
									attackPhase = 8;

									//Set Global Message
									combatStats.ui.SetGlobalMessage ("Burn");

									break;
								}
							}
							else if(Input.GetKeyDown (KeyCode.R) || elementalButton[2] || elementalCharge[2])
							{
								elementalStance = 3; //Lightning Elemental Stance

								switch(actionMode)
								{
								case 0: //Attack Mode
									if(objectSelection == 1)
									{
										attackPhase = 1;
										anim.SetInteger ("Attack Phase", 1);	//Set Attack Phase Animation to Ready
									}
									else if(objectSelection == 2)
									{
										attackPhase = 5;
									}
									break;
								case 1: //Defend Mode
									attackPhase = 6;
									break;
								case 2:	//Support Mode
									attackPhase = 7;

									//Set Global Message
									combatStats.ui.SetGlobalMessage ("Overcharge");

									break;
								case 3:	//Curse Mode
									attackPhase = 8;

									//Set Global Message
									combatStats.ui.SetGlobalMessage ("Stun");

									break;
								}
							}
							else if(Input.GetKeyDown (KeyCode.W) || elementalButton[3] || elementalCharge[3])
							{
								elementalStance = 4; //Water Elemental Stance

								switch(actionMode)
								{
								case 0: //Attack Mode
									if(objectSelection == 1)
									{
										attackPhase = 1;
										anim.SetInteger ("Attack Phase", 1);	//Set Attack Phase Animation to Ready
									}
									else if (objectSelection == 2)
									{
										attackPhase = 5;
									}
									break;
								case 1: //Defend Mode
									attackPhase = 6;
									break;
								case 2:	//Support Mode
									attackPhase = 7;
									//Set Global Message
									combatStats.ui.SetGlobalMessage ("Purify");

									break;
								case 3:	//Curse Mode
									attackPhase = 8;

									//Set Global Message
									combatStats.ui.SetGlobalMessage ("Rust");

									break;
								}
							}

							//Move to Scan Mode
							else if(Input.GetKeyDown(KeyCode.Space))
							{
								combatStats.ui.ScanModeSwitch (true);
								combatStats.ui.scanScreen.RevealScanScreen(true);
							}

							//Move to Codex Mode
							else if(Input.GetKeyDown (KeyCode.Z) || combatStats.ui.codexButtonSelect == true)
							{
								if(objectSelection == 0)
								{
									combatStats.ui.OpenCharacterCodexPage ();
								}
								else if(objectSelection == 1)
								{
									combatStats.ui.OpenCharacterCodexPage ();
								}
								else if(objectSelection == 2)
								{
									combatStats.ui.OpenObjectCodexPage ();
								}
							}

							//Change Modes
							else if(Input.GetKeyDown (KeyCode.Tab))
							{
								SwitchToNextMode ();
							}
						}
						else
						{
							idleDelay -= Time.deltaTime;
						}
					}
				}
				else
				{
					//Close Codex Screen
					if(Input.GetKeyDown (KeyCode.Z)||Input.GetKeyDown (KeyCode.Escape)
					   || combatStats.ui.codexButtonSelect == true)
					{
						combatStats.ui.CloseCodexPages ();
					}
					if(Input.GetKeyDown (KeyCode.Tab))
					{
						SwitchToNextMode ();
						SwitchToNextMode ();
					}
				}
				 */
				break;
			case 1: //Ready
				ReadyingAttack ();
				break;
			case 2: //Single Shot Attack Mode
				if(confirmed)
				{
					if(singleShot)
					{
						print ("Attack");
						anim.SetInteger ("Index", 0); //*For now using 0 as variation*
						anim.SetInteger ("Attack Phase", attackPhase);
						
						isSelecting = false;
						
						singleShot = false;
					}
				}
				else
				{
					if(!isSelecting)
					{
						isSelecting = true;
						//Reveal Collider Trigger
						selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
						
						//Show the Time of Action and Party Layout
						combatStats.ui.partyLayoutPanel.gameObject.SendMessage ("FadeTrigger", true, SendMessageOptions.DontRequireReceiver);
						combatStats.ui.timeOfAction.gameObject.SendMessage ("FadeTrigger", true, SendMessageOptions.DontRequireReceiver);
						//Hide the Action Description
						combatStats.ui.actionDescription.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);

						//Create Sequence 
						combatStats.ui.sequenceUI.CreateSequence ("Select");

						//Reveal Elemental Button Effect
						combatStats.ui.effectsUI[elementalStance - 1 + 4].SetActive (true);
					}
					else
					{
						//If chosen target then it is finally confirmed
						if(Input.GetButtonDown ("Submit") || Input.GetButtonDown ("Earth") || Input.GetMouseButtonDown (0)
						   || CombatManager.enemies.Count == 1)
						{
							ConfirmAction ();
							singleShot = true;

							//Set Single Shot Camera
							SingleShotCameraSettings ();

							//Clear UI Effects
							combatStats.ui.ClearUIEffects ();
						}
						
						if(Input.GetButtonDown ("Back") || Input.GetButtonDown ("Fire"))
						{
							isSelecting = false;
							//Reveal Collider Trigger
							//selector.gameObject.SendMessage ("ColliderSwitch", false, SendMessageOptions.DontRequireReceiver);
							//Go back to screen
							
							//Fade the Time of Action and Party Layout
							combatStats.ui.partyLayoutPanel.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);
							combatStats.ui.timeOfAction.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);
							//Show the Action Description
							combatStats.ui.actionDescription.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);

							//Destroy latest sequence
							combatStats.ui.sequenceUI.DestroyLatestSequence ();

							//Go back to screen
							ReturnToIdle ();
							combatStats.ui.actionIntroPanel.SetTrigger ("AttackMode");
							
							//Return AP Cost
							combatStats.RegenAP (false, combatStats.elementalAPCost[elementalStance-1]);

							//Clear UI Effects
							combatStats.ui.ClearUIEffects ();

							//Set Intro Camera
							IntroCameraSettings ();
						}
					}
				}
				break;
			case 3: //Flurry Attack Mode
				FlurryAttack ();
				break;
			case 4: //Charging Attack Mode
				ChargeAttack ();
				break;
			case 5:	//Environment Interaction
				if(!environmentInteract)
				{
					CombatManager.environmentObjects[objectCurrentSelection[2]].SendMessage ("EnvironmentInteract", elementalStance, 
					                                                         SendMessageOptions.DontRequireReceiver);

					//Call the camera
					CombatCamera.control.SpawnGlobalAnimation (null, "View Battlefield", 400);

					//Hide Selection
					HideSelection ();
					EndTurn ();
					environmentInteract = true;
				}
				break;
			case 6:	//Defend Mode
				if(confirmed)
				{
					if(!defendMode)
					{
						//Hide Selection
						HideSelection ();

						if(objectCurrentSelection[0] == combatStats.PlayerIndex ())
						{
							//Calculate Resistance
							float defendRating = 2f;
							
							switch(elementalStance)
							{
							case 0:
								defendRating += combatStats.character.level / 100f; 
								break;
							case 1:	//If Earth
								defendRating += combatStats.character.earthAffinity / 100f; 
								print ("Defend with Earth");
								break;
							case 2:	//If Fire
								defendRating += combatStats.character.fireAffinity / 100f; 
								print ("Defend with Fire");
								break;
							case 3:	//If Lightning
								defendRating += combatStats.character.lightningAffinity / 100f; 
								print ("Defend with Lightning");
								break;
							case 4:	//If Water
								defendRating += combatStats.character.waterAffinity / 100f; 
								print ("Defend with Water");
								break;
							}
							
							//Activate and send defend message to the PlayerCombatCharacter Script
							combatStats.SetDefend(elementalStance, defendRating);
							
							//Move into the Defend Animation
							anim.SetInteger ("Attack Phase", attackPhase);
							anim.SetInteger ("Index", elementalStance);

							defendMode = true;

							//Set Global Message
							combatStats.ui.SetGlobalMessage ("Guard");
							combatStats.statusUI.protectedPanel.gameObject.SetActive (true);

							//Stop the camera
							CombatCamera.control.Stop ();
							CombatCamera.control.CameraReset ();

							EndTurnDelay (2f);
						}
						else
						{
							//Mark as Overwatch Protected
							//print ("PROVIDING OVERWATCH SIR");

							//Send Message to Overwatch Target
							//You cannot overwatch a target that is overwatching you
							if(combatStats.overwatchIndex != CombatManager.playerStats[objectCurrentSelection[0]].PlayerIndex())
							{
								//If you are already overwatching the target with the same element
								if(CombatManager.playerStats[objectCurrentSelection[0]].overwatchElement == elementalStance &&
								   CombatManager.playerStats[objectCurrentSelection[0]].overwatchIndex == combatStats.PlayerIndex ())
								{
									//Send cancel toggle order
									CombatManager.playerStats[objectCurrentSelection[0]].CancelOverwatchToggle ();

									print ("Set Cancel Overwatch Toggle");

									ReturnToIdle ();
									defendMode = false;
								}
								else
								{
									//Set as Overwatch Target
									CombatManager.playerStats[objectCurrentSelection[0]].SetOverwatch (combatStats.PlayerIndex(), 
								                                                                  	   elementalStance);

									//Create overwatch UI
									//CombatManager.playerStats[objectCurrentSelection[0]].statusUI.ActivateBuff (elementalStance + 3);

									defendMode = true;

									combatStats.ui.actionIntroPanel.SetTrigger ("Exit");

									//Set Global Message
									combatStats.ui.SetGlobalMessage ("Providing Overwatch to " + 
									                                 CombatManager.playerStats[objectCurrentSelection[0]].character.name);

									//Stop the camera
									CombatCamera.control.Stop ();
									CombatCamera.control.CameraReset ();
									EndTurnDelay (1f);
								}
							}
							else
							{
								//print ("Not allowed to overwatch an ally that is overwatching you");

								//Set Global Message
								combatStats.ui.SetGlobalMessage ("Can't provide Overwatch to a target that is protecting you");

								ReturnToIdle ();
								defendMode = false;
							}
						}
					}
				}
				else
				{
					if(!isSelecting)
					{
						//If not selecting and Submit buttons pressed then move to selection
						isSelecting = true;
						//Reveal Collider Trigger
						selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);

						//Show the Time of Action and Party Layout
						combatStats.ui.partyLayoutPanel.gameObject.SendMessage ("FadeTrigger", true, SendMessageOptions.DontRequireReceiver);
						combatStats.ui.timeOfAction.gameObject.SendMessage ("FadeTrigger", true, SendMessageOptions.DontRequireReceiver);
						//Hide the Action Description
						combatStats.ui.actionDescription.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);

						//Create Sequence 
						combatStats.ui.sequenceUI.CreateSequence ("Select");

						//Reveal Elemental Button Effect
						combatStats.ui.effectsUI[0 + 4].SetActive (true);
					}
					else
					{
						combatStats.ui.actionIntroPanel.ResetTrigger ("Exit");
						combatStats.ui.actionIntroPanel.ResetTrigger ("Confirm");

						//If chosen target then it is finally confirmed
						if(Input.GetButtonDown ("Submit") || Input.GetButtonDown ("Earth") || Input.GetMouseButtonDown (0)
						   || CombatManager.players.Count == 1)
						{
							ConfirmAction ();

							//EndTurnDelay (2f);

							//Clear UI Effects
							combatStats.ui.ClearUIEffects ();
						}

						if(Input.GetButtonDown ("Back"))
						{
							isSelecting = false;
							//Reveal Collider Trigger
							//selector.gameObject.SendMessage ("ColliderSwitch", false, SendMessageOptions.DontRequireReceiver);

							//Fade the Time of Action and Party Layout
							combatStats.ui.partyLayoutPanel.gameObject.SendMessage ("FadeTrigger", true, SendMessageOptions.DontRequireReceiver);
							combatStats.ui.timeOfAction.gameObject.SendMessage ("FadeTrigger", true, SendMessageOptions.DontRequireReceiver);
							//Show the Action Description
							combatStats.ui.actionDescription.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);
						
							//Go back to screen
							ReturnToIdle ();
							actionMode = 0;
							modeSelection = true;
							//combatStats.ui.actionIntroPanel.SetTrigger ("DefendMode");
							//combatStats.ui.actionIntroPanel.SetTrigger ("Exit");
							combatStats.ui.actionIntroPanel.SetTrigger ("Exit");
							//combatStats.ui.actionIntroPanel.ResetTrigger  ("Exit");
							//combatStats.ui.actionIntroPanel.SetTrigger ("Exit");
							//Clear UI Effects
							combatStats.ui.ClearUIEffects ();

							//Set Intro Camera
							IntroCameraSettings ();


							combatStats.ui.sequenceUI.ClearSequence ();
							combatStats.ui.sequenceUI.CreateSequence (CombatUIManager.mainPlayerScript.combatStats.character.name);
							GameObject.FindGameObjectWithTag("Combat Selector").SendMessage ("ColliderSwitch", false, SendMessageOptions.DontRequireReceiver);						
						}
					}
				}
				break;
			case 7:	//Support Mode
				if(confirmed)
				{
					if(!supportMode)
					{
						//Hide Selection
						HideSelection ();

						//Move into the Buff Animation
						anim.SetInteger ("Attack Phase", attackPhase);
						anim.SetInteger ("Index", 0);

						//Call the camera
						CombatCamera.control.SpawnGlobalAnimation (null, "View Battlefield", 400);

						supportMode = true;
						print ("Support");
					}
				}
				else
				{
					if(!isSelecting)
					{
						isSelecting = true;
						//Reveal Collider Trigger
						selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);

						//Show the Time of Action and Party Layout
						combatStats.ui.partyLayoutPanel.gameObject.SendMessage ("FadeTrigger", true, SendMessageOptions.DontRequireReceiver);
						combatStats.ui.timeOfAction.gameObject.SendMessage ("FadeTrigger", true, SendMessageOptions.DontRequireReceiver);
						//Hide the Action Description
						combatStats.ui.actionDescription.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);
					
						//Create Sequence 
						combatStats.ui.sequenceUI.CreateSequence ("Select");

						//Reveal Elemental Button Effect
						combatStats.ui.effectsUI[elementalStance - 1 + 4].SetActive (true);
					}
					else
					{
						//If chosen target then it is finally confirmed
						if(Input.GetButtonDown ("Submit") || Input.GetButtonDown ("Earth") || Input.GetMouseButtonDown (0)
						   || CombatManager.players.Count == 1)
						{
							ConfirmAction ();

							//Clear UI Effects
							combatStats.ui.ClearUIEffects ();
						}
						
						if(Input.GetButtonDown ("Back") || Input.GetButtonDown ("Fire"))
						{
							isSelecting = false;

							//Fade the Time of Action and Party Layout
							combatStats.ui.partyLayoutPanel.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);
							combatStats.ui.timeOfAction.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);
							//Show the Action Description
							combatStats.ui.actionDescription.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);

							//Go back to screen
							ReturnToIdle ();
							combatStats.ui.actionIntroPanel.SetTrigger ("SupportMode");

							//Return AP Cost
							if(elementalStance != 3)
							{
								combatStats.RegenAP (false, combatStats.elementalAPCost[elementalStance-1]);
							}
							
							//Destroy latest sequence
							combatStats.ui.sequenceUI.DestroyLatestSequence ();

							//Clear UI Effects
							combatStats.ui.ClearUIEffects ();

							//Set Intro Camera
							IntroCameraSettings ();
						}
					}
				}
				break;
			case 8:	//Curse Mode
				if(confirmed)
				{
					if(!curseMode)
					{
						//Hide Selection
						HideSelection ();

						//Move into the Debuff Animation
						anim.SetInteger ("Attack Phase", attackPhase);
						anim.SetInteger ("Index", 0);

						//Call the camera
						//CombatCamera.control.SpawnGlobalAnimation (null, "View Battlefield", 400);
						CombatCamera cam = CombatCamera.control; 
						cam.SetAnimated (anim.gameObject);
						cam.DelayStop (1.6f);
						cam.SetPosition(5);
						cam.SetHeight (1f);
						cam.SetMoveSpeed (0.1f);
						cam.increaseMoveSpeed = true;
						cam.Pedestal (-1);
						cam.SetDistance (0.9f);
						cam.SetFocus (CombatManager.enemies[objectCurrentSelection[1]]);

						curseMode = true;
						print ("Curse");
					}
				}
				else
				{
					if(!isSelecting)
					{
						isSelecting = true;
						//Reveal Collider Trigger
						selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);

						//Show the Time of Action and Party Layout
						combatStats.ui.partyLayoutPanel.gameObject.SendMessage ("FadeTrigger", true, SendMessageOptions.DontRequireReceiver);
						combatStats.ui.timeOfAction.gameObject.SendMessage ("FadeTrigger", true, SendMessageOptions.DontRequireReceiver);
						//Hide the Action Description
						combatStats.ui.actionDescription.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);
					
						//Create Sequence 
						combatStats.ui.sequenceUI.CreateSequence ("Select");

						//Reveal Elemental Button Effect
						combatStats.ui.effectsUI[elementalStance - 1 + 4].SetActive (true);
					}
					else
					{
						//If chosen target then it is finally confirmed
						if(Input.GetButtonDown ("Submit") || Input.GetButtonDown ("Earth") || Input.GetMouseButtonDown (0)
						   || CombatManager.enemies.Count == 1)
						{
							ConfirmAction ();

							//Clear UI Effects
							combatStats.ui.ClearUIEffects ();
						}
						
						if(Input.GetButtonDown ("Back") || Input.GetButtonDown ("Fire"))
						{
							isSelecting = false;
	
							//Fade the Time of Action and Party Layout
							combatStats.ui.partyLayoutPanel.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);
							combatStats.ui.timeOfAction.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);
							//Show the Action Description
							combatStats.ui.actionDescription.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);

							//Go back to screen
							ReturnToIdle ();
							combatStats.ui.actionIntroPanel.SetTrigger ("CurseMode");

							//Return AP Cost
							combatStats.RegenAP (false, combatStats.elementalAPCost[elementalStance-1]);
							
							//Destroy latest sequence
							combatStats.ui.sequenceUI.DestroyLatestSequence ();

							//Clear UI Effects
							combatStats.ui.ClearUIEffects ();

							//Set Intro Camera
							IntroCameraSettings ();
						}
					}
				}
				break; 
			}

			//Look At Target function 
			if(lookAtTarget)
			{
				//Rotate towards countdown 
				lookAtTargetTimer -= Time.deltaTime; 

				if(lookAtTargetTimer > 0f)
				{
					//Rotate towards target
					Vector3 targetDir = CombatManager.enemies[objectCurrentSelection[1]].transform.position - 
										anim.gameObject.transform.position;
					targetDir.y = 0;
					float step = rotateSpeed * Time.deltaTime;
					Vector3 newDir = Vector3.RotateTowards (anim.gameObject.transform.forward, targetDir, step, 0.0f);
					anim.gameObject.transform.rotation = Quaternion.LookRotation (newDir);
				}
				else
				{
					lookAtTarget = false;
					lookAtTargetTimer = 1f;
				}
			}

			if(lookAtInitialRotation)
			{
				anim.gameObject.transform.rotation = Quaternion.RotateTowards (anim.gameObject.transform.rotation,
				                                                      			transform.rotation, rotateSpeed);


				if(anim.gameObject.transform.localEulerAngles == Vector3.zero)
				{
					lookAtInitialRotation = false;
				}
			}

			//Move to Target Function
			if(moveToTarget)
			{
				//Calculate Distance
				if(CombatManager.enemies[objectCurrentSelection[1]])
				{
					float dist = Vector3.Distance (CombatManager.enemies[objectCurrentSelection[1]].transform.position,
					                               anim.gameObject.transform.position);
					if(dist > targetWidth/2f + 2f)
					{
						//Move Towards target
						float moveStep = moveSpeed * Time.deltaTime;
						anim.gameObject.transform.position = Vector3.MoveTowards (anim.gameObject.transform.position,
						                                                          CombatManager.enemies[objectCurrentSelection[1]].transform.position,
						                                                          moveStep);
					}
					else
					{
						moveToTarget = false;

						//if the move speed was a teleport than reveal self again
						if(moveSpeed >= 30f)
						{
							TeleportEffectToggle(false);
						}
					}
				}
				else
				{
					objectCurrentSelection[1] = 0;
				}
			}

			if(moveToInitialPosition)
			{
				//Move Towards target
				float moveStep = moveSpeed * Time.deltaTime;
				anim.gameObject.transform.position = Vector3.MoveTowards (anim.gameObject.transform.position,
				                                                          transform.position, moveStep);

				if(anim.gameObject.transform.localPosition == Vector3.zero)
				{
					moveToInitialPosition = false;

					//if the move speed was a teleport than reveal self again
					if(moveSpeed >= 30f)
					{
						TeleportEffectToggle(false);
					}
				}
			}
		}

		//If Scan Mode is On
		if(CombatUIManager.scanMode)
		{
			//Delay Input
			if(scanModeDelay <= 0f)
			{
				if(!scanModeActivate)
				{
					//If Left Mouse button is clicked - Submit 
					if(Input.GetMouseButtonUp(0))
					{
						//Send message to Combat UI Manager to retrieve scan values and initiate scan
						combatStats.ui.SubmitScan ();
						//Hide Selection
						HideSelection ();
						//Activate scanModeActivate to prevent input
						scanModeActivate = true;
						//Use the Trigger End to activate the Scan Animation 
						anim.SetInteger("Attack Phase", 0);
						anim.SetInteger ("Index", 0);	//Choose animation
						anim.SetTrigger ("End");
					}
					//If Right mouse button or escape is clicked - exit interface
					if(Input.GetMouseButtonUp (1) || Input.GetKeyUp (KeyCode.Escape))
					{
						//Send message to Combat UI Manager to exit scan mode
						combatStats.ui.ExitScan ();
						//Reset Delay
						scanModeDelay = 0.5f;
						//Show Selection
						ShowSelection();
						//Return to idle
						ReturnToIdle ();
						//Unlock the Action Modes
						actionModeLock = false;
					}
				}
			}
			else
			{
				//Countdown delay
				scanModeDelay -= Time.deltaTime;

				//Hide Selection
				HideSelection ();
			}
		}

		//At the very bottom of Update turn off elemental Buttons... The reason for this:
		//Using the button invoke function, elemental button functions get called in between Update and LateUpdate...
		TurnOffElementalButtons();
	}

	//This function is called after Update
	void LateUpdate()
	{
		//Turn off any button presses
		//TurnOffElementalButtons ();

		//Adding Extra Selection Controls
		if(selectDirection != 0)
		{
			selectDirection = 0;
		}
	}

	public void ReadyingAttack()
	{
		HideSelection (); //Hide Selector
		
		CombatCamera.control.Stop();
		
		readyTimer -= Time.deltaTime; 
		
		//Move to Single Shot
		if(readyTimer <= 0f)
		{
			//When Ready Timer is complete and ready, Determine whether a charge attack or single shot
			
			//If Still holding specific charge button after idle timer is completed
			//Move to Charge
			if(Input.GetKey(KeyCode.E) || elementalCharge[0])		//Earth Charge Mode
			{
				if(CanCharge())
				{
					elementalStance = 1;
					attackPhase = 4;
					anim.SetInteger ("Attack Phase", attackPhase);
					
					//Call the camera
					CombatCamera.control.SpawnGlobalAnimation (null, "View Battlefield", 400);
					
					//Set Global Message to Charge
					combatStats.ui.SetGlobalMessage ("Earth Charge");
					
					//Show Charging Effect
					currentCharge = Instantiate (chargeEffect[0], 
					                             chargeNode.position,
					                             chargeNode.rotation) as Transform;
					
					//Create UI - Charge Bar
					combatStats.ui.CreateChargeSlider(elementalStance);
					combatStats.ui.CreateLevelBars (levelBarLimit[elementalStance]);
					
					//Charging, in case when player finishes charging, cannot re charge during anim
					charging = true;
					//Calculate Max Charge with the levelbar limit
					chargeMax = levelBarLimit[elementalStance] * combatStats.elementalAPCost[0];
					
					elementalCharge[0] = true;
				}
				else
				{
					ReturnToIdle ();
				}
			}
			else if(Input.GetKey(KeyCode.Q) || elementalCharge[1])	//Fire Charge Mode
			{
				if(CanCharge ())
				{
					elementalStance = 2;
					attackPhase = 4;
					anim.SetInteger ("Attack Phase", attackPhase);
					
					//Call the camera
					CombatCamera.control.SpawnGlobalAnimation (null, "View Battlefield", 400);
					
					//Set Global Message to Charge
					combatStats.ui.SetGlobalMessage ("Fire Charge");
					
					//Show Charging Effect
					currentCharge = Instantiate (chargeEffect[1], 
					                             chargeNode.position,
					                             chargeNode.rotation) as Transform;
					
					//Create UI - Charge Bar
					combatStats.ui.CreateChargeSlider(elementalStance);
					combatStats.ui.CreateLevelBars (levelBarLimit[elementalStance]);
					
					//Charging, in case when player finishes charging, cannot re charge during anim
					charging = true;
					//Calculate Max Charge with the levelbar limit
					chargeMax = levelBarLimit[elementalStance] * combatStats.elementalAPCost[1];
					
					elementalCharge[1] = true;
				}
				else
				{
					ReturnToIdle ();
				}
			}
			else if(Input.GetKey(KeyCode.R) || elementalCharge[2])	//Lightning Charge Mode
			{
				if(CanCharge ())
				{
					elementalStance = 3;
					attackPhase = 4;
					anim.SetInteger ("Attack Phase", attackPhase);
					
					//Call the camera
					CombatCamera.control.SpawnGlobalAnimation (null, "View Battlefield", 400);
					
					//Set Global Message to Charge
					combatStats.ui.SetGlobalMessage ("Lightning Charge");
					
					//Show Charging Effect
					currentCharge = Instantiate (chargeEffect[2], 
					                             chargeNode.position,
					                             chargeNode.rotation) as Transform;
					
					//Create UI - Charge Bar
					combatStats.ui.CreateChargeSlider(elementalStance);
					combatStats.ui.CreateLevelBars (levelBarLimit[elementalStance]);
					
					//Charging, in case when player finishes charging, cannot re charge during anim
					charging = true;
					//Calculate Max Charge with the levelbar limit
					chargeMax = levelBarLimit[elementalStance] * combatStats.elementalAPCost[2];
					
					elementalCharge[2] = true;
				}
				else
				{
					ReturnToIdle ();
				}
			}
			else if(Input.GetKey(KeyCode.W) || elementalCharge[3])	//Water Charge Mode
			{
				if(CanCharge ())
				{
					elementalStance = 4;
					attackPhase = 4;
					anim.SetInteger ("Attack Phase", attackPhase);
					
					//Call the camera
					CombatCamera.control.SpawnGlobalAnimation (null, "View Battlefield", 400);
					
					//Set Global Message to Charge
					combatStats.ui.SetGlobalMessage ("Water Charge");
					
					//Show Charging Effect
					currentCharge = Instantiate (chargeEffect[3], 
					                             chargeNode.position,
					                             chargeNode.rotation) as Transform;
					
					//Create UI - Charge Bar
					combatStats.ui.CreateChargeSlider(elementalStance);
					combatStats.ui.CreateLevelBars (levelBarLimit[elementalStance]);
					
					//Charging, in case when player finishes charging, cannot re charge during anim
					charging = true;
					//Calculate Max Charge with the levelbar limit
					chargeMax = levelBarLimit[elementalStance] * combatStats.elementalAPCost[3];
					
					elementalCharge[3] = true;
				}
				else
				{
					ReturnToIdle ();
				}
			}
			else
			{
				//Move to Single Shot Attack Mode
				attackPhase = 2; //Single Shot
				singleShot = true;
				
				//Call the camera
				CombatCamera.control.SpawnGlobalAnimation (null, "View Battlefield", 400);
				
				//Set Global Message
				combatStats.ui.SetGlobalMessage ("Single Shot");
			}
		}
		
		
		//Move to a Flurry
		if(!flurryActivate)
		{
			if(Input.GetKey(KeyCode.Q)|| Input.GetKey(KeyCode.W)||
			   Input.GetKey(KeyCode.E)|| Input.GetKey(KeyCode.R)||
			   elementalButton[0]|| elementalButton[1]||
			   elementalButton[2]|| elementalButton[3])
			{
				flurryActivate = true;
				
				//Store the first flurry element
				//Create button storage of elements array and assign first button element
				flurryNextElements = new int[levelBarLimit[0]];
				currentLevelBar = 1;
				flurryNextElements[0] = elementalStance;
			}
		}
		else
		{
			//Initialise the Flurry Attack Mode
			
			//Teleport to target and hit with specified element
			if(Input.GetKeyDown (KeyCode.E) || elementalButton[0])
			{
				if(CanFlurry(0))
				{
					attackPhase = 3;	//Flurry Attack Mode Animation State
					anim.SetInteger ("Attack Phase", attackPhase); //Play Flurry Animation
					
					//Set Flurry Camera
					FlurryCameraSettings ();
					
					//Set Global Message
					combatStats.ui.SetGlobalMessage ("Flurry");
					
					//Teleport to enemy
					MoveToTarget (teleportDashSpeed);
					LookAtTarget (teleportRotateSpeed);
					
					//Create button storage of elements array and assign first button element
					flurryCurrentButton = 1;
					
					//Create flurry UI Level Bars
					combatStats.ui.CreateLevelBars (levelBarLimit[0]);
					combatStats.ui.ActivateLevelBar (flurryNextElements[0], currentLevelBar);
					
					currentLevelBar = 2;	//Move up the current bar and activate the next
					flurryNextElements[1] = 1;
					combatStats.ui.ActivateLevelBar (flurryNextElements[1], currentLevelBar);
					
					
					//AP Cost
					combatStats.APCost (combatStats.elementalAPCost[0]);
				}
				else
				{
					ReturnToIdle ();
				}
				
			}
			else if(Input.GetKeyDown (KeyCode.Q) || elementalButton[1])
			{
				if(CanFlurry (1))
				{
					attackPhase = 3;
					anim.SetInteger ("Attack Phase", attackPhase);
					
					//Set Flurry Camera
					FlurryCameraSettings ();
					
					//Set Global Message
					combatStats.ui.SetGlobalMessage ("Flurry");
					
					//Teleport to enemy
					MoveToTarget (teleportDashSpeed);
					LookAtTarget (teleportRotateSpeed);
					
					//Create button storage of elements array and assign first button element
					flurryCurrentButton = 1;
					
					//Create flurry UI Level Bars
					combatStats.ui.CreateLevelBars (levelBarLimit[0]);
					combatStats.ui.ActivateLevelBar (flurryNextElements[0], currentLevelBar);
					
					currentLevelBar = 2;	//Move up the current bar and activate the next
					flurryNextElements[1] = 2;
					combatStats.ui.ActivateLevelBar (flurryNextElements[1], currentLevelBar);
					
					//AP Cost
					combatStats.APCost (combatStats.elementalAPCost[1]);
				}
				else
				{
					ReturnToIdle ();
				}
			}
			else if(Input.GetKeyDown (KeyCode.R) || elementalButton[2])
			{
				if(CanFlurry (2))
				{
					attackPhase = 3;
					anim.SetInteger ("Attack Phase", attackPhase);
					
					//Set Flurry Camera
					FlurryCameraSettings ();
					
					//Set Global Message
					combatStats.ui.SetGlobalMessage ("Flurry");
					
					//Teleport to enemy
					MoveToTarget (teleportDashSpeed);
					LookAtTarget (teleportRotateSpeed);
					
					//Create button storage of elements array and assign first button element
					flurryCurrentButton = 1;
					
					//Create flurry UI Level Bars
					combatStats.ui.CreateLevelBars (levelBarLimit[0]);
					combatStats.ui.ActivateLevelBar (flurryNextElements[0], currentLevelBar);
					
					currentLevelBar = 2;	//Move up the current bar and activate the next
					flurryNextElements[1] = 3;
					combatStats.ui.ActivateLevelBar (flurryNextElements[1], currentLevelBar);
					
					//AP Cost
					combatStats.APCost (combatStats.elementalAPCost[2]);
				}
				else
				{
					ReturnToIdle ();
				}
			}
			else if(Input.GetKeyDown (KeyCode.W) || elementalButton[3])
			{
				if(CanFlurry (3))
				{
					attackPhase = 3;
					anim.SetInteger ("Attack Phase", attackPhase);
					
					//Set Flurry Camera
					FlurryCameraSettings ();
					
					//Set Global Message
					combatStats.ui.SetGlobalMessage ("Flurry");
					
					//Teleport to enemy
					MoveToTarget (teleportDashSpeed);
					LookAtTarget (teleportRotateSpeed);
					
					//Create button storage of elements array and assign first button element
					flurryCurrentButton = 1;
					
					//Create flurry UI Level Bars
					combatStats.ui.CreateLevelBars (levelBarLimit[0]);
					combatStats.ui.ActivateLevelBar (flurryNextElements[0], currentLevelBar);
					
					currentLevelBar = 2;	//Move up the current bar and activate the next
					flurryNextElements[1] = 4;
					combatStats.ui.ActivateLevelBar (flurryNextElements[1], currentLevelBar);
					
					//AP Cost
					combatStats.APCost (combatStats.elementalAPCost[3]);
				}
				else
				{
					ReturnToIdle ();
				}
			}
		}
	}
	
	public void SingleShotAttack()
	{
		if(confirmed)
		{
			if(singleShot)
			{
				print ("Attack");
				anim.SetInteger ("Index", 0); //*For now using 0 as variation*
				anim.SetInteger ("Attack Phase", attackPhase);

				//AP Cost
				combatStats.APCost (combatStats.elementalAPCost[elementalStance-1]);

				isSelecting = false;
				
				singleShot = false;
			}
		}
		else
		{
			if(!isSelecting)
			{
				//If not selecting and Submit buttons pressed then move to selection
				if(Input.GetButtonUp ("Submit") || Input.GetButtonUp ("Earth") || Input.GetMouseButtonUp (0))
				{
					isSelecting = true;
					//Reveal Collider Trigger
					selector.gameObject.SendMessage ("ColliderSwitch", true, SendMessageOptions.DontRequireReceiver);
					
					//Clear the level bars
					combatStats.ui.HideLevelBars ();

					//Show the Time of Action and Party Layout
					combatStats.ui.partyLayoutPanel.gameObject.SendMessage ("FadeTrigger", true, SendMessageOptions.DontRequireReceiver);
					combatStats.ui.timeOfAction.gameObject.SendMessage ("FadeTrigger", true, SendMessageOptions.DontRequireReceiver);
					//Hide the Action Description
					combatStats.ui.actionDescription.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);
				}
				
				if(Input.GetButtonUp ("Back") || Input.GetButtonUp ("Fire"))
				{
					//Go back to screen
					ReturnToIdle ();
					combatStats.ui.actionIntroPanel.SetTrigger ("DefendMode");
					
					//Clear the level bars
					combatStats.ui.ClearLevelBars ();
				}
			}
			else
			{
				//If chosen target then it is finally confirmed
				if(Input.GetButtonUp ("Submit") || Input.GetButtonUp ("Earth") || Input.GetMouseButtonUp (0))
				{
					ConfirmAction ();
					singleShot = true;

					//Clear the level bars
					combatStats.ui.ClearLevelBars ();

					//Set Single Shot Camera
					SingleShotCameraSettings ();
				}
				
				if(Input.GetButtonUp ("Back") || Input.GetButtonUp ("Fire"))
				{
					isSelecting = false;
					//Reveal Collider Trigger
					selector.gameObject.SendMessage ("ColliderSwitch", false, SendMessageOptions.DontRequireReceiver);
					//Go back to screen

					//Re show action confirm bar
					combatStats.ui.RevealLevelBars ();

					//Fade the Time of Action and Party Layout
					combatStats.ui.partyLayoutPanel.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);
					combatStats.ui.timeOfAction.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);
					//Show the Action Description
					combatStats.ui.actionDescription.gameObject.SendMessage ("FadeTrigger", true, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
	
	public void FlurryAttack()
	{
		//The Flurry Attack Mode has been activated and this is where the flurry code works in
		if(flurryReady)
		{
			//print ("Surprise muthafucka");
			flurryTimer -= Time.deltaTime; //Flurry Timer Countdown
			
			if(flurryTimer > 0f && flurryCurrentButton < currentLevelBar)
			{
				elementalStance = flurryNextElements[flurryCurrentButton];
				anim.SetTrigger("Next"); //Move to next Chain Animation
				flurryReady = false;
				flurryTimer = flurryMaxTimer;
				
				DetermineFlurryTeleport (); 	//Calculate if character needs to be teleported
				
				//Increase current bar storage
				flurryCurrentButton++;
				
				//print ("The Current Element is" + elementalStance + "in "+ flurryCurrentButton);
			}
			else 
			{
				//If flurry is finished, return to position and end the turn
				//Teleporting back to Initial Position will be called by animation clip
				//End turn will be called in the finishing flurry animation
				anim.SetTrigger ("End"); //End the chain
				combatStats.ui.ClearLevelBars ();
				flurryReady = false;
			}
		}
		
		//Store and acquire Button presses
		if(currentLevelBar < levelBarLimit[0])
		{
			if(Input.GetKeyDown (KeyCode.E) || elementalButton[0])	//If earth
			{
				if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[0])
				{
					flurryNextElements[currentLevelBar] = 1;
					
					//Activate Next Level Bar
					currentLevelBar ++;
					combatStats.ui.ActivateLevelBar (1, currentLevelBar);
					
					//AP Cost
					combatStats.APCost (combatStats.elementalAPCost[0]);
				}
			}
			else if(Input.GetKeyDown (KeyCode.Q) || elementalButton[1])	//if fire
			{
				if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[1])
				{
					flurryNextElements[currentLevelBar] = 2;
					
					//Activate Next Level Bar
					currentLevelBar ++;
					combatStats.ui.ActivateLevelBar (2, currentLevelBar);
					
					//AP Cost
					combatStats.APCost (combatStats.elementalAPCost[1]);
				}
			}
			else if(Input.GetKeyDown (KeyCode.R) || elementalButton[2])	//If Lightning
			{
				if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[2])
				{
					flurryNextElements[currentLevelBar] = 3;
					
					//Activate Next Level Bar
					currentLevelBar ++;
					combatStats.ui.ActivateLevelBar (3, currentLevelBar);
					
					//AP Cost
					combatStats.APCost (combatStats.elementalAPCost[2]);
				}
			}
			else if(Input.GetKeyDown (KeyCode.W) || elementalButton[3])	//If Water
			{
				if(combatStats.stat.actionPoints >= combatStats.elementalAPCost[3])
				{
					flurryNextElements[currentLevelBar] = 4;
					
					//Activate Next Level Bar
					currentLevelBar ++;
					combatStats.ui.ActivateLevelBar (4, currentLevelBar);
					
					//AP Cost
					combatStats.APCost (combatStats.elementalAPCost[3]);
				}
			}
		}
		
		//Gather Direction input when the player wants to flurry the next enemy etc
		if(selectDirection == -1)
		{
			//If flurry is a positive number, reset to 0
			if(flurryNextEnemy > 0)
			{
				flurryNextEnemy = 0;
			}
			
			flurryNextEnemy --; 
		}
		if(selectDirection == 1)
		{
			//If flurry is a negative number, reset to 0
			if(flurryNextEnemy < 0)
			{
				flurryNextEnemy = 0;
			}
			
			flurryNextEnemy ++;
		}

	}
	
	public void ChargeAttack()
	{
		if(charging)
		{
			switch(elementalStance)
			{
			case 1:	//if Charging with Earth
				if(Input.GetKey(KeyCode.E) || elementalCharge[0])
				{
					//Increase Charge and decrease AP
					float chargeCalculations = Time.deltaTime * chargeSpeed;
					charge += chargeCalculations;
					chargeBarCount += chargeCalculations;
					chargeAPCount += chargeCalculations;
					
					//Activate Level Bar when Ap cost reached
					if(chargeBarCount >= combatStats.elementalAPCost[0])
					{
						chargeCurrentBar++;
						combatStats.ui.ActivateLevelBar (elementalStance, chargeCurrentBar);
						anim.SetInteger ("Index", chargeCurrentBar);	//Next Stance
						chargeBarCount = 0f;
					}
					
					//Drain AP
					if(chargeAPCount >= 1f)
					{
						combatStats.APCost (1);
						chargeAPCount = 0f;
					}
					
					combatStats.stat.actionPoints -= (int)chargeCalculations;
					//Charge Slider will equal charge 
					combatStats.ui.chargeSlider.value = (float)(charge / chargeMax);
				}
				
				//If button released or Charge is over Max charge or if no AP
				if(Input.GetKeyUp (KeyCode.E) || !elementalCharge[0] ||
				   combatStats.stat.actionPoints <= 0 ||
				   charge >= chargeMax)
				{
					ElementalChargeCancel (); 
					
					//Activate the Charged Attack
					anim.SetTrigger ("Next");
					
					//Destroy the Charge Level UI Bar
					combatStats.ui.ClearChargeSlider ();
					
					//Determine whether in critical zone
					chargeInstantCritical = DetermineChargeCritical ();
					
					//Turn off Charging
					charging = false;
				}
				break;
			case 2:	//if Charging with Fire
				if(Input.GetKey(KeyCode.Q) || elementalCharge[1])
				{
					//Increase Charge and decrease AP
					float chargeCalculations = Time.deltaTime * chargeSpeed;
					charge += chargeCalculations;
					chargeBarCount += chargeCalculations;
					chargeAPCount += chargeCalculations;
					
					//Activate Level Bar when Ap cost reached
					if(chargeBarCount >= combatStats.elementalAPCost[1])
					{
						chargeCurrentBar++;
						combatStats.ui.ActivateLevelBar (elementalStance, chargeCurrentBar);
						anim.SetInteger ("Index", chargeCurrentBar);	//Next Stance
						chargeBarCount = 0f;
					}
					
					//Drain AP
					if(chargeAPCount >= 1f)
					{
						combatStats.APCost (1);
						chargeAPCount = 0f;
					}
					
					combatStats.stat.actionPoints -= (int)chargeCalculations;
					//Charge Slider will equal charge 
					combatStats.ui.chargeSlider.value = (float)(charge / chargeMax);
				}
				
				//If button released or Charge is over Max charge or if no AP
				if(Input.GetKeyUp (KeyCode.Q) || !elementalCharge[1] ||
				   combatStats.stat.actionPoints <= 0 ||
				   charge >= chargeMax)
				{
					ElementalChargeCancel (); 
					
					//Activate the Charged Attack
					anim.SetTrigger ("Next");
					
					//Destroy the Charge Level UI Bar
					combatStats.ui.ClearChargeSlider ();
					
					//Determine whether in critical zone
					chargeInstantCritical = DetermineChargeCritical ();
					
					//Turn off Charging
					charging = false;
				}
				break;
			case 3:	//if Charging with Lightning
				if(Input.GetKey(KeyCode.R) || elementalCharge[2])
				{
					//Increase Charge and decrease AP
					float chargeCalculations = Time.deltaTime * chargeSpeed;
					charge += chargeCalculations;
					chargeBarCount += chargeCalculations;
					chargeAPCount += chargeCalculations;
					
					//Activate Level Bar when Ap cost reached
					if(chargeBarCount >= combatStats.elementalAPCost[2])
					{
						chargeCurrentBar++;
						combatStats.ui.ActivateLevelBar (elementalStance, chargeCurrentBar);
						anim.SetInteger ("Index", chargeCurrentBar);	//Next Stance
						chargeBarCount = 0f;
					}
					
					//Drain AP
					if(chargeAPCount >= 1f)
					{
						combatStats.APCost (1);
						chargeAPCount = 0f;
					}
					
					combatStats.stat.actionPoints -= (int)chargeCalculations;
					//Charge Slider will equal charge 
					combatStats.ui.chargeSlider.value = (float)(charge / chargeMax);
				}
				
				//If button released or Charge is over Max charge or if no AP
				if(Input.GetKeyUp (KeyCode.R) || !elementalCharge[2] ||
				   combatStats.stat.actionPoints <= 0 ||
				   charge >= chargeMax)
				{
					ElementalChargeCancel (); 
					
					//Activate the Charged Attack
					anim.SetTrigger ("Next");
					
					//Destroy the Charge Level UI Bar
					combatStats.ui.ClearChargeSlider ();
					
					//Determine whether in critical zone
					chargeInstantCritical = DetermineChargeCritical ();
					
					//Turn off Charging
					charging = false;
				}
				break;
			case 4:	//if Charging with Water
				if(Input.GetKey(KeyCode.W) || elementalCharge[3])
				{
					//Increase Charge and decrease AP
					float chargeCalculations = Time.deltaTime * chargeSpeed;
					charge += chargeCalculations;
					chargeBarCount += chargeCalculations;
					chargeAPCount += chargeCalculations;
					
					//Activate Level Bar when Ap cost reached
					if(chargeBarCount >= combatStats.elementalAPCost[3])
					{
						chargeCurrentBar++;
						combatStats.ui.ActivateLevelBar (elementalStance, chargeCurrentBar);
						anim.SetInteger ("Index", chargeCurrentBar);	//Next Stance
						chargeBarCount = 0f;
					}
					
					//Drain AP
					if(chargeAPCount >= 1f)
					{
						combatStats.APCost (1);
						chargeAPCount = 0f;
					}
					
					combatStats.stat.actionPoints -= (int)chargeCalculations;
					//Charge Slider will equal charge 
					combatStats.ui.chargeSlider.value = (float)(charge / chargeMax);
				}
				
				//If button released or Charge is over Max charge or if no AP
				if(Input.GetKeyUp (KeyCode.W) || !elementalCharge[3]||
				   combatStats.stat.actionPoints <= 0 ||
				   charge >= chargeMax)
				{
					ElementalChargeCancel (); 
					
					//Activate the Charged Attack
					anim.SetTrigger ("Next");
					
					//Destroy the Charge Level UI Bar
					combatStats.ui.ClearChargeSlider ();
					
					//Determine whether in critical zone
					chargeInstantCritical = DetermineChargeCritical ();
					
					//Turn off Charging
					charging = false;
				}
				break;
			}
		}
	}
	
	void ControlSelection()
	{
		//-------------------------------------Selection------------------------------------------
		if(isSelecting)
		{
			switch(objectSelection)
			{
			case 0:	//Player Selection
				if(selectDirection == -1)
				{
					objectCurrentSelection[0]--; 
					
					if(objectCurrentSelection[0] < 0)
					{
						objectCurrentSelection[0] = CombatManager.players.Count - 1;
					}
					
					selector.position = CombatManager.players[objectCurrentSelection[0]].transform.position;
				}
				
				if(selectDirection == 1)
				{
					objectCurrentSelection[0]++; 
					
					if(objectCurrentSelection[0] >= CombatManager.players.Count)
					{
						objectCurrentSelection[0] = 0;
					}			
					
					selector.position = CombatManager.players[objectCurrentSelection[0]].transform.position;
				}
				break;
			case 1:	//Enemy Selection
				if(selectDirection == -1)
				{
					objectCurrentSelection[1]--; 
					
					if(objectCurrentSelection[1] < 0)
					{
						if(CombatManager.environmentObjects.Count != 0)
						{
							//Move to Environment Selection
							objectSelection = 2;
							objectCurrentSelection[2] = CombatManager.environmentObjects.Count - 1;
							selector.position = CombatManager.environmentObjects[objectCurrentSelection[2]].transform.position;
						}
						else
						{
							objectCurrentSelection[1] = CombatManager.enemies.Count -1;
							selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
						}
					}
					else
					{						
						selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
					}
				}
				
				if(selectDirection == 1)
				{
					objectCurrentSelection[1]++; 
					
					if(objectCurrentSelection[1] >= CombatManager.enemies.Count)
					{
						if(CombatManager.environmentObjects.Count != 0)
						{
							//Move to Environment Selection
							objectSelection = 2;
							objectCurrentSelection[2] = 0;
							selector.position = CombatManager.environmentObjects[objectCurrentSelection[2]].transform.position;
						}
						else
						{
							objectCurrentSelection[1] = 0;
							selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
						}
					}		
					else
					{
						selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
					}
				}
				break;
			case 2:	//Environment Selection
				if(selectDirection == -1)
				{
					objectCurrentSelection[2]--; 
					
					if(objectCurrentSelection[2] < 0)
					{
						//Move to Environment Selection
						objectSelection = 1;
						objectCurrentSelection[1] = CombatManager.enemies.Count - 1;
						selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
					}
					else
					{						
						selector.position = CombatManager.environmentObjects[objectCurrentSelection[2]].transform.position;
					}
				}
				
				if(selectDirection == 1)
				{
					objectCurrentSelection[2]++; 
					
					if(objectCurrentSelection[2] >= CombatManager.environmentObjects.Count)
					{
						//Move to Environment Selection
						objectSelection = 1;
						objectCurrentSelection[1] = 0;
						selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
					}		
					else
					{
						selector.position = CombatManager.environmentObjects[objectCurrentSelection[2]].transform.position;
					}
				}
				break;
			}
		}
	}

	public void ConfirmAction()
	{
		confirmed = true;

		//Play Confirmation Sound
		combatStats.ui.confirmSound.PlayOneShot(combatStats.ui.confirmAudioClip);

		//Clear Sequence
		combatStats.ui.sequenceUI.ClearSequence ();

		//Update AP UI
		combatStats.ui.UpdateAPBar ();

		//Update Element Exp
		combatStats.UpdateElementExp (elementalStance);
		//print (elementalStance);
	}

	//This Procedure is called when all elemental buttons need to be turned off
	void TurnOffElementalButtons()
	{
		//Turn off any button presses
		for(int i = 0; i < elementalButton.Length; i++)
		{
			if(elementalButton[i] == true)
			{
				elementalButton[i] = false;
			}
		}
	}

	//This procedure is called when an elemental UI button is pressed
	public void ElementalTrigger(int _element)
	{
		elementalButton[_element] = true;
		//Invoke ("TurnOffElementalButtons",0.1f);
		//print (elementalButton[_element]);
	}

	//This procedure is called when an elemental ui button is held down
	public void ElementalChargeTrigger(int _element)
	{
		//elementalButton[_element] = true;
		elementalCharge[_element] = true;
	}

	public void ElementalChargeCancel()
	{
		for(int i = 0; i < elementalCharge.Length; i++)
		{
			if(elementalCharge[i] == true)
			{
				elementalCharge[i] = false;
			}
		}
	}

	//This function is to fire the single shot projectile, activated by the animator
	public void FireSingleShot(int _projectileSlot)
	{
		//Fire
		if(elementalStance == 2)
		{
			//Calculate damage
			float damage = (((float)combatStats.character.fireAffinity / 100f) + 1f) * combatStats.stat.attack;

			//Spawn Projectile and specify information to projectile
			Transform projectile = Instantiate (singleShotProjectiles[elementalStance-1],
			                                    projectileNode[_projectileSlot-1].position, 
			                                    projectileNode[_projectileSlot-1].rotation) as Transform;
			//Calculate status chance
			float statusChance =  CalculateStatusChance(singleShotStatusChance, combatStats.character.fireAffinity, 2f);
			//Get the targets script component and set target
			CombatProjectile trajectory = projectile.gameObject.GetComponent<CombatProjectile>();
			trajectory.SetTarget (combatStats.stat, CombatManager.enemies[objectCurrentSelection[1]], (int)damage, false, 
			                      statusChance, singleShotCritChance);
		}
		
		//Water
		if(elementalStance == 4)
		{
			//Calculate damage
			float damage = (((float)combatStats.character.waterAffinity / 100f) + 1f) * combatStats.stat.attack;
			
			//Spawn Projectile and specify information to projectile
			Transform projectile = Instantiate (singleShotProjectiles[elementalStance-1],
			                                    projectileNode[_projectileSlot-1].position, 
			                                    projectileNode[_projectileSlot-1].rotation) as Transform;
			//Calculate status chance
			float statusChance =  CalculateStatusChance(singleShotStatusChance, combatStats.character.waterAffinity, 2f);
			//Get the targets script component and set target
			CombatProjectile trajectory = projectile.gameObject.GetComponent<CombatProjectile>();
			trajectory.SetTarget (combatStats.stat, CombatManager.enemies[objectCurrentSelection[1]], (int)damage, false, 
			                     statusChance, singleShotCritChance);
		}
		
		//Earth 
		if(elementalStance == 1)
		{
			//Calculate damage
			float damage = (((float)combatStats.character.earthAffinity / 100f) + 1f) * combatStats.stat.attack;
			
			//Spawn Projectile and specify information to projectile
			Transform projectile = Instantiate (singleShotProjectiles[elementalStance-1],
			                                    projectileNode[_projectileSlot-1].position, 
			                                    projectileNode[_projectileSlot-1].rotation) as Transform;
			//Calculate status chance
			float statusChance =  CalculateStatusChance(singleShotStatusChance, combatStats.character.earthAffinity, 2f);
			//Get the targets script component and set target
			CombatProjectile trajectory = projectile.gameObject.GetComponent<CombatProjectile>();
			trajectory.SetTarget (combatStats.stat, CombatManager.enemies[objectCurrentSelection[1]], (int)damage, false, 
			                      statusChance, singleShotCritChance);
		}
		
		//Lightning
		if(elementalStance == 3)
		{
			//Calculate damage
			float damage = (((float)combatStats.character.lightningAffinity / 100f) + 1f) * combatStats.stat.attack;
			
			//Spawn Projectile and specify information to projectile
			Transform projectile = Instantiate (singleShotProjectiles[elementalStance-1],
			                                    projectileNode[_projectileSlot-1].position, 
			                                    projectileNode[_projectileSlot-1].rotation) as Transform;
			//Calculate status chance
			float statusChance =  CalculateStatusChance(singleShotStatusChance, combatStats.character.lightningAffinity, 2f);
			//Get the targets script component and set target
			CombatProjectile trajectory = projectile.gameObject.GetComponent<CombatProjectile>();
			trajectory.SetTarget (combatStats.stat, CombatManager.enemies[objectCurrentSelection[1]], (int)damage, false, 
			                      statusChance, singleShotCritChance);
		}
	}

	public void SetFlurryDamage()
	{
		//Special Note: The accuracy stat will be temporarily increased when using flurry (x2)
		combatStats.stat.accuracy += (int)((float)combatStats.stat.accuracyBase * (float)flurryAccuracy);

		//Calculate damage and status chance
		float damage = 0f;
		float statusChance = 0f;

		//Fire
		if(elementalStance == 2)
		{
			//Calculate damage: for Flurry the damage will be halved
			damage = (((float)combatStats.character.fireAffinity / 100f) + 1f) * (combatStats.stat.attack / 2f);

			//Calculate status chance
			statusChance =  CalculateStatusChance(flurryStatusChance, combatStats.character.fireAffinity, 1f);

			//Damage is increased by the AP Cost for increased upgradability
			damage *= 1 + ((float)combatStats.elementalAPCost[1] / 100f);
		}
		
		//Water
		if(elementalStance == 4)
		{
			//Calculate damage
			damage = (((float)combatStats.character.waterAffinity / 100f) + 1f) * (combatStats.stat.attack / 2f);

			//Calculate status chance
			statusChance =  CalculateStatusChance(flurryStatusChance, combatStats.character.waterAffinity, 1f);

			//Damage is increased by the AP Cost for increased upgradability
			damage *= 1 + ((float)combatStats.elementalAPCost[3] / 100f);
		}
		
		//Earth 
		if(elementalStance == 1)
		{
			//Calculate damage
			damage = (((float)combatStats.character.earthAffinity / 100f) + 1f) * (combatStats.stat.attack / 2f);

			//Calculate status chance
			statusChance =  CalculateStatusChance(flurryStatusChance, combatStats.character.earthAffinity, 1f);

			//Damage is increased by the AP Cost for increased upgradability
			damage *= 1 + ((float)combatStats.elementalAPCost[0] / 100f);
		}
		
		//Lightning
		if(elementalStance == 3)
		{
			//Calculate damage
			damage = (((float)combatStats.character.lightningAffinity / 100f) + 1f) * (combatStats.stat.attack / 2f);

			//Calculate status chance
			statusChance =  CalculateStatusChance(flurryStatusChance, combatStats.character.lightningAffinity, 1f);

			//Damage is increased by the AP Cost for increased upgradability
			damage *= 1 + ((float)combatStats.elementalAPCost[2] / 100f);
		}

		//Instantiate Sound Effect
		Instantiate (flurrySound[elementalStance - 1], anim.transform.position, anim.transform.rotation);

		CombatManager.enemyStats[objectCurrentSelection[1]].SetDamage (combatStats.stat, elementalStance, (int) damage,
		                                               statusChance, flurryCritChance);

		//Accuracy is restored
		combatStats.stat.accuracy -= (int)((float)combatStats.stat.accuracyBase * (float)flurryAccuracy);
	}

	public void SetChargeDamage()
	{
		//Special Note: The accuracy stat will be temporarily increased when using charged (x2)
		combatStats.stat.accuracy += (int)((float)combatStats.stat.accuracyBase * (float)chargeAccuracy);

		//Calculate damage and status chance
		float damage = 0f;
		float statusChance = 0f;
		
		//Fire
		if(elementalStance == 2)
		{
			//Calculate damage: for charged, depending on how many charge is multiplied to the damage / 10f 
			damage = (((float)combatStats.character.fireAffinity / 100f) + 1f) * 
				((float)combatStats.stat.attack * (charge / 10f));
			
			//Calculate status chance
			statusChance =  CalculateStatusChance(chargeStatusChance, combatStats.character.fireAffinity, 2f);
		}
		
		//Water
		if(elementalStance == 4)
		{
			//Calculate damage
			damage = (((float)combatStats.character.waterAffinity / 100f) + 1f) * 
				((float)combatStats.stat.attack * (charge / 10f));
			
			//Calculate status chance
			statusChance =  CalculateStatusChance(chargeStatusChance, combatStats.character.waterAffinity, 2f);
		}
		
		//Earth 
		if(elementalStance == 1)
		{
			//Calculate damage
			damage = (((float)combatStats.character.earthAffinity / 100f) + 1f) * 
				((float)combatStats.stat.attack * (charge / 10f));
			
			//Calculate status chance
			statusChance =  CalculateStatusChance(chargeStatusChance, combatStats.character.earthAffinity, 2f);
		}
		
		//Lightning
		if(elementalStance == 3)
		{
			//Calculate damage
			damage = (((float)combatStats.character.lightningAffinity / 100f) + 1f) * 
				((float)combatStats.stat.attack * (charge / 10f));
			
			//Calculate status chance
			statusChance =  CalculateStatusChance(chargeStatusChance, combatStats.character.lightningAffinity, 2f);
		}

		//Decide whether critical hit, during charge the player may let go at the correct moment and score an
		//instant critical
		float criticalHitChance = 0f;

		if(chargeInstantCritical)
		{
			criticalHitChance = 100f; 
			print ("ITS AN INSTANT CRITICAL");
		}
		else
		{
			criticalHitChance = chargeCritChance;
		}

		//Get the targets script component and set target
		CombatProjectile trajectory = currentCharge.gameObject.GetComponent<CombatProjectile>();

		trajectory.SetTarget (combatStats.stat, CombatManager.enemies[objectCurrentSelection[1]], (int)damage, false, 
		                      statusChance, criticalHitChance);

		//Accuracy is restored
		combatStats.stat.accuracy -= (int)((float)combatStats.stat.accuracyBase * (float)chargeAccuracy);
	}

	public void SetDebuff()
	{
		//Special Note: The accuracy stat will be temporarily increased when using debuff (x2)
		combatStats.stat.accuracy += (int)((float)combatStats.stat.accuracyBase * 2f);
		
		//Calculate damage and status chance
		float damage = 0f;

		float statusChance = curseStatusChance;	//Very High chance of inflicting status chance
		
		//Fire
		if(elementalStance == 2)
		{
			//Calculate damage: for Cursing the damage will be put down by 4 times
			damage = (((float)combatStats.character.fireAffinity / 100f) + 1f) * (combatStats.stat.attack / 4f);

			//Instantiate Curse Effect to Target
			Instantiate (curseEffect[1],
			             CombatManager.enemies[objectCurrentSelection[1]].transform.position,
			             CombatManager.enemies[objectCurrentSelection[1]].transform.rotation);
		}
		
		//Water
		if(elementalStance == 4)
		{
			//Calculate damage
			damage = (((float)combatStats.character.waterAffinity / 100f) + 1f) * (combatStats.stat.attack / 4f);

			//Instantiate Curse Effect to Target
			Instantiate (curseEffect[3],
			             CombatManager.enemies[objectCurrentSelection[1]].transform.position,
			             CombatManager.enemies[objectCurrentSelection[1]].transform.rotation);
		}
		
		//Earth 
		if(elementalStance == 1)
		{
			//Calculate damage
			damage = (((float)combatStats.character.earthAffinity / 100f) + 1f) * (combatStats.stat.attack / 4f);

			//Instantiate Curse Effect to Target
			Instantiate (curseEffect[0],
			             CombatManager.enemies[objectCurrentSelection[1]].transform.position,
			             CombatManager.enemies[objectCurrentSelection[1]].transform.rotation);
		}
		
		//Lightning
		if(elementalStance == 3)
		{
			//Calculate damage
			damage = (((float)combatStats.character.lightningAffinity / 100f) + 1f) * (combatStats.stat.attack / 4f);
			//The status chance of stunning should be lower and should be at least 10% more than single shots chance
			statusChance = curseStatusChance - 0.1f;	

			//Instantiate Curse Effect to Target
			Instantiate (curseEffect[2],
			             CombatManager.enemies[objectCurrentSelection[1]].transform.position,
			             CombatManager.enemies[objectCurrentSelection[1]].transform.rotation);
		}
		
		CombatManager.enemyStats[objectCurrentSelection[1]].SetDamage (combatStats.stat, elementalStance, (int) damage,
		                                                               statusChance, 0f);	//no criticals
		
		//Accuracy is restored
		combatStats.stat.accuracy -= (int)((float)combatStats.stat.accuracyBase * 2f);
	}

	public void SetBuff()
	{
		//Depending on Element Buff target character 

		//Status Durations is the magic number 3
		switch(elementalStance)
		{
		case 1:	//If Earth
			CombatManager.playerStats[objectCurrentSelection[0]].SetAura (3);
			break;
		case 2:	//If Fire
			CombatManager.playerStats[objectCurrentSelection[0]].SetBlazingSpirit (3);
			break;
		case 3:	//If Lightning
			CombatManager.playerStats[objectCurrentSelection[0]].SetOvercharged (3);
			break;
		case 4:	//If Water
			CombatManager.playerStats[objectCurrentSelection[0]].SetPurify (3);
			break;
		}
	}

	float CalculateStatusChance(float _chance, float _affinityLevel, float _multiplier)
	{
		float statusChance;
		statusChance = ((_chance * (_affinityLevel * _multiplier)) / 100f) + _chance;
		return statusChance;
	}

	//Special Note: Using anim.gameObject.transform to move the animation, not this object this script is attached

	//This function is to teleport next to the enemy into melee range
	public void MoveToTarget(float _moveSpeed)
	{
		moveSpeed = _moveSpeed;

		//Calculate Target Location
		CapsuleCollider targetMeasurement = CombatManager.enemies[objectCurrentSelection[1]].GetComponent<CapsuleCollider>();
		targetWidth = targetMeasurement.radius;

		moveToInitialPosition = false;
		moveToTarget = true;

		//Determine if teleport by using move speed, if over the speed of light then yes.... its a teleport
		if(moveSpeed >= 30f)
		{
			TeleportEffectToggle(true);
		}
	}

	//This function is to Look at Enemy, by activating a boolean and the update will rotate player
	public void LookAtTarget(float _rotateSpeed)
	{
		rotateSpeed = _rotateSpeed;
		lookAtTargetTimer = 1f;
		lookAtInitialRotation = false;
		lookAtTarget = true;
	}

	//These functions restores initial rotation and transform
	public void MoveToInitialPosition(float _moveSpeed)
	{
		moveSpeed = _moveSpeed;

		moveToTarget = false;
		moveToInitialPosition = true;

		//Determine if teleport by using move speed, if over the speed of light then yes.... its a teleport
		if(moveSpeed >= 30f)
		{
			TeleportEffectToggle(true);
		}
	}

	public void LookAtInitialRotation(float _rotateSpeed)
	{
		rotateSpeed = _rotateSpeed;

		lookAtTarget = false;
		lookAtInitialRotation = true;
	}

	//This function prepares the Next Flurry Attack during the Flurry Attack Mode. Called in animation clip
	public void ReadyForNextFlurry()
	{
		flurryReady = true;
		flurryTimer = flurryMaxTimer;
		//print ("Next Flurry");
	}

	//This function Calculates whether the character needs to teleport to the next enemy during a Flurry
	void DetermineFlurryTeleport()
	{
		bool requireTeleport = false;	//Determines whether to teleport at the end of this function

		//selector.position = CombatManager.enemies[objectCurrentSelection[1]].transform.position;
		if(flurryNextEnemy > 0) //if Positive
		{
			flurryNextEnemy --;
			objectCurrentSelection[1]++; 	
			requireTeleport = true;
		}

		if(flurryNextEnemy < 0)	//if negative
		{
			flurryNextEnemy ++;
			objectCurrentSelection[1]--;
			requireTeleport = true;
		}

		//Ensure Selection is not over or lesser than size of list		
		if(objectCurrentSelection[1] >= CombatManager.enemies.Count)
		{
			objectCurrentSelection[1] = 0;
		}	
		if(objectCurrentSelection[1] < 0)
		{
			objectCurrentSelection[1] = CombatManager.enemies.Count -1;
		}

		//Extra Measures: If there is one enemy left than ignore teleport to enemy request
		if(CombatManager.enemies.Count == 1)
		{
			requireTeleport = false;
		}

		//Check Distance in case enemy dies and new enemy needs to be teleported to

		//Calculate Distance
		if(CombatManager.enemies[objectCurrentSelection[1]])
		{
			float dist = Vector3.Distance (CombatManager.enemies[objectCurrentSelection[1]].transform.position,
			                               anim.gameObject.transform.position);
			if(dist > targetWidth/2f + 2f)
			{
				requireTeleport = true;
			}
			else
			{
				requireTeleport = false;
			}
		}
		else
		{
			objectCurrentSelection[1] = 0;
			requireTeleport = true;
		}

		if(requireTeleport)
		{
			//Teleport to enemy
			MoveToTarget (teleportDashSpeed);
			LookAtTarget (teleportRotateSpeed);
		}
	}

	//These functions calculates the size limit of the level bars
	void SetLevelBarSizes()
	{
		levelBarLimit[0] = GetLevelBarSize(combatStats.character.level);			//Determine Normal Level Bar
		levelBarLimit[1] = GetLevelBarSize(combatStats.character.earthAffinity);	//Determine Earth Level Bar
		levelBarLimit[2] = GetLevelBarSize(combatStats.character.fireAffinity);		//Determine Fire Level Bar
		levelBarLimit[3] = GetLevelBarSize(combatStats.character.lightningAffinity);//Determine Lightning Level Bar
		levelBarLimit[4] = GetLevelBarSize(combatStats.character.waterAffinity);	//Determine Water Level Bar
	}

	int GetLevelBarSize(int _level)
	{
		int maxBars = 0;
		//Determine how many bars are needed. Extra Note: The max 
		if(_level >= 0)
		{
			maxBars = 3;
		}
		if (_level >= 5)
		{
			maxBars = 6;
		}
		if (_level >= 10)
		{
			maxBars = 9;
		}
		if (_level >= 15)
		{
			maxBars = 12;
		}
		if (_level >= 20)
		{
			maxBars = 15;
		}
		
		return maxBars;
	}

	//This function is called to determine if it is possible to initiate the flurry attack mode
	bool CanFlurry(int _element)
	{
		//Decide Whether its possible to move into the flurry Mode
		if(combatStats.character.level >= flurryLevelUnlock &&
		   combatStats.stat.actionPoints >= combatStats.elementalAPCost[_element] &&
		   levelBarLimit[0] > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	//This function is called to determine if it is possible to initiate the charging attack mode
	public bool CanCharge()
	{
		//Decide whether its possible to move into the charge attack mode
		if(combatStats.character.level >= chargeLevelUnlock &&
		   combatStats.stat.actionPoints >= 1)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	bool DetermineChargeCritical()
	{
		bool instantCritical = false;

		float chargePercentage = (float)((float)charge / (float)chargeMax);

		float[] criticalZones = combatStats.ui.chargeCriticalZone;

		for(int i = 0; i < criticalZones.Length ; i++)
		{
			if(criticalZones[i] < chargePercentage + 0.02f &&
			   criticalZones[i] > chargePercentage - 0.02f)
			{
				instantCritical = true;
				break;
			}
			else
			{
				instantCritical = false;
			}
		}

		return instantCritical;
	}

	//This procedure returns the attack phase back to idle
	void ReturnToIdle()
	{
		//Cannot go into flurry, move back to idle
		attackPhase = 0;
		anim.SetInteger ("Attack Phase", 0);
		//ShowSelection ();
		flurryActivate = false;
		readyTimer = readyMaxTimer;
		idleDelay = 1f;
		confirmed = false;
		UpdateObjectSelection();
		selector.gameObject.SendMessage ("ColliderSwitch", false, SendMessageOptions.DontRequireReceiver);
	}

	void HideSelection()
	{
		isSelecting = false;
		selector.transform.position = new Vector3(0f,-50f,0);
		combatStats.ui.ActiveUISwitch (false);
		actionModeLock = true;
	}

	void ShowSelection ()
	{
		//isSelecting = true;
		selector.gameObject.SetActive (true);
		combatStats.ui.ActiveUISwitch (true);
		actionModeLock = false;
	}

	void TeleportEffectToggle(bool _reveal)
	{
		if(_reveal)
		{
			//Hide renderers and reveal effects
			for(int i = 0; i< teleportHideObjects.Length; i++)
			{
				teleportHideObjects[i].SetActive (false);
			}
			for(int i = 0; i < teleportHideRenderers.Length; i++)
			{
				teleportHideRenderers[i].enabled = false;
			}
			for(int i = 0; i < teleportRevealEffects.Length; i++)
			{
				teleportRevealEffects[i].SetActive (true);
			}
		}
		else
		{
			//Hide renderers and reveal effects
			for(int i = 0; i< teleportHideObjects.Length; i++)
			{
				teleportHideObjects[i].SetActive (true);
			}
			for(int i = 0; i < teleportHideRenderers.Length; i++)
			{
				teleportHideRenderers[i].enabled = true;
			}
			for(int i = 0; i < teleportRevealEffects.Length; i++)
			{
				teleportRevealEffects[i].SetActive (false);
			}
		}
	}

	//This function is to Switch to the next Action Mode
	void SwitchToNextMode()
	{
		if(!actionModeLock)
		{
			actionMode ++; //Switch to the next Mode

			if(actionMode > 3)
			{
				actionMode = 0;
			}

			UpdateObjectSelection ();

			if(!combatStats.ui.codexMode)
			{
				modeSelection = false;

				combatStats.ui.actionIntroPanel.ResetTrigger ("AnimToggle");
			}
		}
	}

	public void SwitchToMode(int _mode)
	{
		if(!actionModeLock)
		{
			actionMode = _mode;

			UpdateObjectSelection ();

			if(!combatStats.ui.codexMode)
			{
				modeSelection = false;

				combatStats.ui.actionIntroPanel.ResetTrigger ("AnimToggle");
			}
		}
	}

	void IntroCameraSettings()
	{
		CombatCamera.control.SpawnGlobalAnimation (transform, "Player Intro");
	}

	void OffensiveCameraSettings()
	{
		CombatCamera cam = CombatCamera.control;

		//Reset Camera
		cam.CameraReset ();
		
		cam.SetOrigins ();
		
		//Set Speed
		cam.SetMoveSpeed(10f);
		cam.SetRotateSpeed (5f);
		
		//Set at the bottom right corner
		cam.SetPosition (5);
		
		//Set Position
		cam.SetMoveTo (gameObject);
		
		//Set Height
		cam.SetHeight (0.7f);
		
		//Set Distance 
		cam.SetDistance (1f);

		//Set Focus on Selector
		cam.SetFocus (selector.gameObject);

		/*
		//focus on targets
		CombatCamera cam = CombatCamera.control;

		//Reset Camera
		cam.CameraReset ();
		cam.SetOrigins ();

		//Move to player
		cam.SetMoveSpeed (50f);
		cam.SetRotateSpeed (5f);
		cam.SetMoveTo (gameObject);
		cam.SetLookAt (gameObject);

		//Zoom in to look at targets
		cam.SetPosition (18);

		//Focus on Selector
		cam.SetFocus (selector.gameObject);

		cam.SetHeight(0.5f);
		*/
	}

	void DefensiveCameraSettings()
	{
		//move to the front and focus on selected ally
		CombatCamera cam = CombatCamera.control;

		//Reset Camera
		cam.CameraReset();
		cam.SetOrigins ();
		cam.Stop();

		cam.SetHeight(0.5f);

		//Move to the selected player
		cam.SetMoveSpeed (100f);
		cam.SetRotateSpeed (1000f);

		//cam.gameObject.transform.eulerAngles = transform.eulerAngles;

		cam.SetRotation (new Vector3 (transform.eulerAngles.x,
		                              transform.eulerAngles.y - 180f,
		                              transform.eulerAngles.z));
		cam.SetTransform (selector.position);

		cam.SetFollow (selector.gameObject);

		//Change Position to the front of the character
		cam.SetPosition (4);

		//Focus on Selected Player
		cam.SetFocus (selector.gameObject);

		cam.SetDistance (3f);
	}

	void SingleShotCameraSettings()
	{
		CombatCamera cam = CombatCamera.control;
		
		//Reset Camera
		cam.CameraReset ();
		
		cam.SetOrigins ();
		
		//Set Speed
		cam.SetMoveSpeed(0.6f);
		cam.SetRotateSpeed (5f);

		//Set Distance 
		cam.SetDistance (6f);
		
		//Set at the bottom right corner
		cam.SetPosition (4);

		//Set Focus on Selector
		cam.SetLookAt (selector.gameObject);

		//Set Height
		cam.SetHeight (2f);
		
		//Set Position
		//cam.SetMoveTo (gameObject);

		//Move Right
		cam.Truck (1);
		//Then Zoom
		cam.Zoom (1);

		cam.increaseMoveSpeed = true;

		cam.DelayStop (4f);
	}

	void FlurryCameraSettings()
	{
		CombatCamera cam = CombatCamera.control;
		
		//Reset Camera
		cam.CameraReset();
		cam.SetOrigins ();
		cam.Stop();

		cam.SetMoveSpeed (50f);
		cam.SetRotateSpeed (30f);
		
		cam.SetPosition (16);
		cam.SetDistance (4f);
		cam.SetFollow (anim.gameObject);
		cam.SetFocus (anim.gameObject);
		cam.Orbit (-1);
	}

	//This procedure is called when it is required to move to the next selection, usually called via buttons
	void NextSelection(int _direction)
	{
		selectDirection = _direction;
	}

	void NextGroup()
	{
		SwitchToNextMode();
		SwitchToNextMode();
	}

	public void ActivateMode(int _mode)
	{
		actionModeLock = false;

		SwitchToMode (_mode);
	}

	void UpdateObjectSelection()
	{
		//Ensure Selections aren't out of Range
		//Player
		if(objectCurrentSelection[0] >= CombatManager.players.Count)
		{
			objectCurrentSelection[0] = 0;
		}
		//Enemy
		if(objectCurrentSelection[1] >= CombatManager.enemies.Count)
		{
			objectCurrentSelection[1] = 0;
		}
		//Object Interaction
		if(objectCurrentSelection[2] >= CombatManager.environmentObjects.Count)
		{
			objectCurrentSelection[2] = 0;
		}

		if(objectSelection == 2)
		{
			objectCurrentSelection[1] = 0;
		}

		switch(actionMode)
		{
		case 0:	//If Attack Mode
			ShowSelection ();
			objectSelection = 1;
			selector.gameObject.SetActive (true);
			//isSelecting = true;
			selector.position = CombatManager.enemies[objectCurrentSelection[objectSelection]].transform.position;
			break;
		case 1:	//If Defend Mode
			ShowSelection ();
			objectSelection = 0;
			selector.gameObject.SetActive (true);
			//isSelecting = true;
			selector.position = CombatManager.players[objectCurrentSelection[objectSelection]].transform.position;
			break;
		case 2:	//If Support Mode
			ShowSelection ();
			objectSelection = 0;
			selector.gameObject.SetActive (true);
			//isSelecting = true;
			selector.position = CombatManager.players[objectCurrentSelection[objectSelection]].transform.position;
			break;
		case 3:	//If Curse Mode
			ShowSelection ();
			objectSelection = 1;
			selector.gameObject.SetActive (true);
			//isSelecting = true;
			selector.position = CombatManager.enemies[objectCurrentSelection[objectSelection]].transform.position;
			break;
		}
	}

	//This function ends the turn with a delay
	public void EndTurnDelay(float _time)
	{
		Invoke ("EndTurn", _time);
		//If Defending then don't change back to idle
		if(!combatStats.defend)
		{
			anim.SetInteger ("Attack Phase", 0);
		}
	}
	
	//This function will end the players turn
	public void EndTurn()
	{
		//If Defending then don't change back to idle
		if(!combatStats.defend)
		{
			anim.SetInteger ("Attack Phase", 0);
		}
		//Hide Selection
		HideSelection ();
		//Send Message to Combat Manager to start next turn
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("NextTurn", SendMessageOptions.DontRequireReceiver);
		//Turn off this script
		this.enabled = false; 

		//As this will be the Main Player Script De Activate the scan button
		combatStats.ui.ActiveUISwitch (false);	//Turn off UI elements
		combatStats.statusUI.SetCurrent (false);

		//Correct Position
		anim.transform.position = transform.position;
		anim.transform.eulerAngles = transform.eulerAngles;

		//Unmark as Current
		combatStats.currentTurn = false;

		//Destroy Emit Action 
		if(currentEmit)
		{
			Destroy (currentEmit.gameObject);
		}

		//Make sure Selector Collider is off
		selector.gameObject.SendMessage ("ColliderSwitch", false, SendMessageOptions.DontRequireReceiver);

		Time.timeScale = 1f;	//Reset Timescale
		combatStats.ui.skipPrompt.SetActive (false);
	}
}
