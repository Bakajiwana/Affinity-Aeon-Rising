using UnityEngine;
using System.Collections;

//Script Objective: The Overworld Enemy Behaviour	

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]

public class OverworldEnemy : MonoBehaviour 
{
	//Mecanim Variables
	private Animator anim;
	private NavMeshAgent agent;

	//Detection Variables
	private Transform alertSymbol;

	//Main Enemy Node Variables
	private Transform mainArea;
	private CombatActivator combatActivator;
	public float maxDistanceFromMainArea = 20f;
	public float maxPatrolDistance = 5f;

	//Movement Speed
	public float normalSpeed = 3.5f;
	public float fastSpeed = 10f;

	//Behaviour Variables
	private int currentMode = 0;
	public bool spawnOnPlayerEntry = false; 
	public bool moveToPositionOnStart = false;
	private float moveToPositinOnStartTimer = 10f;
	public Transform startMoveToPosition;
	public bool patrolMode = false;
	private float patrolIdleTimer;
	public float patrolIdleMaxTimer = 2f;
	private float patrolTimer;
	public float patrolMaxTimer = 3f;
	private Transform patrolPoint;
	public bool playerDetected = false;
	public bool chaseOnDetection = false;
	private Transform playerObject;
	private float chaseTimer;
	public float chaseMaxTimer = 10f;

	public bool patrolModeAfterDetection = false;

	public float detectionDistance = 2f;
	public float detectionRadius = 4f;

	public bool alertOthersOnDetection = false;

	void Awake()
	{
		AcquireMandatoryComponents();
	}

	// Use this for initialization
	void Start () 
	{	
		//Find the Main Area variable (the parent of this object)
		mainArea = transform.parent;
		combatActivator = mainArea.gameObject.GetComponent<CombatActivator>();

		//Finds and assigns the child of the transform named "Alert".	
		alertSymbol = transform.Find("Alert");

		if(alertSymbol == null)
		{
			Debug.Log("No child with the name 'Alert' attached to the player");
		}

		alertSymbol.gameObject.SetActive (false);

		if(moveToPositionOnStart)
		{
			agent.enabled = false;
		}

		//Initialise Timers
		patrolIdleTimer = patrolIdleMaxTimer;
		patrolTimer = patrolMaxTimer;
		chaseTimer = chaseMaxTimer;

		//If Spawn on Player Entry is true, Disable this object until set active by a trigger 
		if(spawnOnPlayerEntry)
		{
			transform.gameObject.SetActive (false);
		}

		//print (GameObject.FindGameObjectWithTag ("Player").transform.name);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(playerDetected)
		{
			currentMode = 3;
		}

		switch(currentMode)
		{
		case 0:	//Idle
			agent.speed = 0f;
			anim.SetFloat ("Speed", 0f);

			//If Player is Detected = Go to Case 3
			if(playerDetected)
			{
				currentMode = 3;
				goto case 3;
			}

			//If one of the behaviours is true then move to next case
			if(moveToPositionOnStart || patrolMode)
			{
				currentMode++;
				goto case 1;
			}
			break;
		case 1:	//Move To Player Position on Start
			//If Move to position on start is true, move to specified node and also turn off agent
			if(moveToPositionOnStart)
			{
				if(startMoveToPosition && moveToPositinOnStartTimer > 0f && transform.position != startMoveToPosition.position)
				{
					//Move Towards Start Point
					moveToPositinOnStartTimer -= Time.deltaTime;

					float step = normalSpeed * Time.deltaTime;
					transform.position = Vector3.MoveTowards (transform.position, startMoveToPosition.position, step);
					anim.SetFloat ("Speed", normalSpeed);

					//Look at start point
					transform.LookAt (startMoveToPosition);
				}
				else
				{
					//print ("Stop");
					agent.enabled = true;
					moveToPositionOnStart = false;
					currentMode++;
					goto case 2;
				}
			}
			else
			{
				agent.enabled = true;
				moveToPositionOnStart = false;
				currentMode++;
				goto case 2;
			}
			break;
		case 2:	//Patrol Mode
			if(patrolMode)
			{
				if(patrolIdleTimer > 0f)
				{
					//Counting Down
					patrolIdleTimer -= Time.deltaTime;

					//Speed At 0
					agent.speed = 0f;
					anim.SetFloat ("Speed", agent.speed);

					if(patrolIdleTimer <= 0f)
					{
						//Change Patrol Point Location
						patrolPoint.gameObject.SetActive (false);
						patrolPoint.localPosition = RandomPatrolLocation(); 
						patrolPoint.gameObject.SetActive (true);

						//Agent Speed moving
						agent.speed = normalSpeed;
						anim.SetFloat ("Speed", agent.speed);
					}
				}
				else
				{
					//Moving to Patrol Point Location
					agent.destination = patrolPoint.position;

					if(patrolTimer > 0f)
					{
						patrolTimer -= Time.deltaTime;
					}
					else
					{
						PatrolReset();
					}
				}
			}
			break;
		case 3:	//Player Detected
			//Get the distance of this object and the main area node
			float dist = Vector3.Distance (mainArea.position, transform.position);

			if(playerDetected && chaseTimer > 0f  && dist < maxDistanceFromMainArea)
			{
				chaseTimer -= Time.deltaTime; 

				//Chase Player 
				if(chaseOnDetection)
				{
					agent.destination = playerObject.position;
				}
			}
			else
			{
				playerDetected = false;
				ColourChange.turnOffAlert = true;
				anim.SetBool ("Detected", false);
				anim.SetFloat ("Speed", 0f);
				alertSymbol.gameObject.SetActive (false);
				currentMode = 0;
				goto case 0;
			}
			break;
		}
	}

	Vector3 RandomPatrolLocation()
	{
		float x;
		float y;
		float z;

		x = maxPatrolDistance;
		y = mainArea.position.y;
		z = maxPatrolDistance;

		return new Vector3(Random.Range (x,-x), y, Random.Range (z,-z));
	}

	void AcquireMandatoryComponents()
	{
		if(gameObject.GetComponent<Animator>())
		{
			anim = gameObject.GetComponent<Animator>();
		}
		else
		{
			anim = gameObject.GetComponentInChildren<Animator>();
		}

		agent = gameObject.GetComponent<NavMeshAgent>();

		if(!gameObject.GetComponent<Rigidbody>())
		{
			gameObject.AddComponent<Rigidbody>();
		}
		
		if(!gameObject.GetComponent<CapsuleCollider>())
		{
			gameObject.AddComponent<CapsuleCollider>();
		}
		
		if(!gameObject.GetComponent<NavMeshAgent>())
		{
			gameObject.AddComponent<NavMeshAgent>();
		}

		gameObject.GetComponent<Rigidbody>().useGravity = false;
		gameObject.GetComponent<CapsuleCollider>().isTrigger = true;

		//Create an Empty game object and parent to object
		patrolPoint = new GameObject("Patrol Point of " + transform.name).transform;
		patrolPoint.SetParent (transform.parent);

		//Add Components to this patrol point
		SphereCollider col = patrolPoint.gameObject.AddComponent<SphereCollider>();
		col.radius = 1f; 
		col.isTrigger = true;
		patrolPoint.gameObject.AddComponent<NavMeshAgent>();
		patrolPoint.gameObject.layer = 2;

		//Create a Detection Sphere Game Object
		GameObject detectSphere = new GameObject("Detection Sphere");
		detectSphere.transform.SetParent (transform);
		detectSphere.transform.localPosition = Vector3.zero;
		DetectionSphere det = detectSphere.AddComponent<DetectionSphere>();
		det.CreateDetectionSphere (detectionDistance, detectionRadius);
		detectSphere.gameObject.layer = 2;
	}

	public void Detected(Transform _player)
	{
		agent.enabled = true;
		if(playerDetected == false)
		{
			if(alertOthersOnDetection)
			{
				OverworldEnemy[] enemies = mainArea.GetComponentsInChildren<OverworldEnemy>();
				
				for(int i = 0; i < enemies.Length; i++)
				{
					enemies[i].AlertOthers (_player);
				}
			}


			anim.SetBool ("Detected", true);

			agent.speed = 0f;
			anim.SetFloat ("Speed", agent.speed);

			if(chaseOnDetection)
			{
				Invoke ("ChasingSpeed", 1f);
			}

			alertSymbol.gameObject.SetActive (true);
			//print ("Alerting"); 
		}

		playerObject = _player;

		playerDetected = true;
		ColourChange.turnOnAlert = true;
		chaseTimer = chaseMaxTimer;
	//	Debug.Log ("Detected " + chaseTimer);
		currentMode = 3;

		if(patrolModeAfterDetection)
		{
			patrolMode = true;
		}
	}

	public void AlertOthers(Transform _player)
	{
		agent.enabled = true;
		if(playerDetected == false)
		{			
			anim.SetBool ("Detected", true);
			
			agent.speed = 0f;
			anim.SetFloat ("Speed", agent.speed);
			
			if(chaseOnDetection)
			{
				Invoke ("ChasingSpeed", 1f);
			}
			
			alertSymbol.gameObject.SetActive (true);
			//print ("Alerting"); 
		}
		
		playerObject = _player;
		
		playerDetected = true;
		chaseTimer = chaseMaxTimer;
		//	Debug.Log ("Detected " + chaseTimer);
		currentMode = 3;
		
		if(patrolModeAfterDetection)
		{
			patrolMode = true;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == patrolPoint.gameObject)
		{
			//Trigger Search and Reset Patrol Idle Timer
//			Debug.Log ("Patrol Point Reached");
			PatrolReset();
		}

		if(other.gameObject.CompareTag ("Player"))
		{
			ActivateBattle (other.gameObject);
		}
	}

	public void ActivateBattle(GameObject _object)
	{
		combatActivator.StartBattle (_object);
		ColourChange.turnOffAlert = true;
	}

	void PatrolReset()
	{
		anim.SetTrigger ("Search");
		patrolIdleTimer = patrolIdleMaxTimer;
		patrolTimer = patrolMaxTimer;
	}

	void ChasingSpeed()
	{
		agent.speed = fastSpeed;
		anim.SetFloat ("Speed", agent.speed);
	}
}
