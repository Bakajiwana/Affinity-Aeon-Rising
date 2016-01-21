using UnityEngine;
using System.Collections;

//This script acts as a communication to the main script and handles all animation events

public class PlayerCombatAIEvents : MonoBehaviour 
{
	public PlayerCombatAI characterEvent;

	public CombatCharacterCamera cameraEvent;

	public PlayerCombatCharacter character;

	public Transform[] characterProps;

	public bool startWithAllPropsOff = false;
	
	private int currentPropIndex;

	private bool moveToTarget = false;
	private bool lookAtTarget = false;
	private bool lookAtReturn = false;
	private bool rotateToTarget = false;
	
	private bool moveToInitial = false;
	private bool rotateToInitial = false;
	
	public float moveMaxTimer = 3f;
	private float moveTimer;
	public float stopDistance = 2f;
	public float rotateMaxTimer = 2f;
	private float rotateTimer;
	
	public float moveSpeed = 5f;
	public float rotateSpeed = 5f;
	
	private Quaternion iniRot;


	// Use this for initialization
	void Start () 
	{
		moveTimer = moveMaxTimer;
		rotateTimer = rotateMaxTimer;
		
		iniRot = characterEvent.gameObject.transform.rotation;

		if(startWithAllPropsOff)
		{
			foreach(Transform obj in characterProps)
			{
				obj.gameObject.SetActive (false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If Move Towards Target
		if(moveToTarget)
		{
			if(moveTimer >= 0f)
			{
				//Calculate Distance
				float dist = Vector3.Distance (CombatManager.enemies[characterEvent.targetIndex].transform.position,
				                               transform.position);
				if(dist > stopDistance)
				{
					float step = moveSpeed * Time.deltaTime;
					transform.position = Vector3.MoveTowards (transform.position, 
					                                          CombatManager.enemies[characterEvent.targetIndex].transform.position,
					                                          step);
				}
				else
				{
					characterEvent.anim.SetTrigger ("Next");
				}
				
				moveTimer -= Time.deltaTime;
			}
			else
			{
				moveToTarget = false;
			}
		}
		
		//If Move Towards Target
		if(moveToInitial)
		{
			if(moveTimer >= 0f)
			{
				float step = moveSpeed * Time.deltaTime;
				transform.position = Vector3.MoveTowards (transform.position, 
				                                          characterEvent.gameObject.transform.position,
				                                          step);
				
				moveTimer -= Time.deltaTime;
				
				if(transform.position == characterEvent.gameObject.transform.position)
				{
					characterEvent.anim.SetTrigger ("Next");
				}
			}
			else
			{
				moveToInitial = false;
			}
		}
		
		if(rotateToTarget)
		{
			if(rotateTimer >= 0f)
			{
				Vector3 lookPos = CombatManager.enemies[characterEvent.targetIndex].transform.position - transform.position;
				lookPos.y = 0;
				
				Quaternion rotation = Quaternion.LookRotation (lookPos);
				
				transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * rotateSpeed);
				
				rotateTimer -= Time.deltaTime;
			}
			else
			{
				rotateToTarget = false;
			}
		}

		if(CombatManager.enemies.Count > 0)
		{
			if(rotateToInitial)
			{
				if(rotateTimer >= 0f)
				{
					
					Vector3 lookPos = characterEvent.gameObject.transform.position - transform.position;
					Vector3 iniPos = CombatManager.enemies[0].transform.position - transform.position;
					if(lookPos != Vector3.zero)
					{
						lookPos.y = 0;
						
						Quaternion rotation = Quaternion.LookRotation (lookPos);
						
						transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * rotateSpeed);
					}
					else
					{
						if(iniPos != Vector3.zero)
						{
							iniPos.y = 0;
							
							Quaternion rotation = Quaternion.LookRotation (iniPos);
							
							transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * rotateSpeed);
						}
					}
					
					rotateTimer -= Time.deltaTime;
				}
				else
				{
					rotateToInitial = false;
				}
			}
		}
		
		if(lookAtTarget)
		{
			if(rotateTimer >= 0f)
			{
				Vector3 targetDir = CombatManager.enemies[characterEvent.targetIndex].transform.position - 
					characterEvent.gameObject.transform.position;
				float step = rotateSpeed * Time.deltaTime;
				Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, step, 0.0f);
				characterEvent.gameObject.transform.rotation = Quaternion.LookRotation (newDir);
				
				rotateTimer -= Time.deltaTime;
			}
			else
			{
				lookAtTarget = false;
			}
		}
		
		if(lookAtReturn)
		{
			if(rotateTimer >= 0f)
			{
				float step = rotateSpeed * 15f * Time.deltaTime;
				
				characterEvent.gameObject.transform.rotation = Quaternion.RotateTowards (characterEvent.gameObject.transform.rotation,
				                                                                         iniRot, step);
				
				rotateTimer -= Time.deltaTime;
			}
			else
			{
				lookAtReturn = false;
			}
		}
		
	}

	public void SwitchOffProp(int _index)
	{
		characterProps[_index].gameObject.SetActive (false);
	}
	
	public void SwitchOnProp(int _index)
	{
		characterProps[_index].gameObject.SetActive (true);
	}
	
	public void SpawnOnSelf(GameObject _object)
	{
		Instantiate (_object.transform, transform.position, transform.rotation);
	}
	
	public void SpawnOnSelected(GameObject _object)
	{
		Instantiate (_object.transform, 
		             GameObject.FindGameObjectWithTag ("Combat Selector").transform.position, 
		             GameObject.FindGameObjectWithTag ("Combat Selector").transform.rotation);
	}
	
	public void SpawnOnSelfAndParent(GameObject _object)
	{
		Transform spawn = Instantiate (_object.transform, transform.position, transform.rotation) as Transform;
		spawn.SetParent (transform,  true);
	}
	
	public void SetPropIndex(int _index)
	{
		currentPropIndex = _index;
	}
	
	public void SpawnOnProp(GameObject _object)
	{
		Instantiate (_object.transform, characterProps[currentPropIndex].position, characterProps[currentPropIndex].rotation);
	}
	
	public void SpawnOnPropAndParent(GameObject _object)
	{
		Transform spawn = Instantiate (_object.transform, characterProps[currentPropIndex].position, characterProps[currentPropIndex].rotation) as Transform;
		spawn.SetParent (characterProps[currentPropIndex],  true);
	}
	
	public void EndTurn()
	{
		characterEvent.EndTurn();
	}
	
	public void FireSingleShot(int _projectileSlot)
	{
		characterEvent.FireProjectile (_projectileSlot);
	}
	
	public void EndTurnDelay(float _time)
	{
		characterEvent.EndTurnDelay (_time);
	}

	public void TurnOnCharacterCamera()
	{
		cameraEvent.TurnOnCharacterCamera();
	}
	
	public void SwitchBackToRandomArenaCamera()
	{
		cameraEvent.SwitchBackToRandomArenaCamera();
	}
	
	public void TurnOnRandomArenaCamera()
	{
		cameraEvent.TurnOnRandomArenaCamera();
	}
	
	public void SwitchBackToSelectedArenaCamera(int _camera)
	{
		cameraEvent.SwitchBackToSelectedArenaCamera(_camera);
	}

	public void SpawnObject(GameObject _sound)
	{
		Instantiate (_sound, transform.position,transform.rotation);
	}

	public void TeleportBackToPosition()
	{
		character.TeleportBackToPosition ();
	}

	public void SpawnObjectOnTarget(GameObject _object)
	{
		Instantiate (_object, CombatManager.enemies[characterEvent.targetIndex].transform.position,
		             CombatManager.enemies[characterEvent.targetIndex].transform.rotation);
	}
	
	public void SpawnObjectOnField(GameObject _object)
	{
		Instantiate (_object, Vector3.zero, Quaternion.identity);
	}
	
	public void SetEnemyDamage()
	{
		characterEvent.SetEnemyDamage ();
	}
	
	public void SetEnemyDamageDelay(float _time)
	{
		characterEvent.SetEnemyDamageDelay (_time);
	}
	
	public void MoveToTarget()
	{
		moveToTarget = true;
		moveTimer = moveMaxTimer;
	}
	
	public void RotateToTarget()
	{
		rotateToTarget = true;
		lookAtTarget = false;
		rotateTimer = rotateMaxTimer;
	}
	
	public void LookAtTarget()
	{
		lookAtTarget = true;
		rotateTimer = rotateMaxTimer;
	}
	
	public void LookAtReturn()
	{
		lookAtReturn = true;
		lookAtTarget = false;
		rotateTimer = rotateMaxTimer;
	}
	
	public void MoveToInitial()
	{
		moveToInitial = true;
		moveToTarget = false;
		moveTimer = moveMaxTimer;
	}
	
	public void RotateToInitial()
	{
		rotateToInitial = true;
		rotateToTarget = false;
		rotateTimer = rotateMaxTimer;
	}
	
	public void SetParentLookAt()
	{
		transform.parent.LookAt (CombatManager.enemies[characterEvent.targetIndex].transform);
	}
	
	public void SetParentLookAtReturn()
	{
		transform.parent.eulerAngles = Vector3.zero;
	}
	
	public void ParentNodeFixRotation(float _rot)
	{
		transform.parent.localEulerAngles = new Vector3(0f, _rot, 0f);
	}

	public void Defend()
	{
		characterEvent.Defend ();
	}
}
