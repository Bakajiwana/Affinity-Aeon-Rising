using UnityEngine;
using System.Collections;

//Script Objective: Apply Events to Animator and control Camera through animation events

public class CombatCameraEvents : MonoBehaviour 
{
	private GameObject focusObject;

	//This Procedure is used to Change the Position of the Camera
	public void SetCameraPosition(int _index)
	{
		CombatCamera.control.SetPosition (_index);
	}
	
	//This Procedure is called to Move the Main node towards an object
	public void SetCameraMoveTo()
	{
		CombatCamera.control.SetMoveTo (gameObject);
	}
	
	//This Procedure is called to Rotate the Main node towards an object
	public void SetCameraLookAt()
	{
		CombatCamera.control.SetLookAt (gameObject);
	}

	public void CameraSetObjectFocus(GameObject _object)
	{
		focusObject = _object; 
	}
	
	//This Procedure is called to Focus the camera onto a target
	public void SetCameraFocus()
	{
		CombatCamera.control.SetFocus (gameObject);
	}

	public void SetCameraFocusOnObject()
	{
		CombatCamera.control.SetFocus (focusObject);
	}

	public void SetCameraMoveToFocusObject()
	{
		CombatCamera.control.SetMoveTo (focusObject);
	}
	
	public void SetCameraFocusHeight(float _height)
	{
		CombatCamera.control.SetFocusHeight (_height);
	}
	
	public void SetCameraFocusOrigin()
	{
		CombatCamera.control.SetFocusOrigin ();
	}
	
	//This Procedure is called to Set the movement speed of the camera
	public void SetCameraMoveSpeed(float _moveSpeed)
	{
		CombatCamera.control.SetMoveSpeed (_moveSpeed);
	}
	
	//This Prcedure is called to set the rotation speed of the camera
	public void SetCameraRotateSpeed(float _rotateSpeed)
	{
		CombatCamera.control.SetRotateSpeed (_rotateSpeed);
	}
	
	//This Procedure is called to zoom the camera 
	public void CameraZoom(int _direction)
	{
		CombatCamera.control.Zoom (_direction);
	}
	
	//Pan - Rotate Left or Right
	public void CameraPan(int _direction)
	{
		CombatCamera.control.Pan (_direction);
	}
	
	//Tilt - Rotate Up or Down
	public void CameraTilt(int _direction)
	{
		CombatCamera.control.Tilt (_direction);
	}
	
	//Truck - Move Left or Right
	public void CameraTruck(int _direction)
	{
		CombatCamera.control.Truck (_direction);
	}
	
	//Pedestal - Move Up or Down
	public void CameraPedestal (int _direction)
	{
		CombatCamera.control.Pedestal (_direction);
	}
	
	//Orbit
	public void CameraOrbit (int _direction)
	{
		CombatCamera.control.Orbit (_direction);
	}
	
	//Set distance
	public void SetCameraDistance (float _meters)
	{
		CombatCamera.control.SetDistance (_meters);
	}
	
	//Set height
	public void SetCameraHeight(float _meters)
	{
		CombatCamera.control.SetHeight (_meters);
	}
	
	//Set Follow
	public void SetCameraFollow()
	{
		CombatCamera.control.SetFollow (gameObject);
	}
	
	//Set Rotate Towards
	public void SetCameraRotateTowards()
	{
		CombatCamera.control.SetRotateTowards (gameObject);
	}
	
	//Set Animated
	public void SetCameraAnimated(GameObject _object)
	{
		CombatCamera.control.SetAnimated (_object);
	}
	
	//Screen Effect
	public void CameraScreenEffect(GameObject _effect)
	{
		CombatCamera.control.ScreenEffect (_effect);
	}

	public void CameraScreenEffect(int _effect)
	{
		CombatCamera.control.ScreenEffect (_effect);
	}

	public void CameraReset()
	{
		CombatCamera.control.CameraReset ();
	}

	public void CameraStop()
	{
		CombatCamera.control.Stop ();
	}

	public void CameraOriginReset()
	{
		CombatCamera.control.SetOrigins ();
	}
}
