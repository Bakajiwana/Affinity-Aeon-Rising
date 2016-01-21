using UnityEngine;
using System.Collections;

//This script acts as a communication to the main script and handles all animation events

public class PlayerCombatEvents : MonoBehaviour 
{
	public PlayerCombatActions characterEvent;

	public CombatCharacterCamera cameraEvent;

	public PlayerCombatCharacter character;

	public Transform[] characterProps;

	public bool startWithAllPropsOff = false;

	private int currentPropIndex;

	// Use this for initialization
	void Start () 
	{
		if(startWithAllPropsOff)
		{
			foreach(Transform obj in characterProps)
			{
				obj.gameObject.SetActive (false);
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
		if(characterEvent)
		{
			characterEvent.EndTurn();
		}
	}
	
	public void FireSingleShot(int _projectileSlot)
	{
		characterEvent.FireSingleShot (_projectileSlot);
	}
	
	public void EndTurnDelay(float _time)
	{
		if(characterEvent)
		{
			characterEvent.EndTurnDelay (_time);
		}
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

	//This function is to teleport next to the enemy into melee range
	public void MoveToTarget(float _moveSpeed)
	{
		characterEvent.MoveToTarget (_moveSpeed);
	}
	
	//This function is to Look at Enemy, by activating a boolean and the update will rotate player
	public void LookAtTarget(float _rotateSpeed)
	{
		characterEvent.LookAtTarget (_rotateSpeed);
	}
	
	//These functions restores initial rotation and transform
	public void MoveToInitialPosition(float _moveSpeed)
	{
		characterEvent.MoveToInitialPosition (_moveSpeed);
	}
	
	public void LookAtInitialRotation(float _rotateSpeed)
	{
		characterEvent.LookAtInitialRotation (_rotateSpeed);
	}
	
	public void ReadyForNextFlurry()
	{
		characterEvent.ReadyForNextFlurry();
	}

	public void SetFlurryDamage()
	{
		characterEvent.SetFlurryDamage ();
	}

	public void SetChargeDamage()
	{
		characterEvent.SetChargeDamage ();
	}

	public void SetDebuff()
	{
		characterEvent.SetDebuff ();
	}

	public void SetBuff()
	{
		characterEvent.SetBuff ();
	}

	public void TeleportBackToPosition()
	{
		character.TeleportBackToPosition ();
	}
}
