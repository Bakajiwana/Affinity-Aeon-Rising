using UnityEngine;
using System.Collections;

public class EnemyCombatEvents : MonoBehaviour 
{
	public EnemyCombatActions characterEvent;

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
				float dist = Vector3.Distance (CombatManager.players[characterEvent.targetIndex].transform.position,
				                               transform.position);
				if(dist > stopDistance)
				{
					float step = moveSpeed * Time.deltaTime;
					transform.position = Vector3.MoveTowards (transform.position, 
					                                          CombatManager.players[characterEvent.targetIndex].transform.position,
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
				Vector3 lookPos = CombatManager.players[characterEvent.targetIndex].transform.position - transform.position;
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

		if(rotateToInitial)
		{
			if(rotateTimer >= 0f)
			{

				Vector3 lookPos = characterEvent.gameObject.transform.position - transform.position;
				Vector3 iniPos = CombatManager.players[0].transform.position - transform.position;
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

		if(lookAtTarget)
		{
			if(rotateTimer >= 0f)
			{
				Vector3 targetDir = CombatManager.players[characterEvent.targetIndex].transform.position - 
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

	public void EndTurn()
	{
		characterEvent.EndTurn();
	}

	public void FireProjectile(int _projectileSlot)
	{
		characterEvent.FireProjectile (_projectileSlot);
	}

	public void EndTurnDelay(float _time)
	{
		characterEvent.EndTurnDelay (_time);
	}

	public void SpawnObject(GameObject _object)
	{
		Instantiate (_object, transform.position,transform.rotation);
	}

	public void SpawnObjectAndParent(GameObject _object)
	{
		Transform objectSpawn = Instantiate (_object.transform, transform.position,transform.rotation) as Transform;
		objectSpawn.SetParent (transform);
	}

	public void SpawnObjectOnTarget(GameObject _object)
	{
		Instantiate (_object, CombatManager.players[characterEvent.targetIndex].transform.position,
		             CombatManager.players[characterEvent.targetIndex].transform.rotation);
	}

	public void SpawnObjectOnField(GameObject _object)
	{
		Instantiate (_object, Vector3.zero, Quaternion.identity);
	}

	public void SetPlayerDamage()
	{
		characterEvent.SetPlayerDamage ();
	}

	public void SetPlayerDamageDelay(float _time)
	{
		characterEvent.SetPlayerDamageDelay (_time);
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
		transform.parent.LookAt (CombatManager.players[characterEvent.targetIndex].transform);
	}

	public void SetParentLookAtReturn()
	{
		transform.parent.eulerAngles = Vector3.zero;
	}

	public void ParentNodeFixRotation(float _rot)
	{
		transform.parent.localEulerAngles = new Vector3(0f, _rot, 0f);
	}
}
