using UnityEngine;
using System.Collections;

//Script Objective: Control the Main Combat Camera

public class CombatCamera : MonoBehaviour 
{
	public static CombatCamera control;

	//Camera Object Variables
	public Transform mainNode;
	public Transform camNode;
	public Transform cam;

	public Transform cameraPositionsNode;
	private Transform[] cameraPositions;

	//Switch Case Variables
	private int mainMove;
	private int mainRotate;

	private int nodeMove;

	private int camRotate; 

	//Camera Variables
	private int currentPosition = 0;
	public float defaultMoveSpeed = 2f;
	private float moveSpeed;
	public float defaultRotateSpeed = 2f;
	private float rotateSpeed;

	private Vector3 moveToPosition;
	private Vector3 lookAtPosition;

	private Transform focusTarget;
	private float focusHeight = 1f;

	private int zoomDirection;
	private int panDirection;
	private int tiltDirection;
	private int truckDirection;
	private int pedestalDirection;
	private int orbitDirection; 

	private Transform followObject;
	private Transform rotateObject; 
	private Transform animationObject;

	public Transform globalCameraAnimator;
	private Animator currentGlobalAnimator;

	public Transform[] screenEffects;
	public Transform effectScreen;

	[HideInInspector]
	public bool increaseMoveSpeed = false;
	[HideInInspector]
	public bool increaseRotateSpeed = false;

	private GlowEffect glow;

	// Use this for initialization
	void Awake () 
	{
		control = this;

		cameraPositions = cameraPositionsNode.GetComponentsInChildren<Transform>();	

		SetOrigins ();
	}

	//Camera Reset
	public void CameraReset()
	{
		moveSpeed = defaultMoveSpeed;
		rotateSpeed = defaultRotateSpeed;

		focusHeight = 1f;

		zoomDirection = 0;
		panDirection = 0;
		tiltDirection = 0;
		truckDirection = 0;
		pedestalDirection = 0;
		orbitDirection = 0; 

		Stop ();
	}

	public void Stop()
	{
		mainMove = 0;
		mainRotate = 0;		
		nodeMove = 0;		
		camRotate = 0;

		increaseMoveSpeed = false;
		increaseRotateSpeed = false;
	}

	public void DelayStop(float _time)
	{
		Invoke ("Stop", _time);
	}

	public void SetOrigins()
	{
		camNode.localEulerAngles = Vector3.zero;
		camNode.localPosition = Vector3.zero;
		cam.localEulerAngles = Vector3.zero;
		cam.localPosition = Vector3.zero;
		cameraPositionsNode.localPosition = Vector3.zero;
		mainNode.localEulerAngles = Vector3.zero;
		cameraPositionsNode.localScale = new Vector3 (1f,1f,1f);
	}

	void Start()
	{
		//Debug and Testing
		//SetPosition (1);
		//SetMoveTo (GameObject.Find ("Selector"));
		//SetLookAt (GameObject.Find ("Selector"));
		//SetFocus (GameObject.Find ("Selector"));
		//SetFocusHeight (1f);
		//SetRotateTowards (GameObject.Find ("Selector"));
		//Zoom (-1);
		//SetHeight (5f);
		//Pan (1);

		/*Intro Camera
		SetMoveSpeed (8f);
		SetRotateSpeed (20f);
		GameObject focus = GameObject.FindGameObjectWithTag ("Combat Selector");
		SetMoveTo(focus);
		SetRotateTowards(focus);
		*/
		SpawnGlobalAnimation (null, "Field Preview");
		glow = cam.GetComponent<GlowEffect>();
		glow.enabled = true;
		Invoke ("TurnOffGlowEffect", 3f);
	}

	void TurnOffGlowEffect()
	{
		glow.enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(glow.isActiveAndEnabled)
		{
			glow.glowTint.b -= 0.005f;
		}

		//Switch Case for the Movement of the Main Camera Node
		switch(mainMove)
		{
		case 0: //Do nothing
			break;
		case 1:	//Set Move To
			float speed = moveSpeed * Time.deltaTime;
			mainNode.position = Vector3.MoveTowards (mainNode.position, moveToPosition, speed);

			if(transform.position == moveToPosition)
			{
				mainMove = 0;
			}
			break;
		case 2:	//Truck - Move Left or Right
			mainNode.Translate((Vector3.right * truckDirection) * moveSpeed * Time.deltaTime, Space.Self);
			break;
		case 3:	//Pedestal - Move Up or Down
			mainNode.Translate((Vector3.up * pedestalDirection) * moveSpeed * Time.deltaTime, Space.World);
			break;
		case 4:	//-----Empty-----
			break;
		case 5:	//Set Follow
			float followSpeed = moveSpeed * Time.deltaTime;
			mainNode.position = Vector3.MoveTowards (mainNode.position, followObject.position, followSpeed);
			break;
		case 6:	//Set Animated
			if(mainNode && animationObject)
			{
				mainNode.position = animationObject.position;
			}

			if(!animationObject)
			{
				mainMove = 0;
			}
			break;
		}

		//Switch Case for the Rotation of the Main Camera Node
		switch(mainRotate)
		{
		case 0:	//Do Nothing
			break;
		case 1: //Set Look At
			Vector3 lookPos = lookAtPosition - mainNode.position;
			lookPos.y = 0;
			Quaternion rotation = Quaternion.LookRotation (lookPos);
			transform.rotation = Quaternion.Slerp (mainNode.rotation, rotation, Time.deltaTime * rotateSpeed);

			//Turn off if camera is focus
			if(camRotate ==1)
			{
				mainRotate = 0;
			}
			break;
		case 2:	//Set Rotate Towards
			Vector3 rotatePos = rotateObject.position - mainNode.position;
			rotatePos.y = 0;
			Quaternion rotateTowards = Quaternion.LookRotation (rotatePos);
			transform.rotation = Quaternion.Slerp (mainNode.rotation, rotateTowards, Time.deltaTime * 5000f);

			//Turn off if camera is focus
			if(camRotate ==1)
			{
				mainRotate = 0;
			}
			break;
		case 3:	//Set Animated
			if(mainNode)
			{
				mainNode.rotation = animationObject.rotation;
			}
			if(!animationObject)
			{
				mainRotate = 0;
			}
			break;
		case 4:	//Orbit
			mainNode.Rotate (Vector3.up * orbitDirection, rotateSpeed * Time.deltaTime);
			break;
		}

		switch(nodeMove)
		{
		case 0:	//Do Nothing
			break;
		case 1:	//Set Position
			camNode.position = cameraPositions[currentPosition].position;
			nodeMove = 0;
			break;
		case 2:	//Zoom
			camNode.Translate((Vector3.forward * zoomDirection) * moveSpeed * Time.deltaTime, Space.Self);
			break;
		}

		switch(camRotate)
		{
		case 0:	//Do Nothing
			break;
		case 1:	//Set Focus
			Vector3 newTarget = new Vector3(focusTarget.position.x, 
			                                focusTarget.position.y + focusHeight, 
			                                focusTarget.position.z);
			Vector3 targetDir = newTarget - cam.position;
			float speed = rotateSpeed * Time.deltaTime;
			Vector3 newDir = Vector3.RotateTowards (cam.forward, targetDir, speed, 0.0f);
			cam.rotation = Quaternion.LookRotation (newDir);
			break;
		case 2:	//Set Focus Origin
			cam.localPosition = Vector3.zero;
			cam.localEulerAngles = Vector3.zero;
			camRotate = 0;
			break;
		case 3:	//Pan - Rotate Left or Right
			cam.Rotate (Vector3.up * panDirection, rotateSpeed * Time.deltaTime);
			break;
		case 4:	//Tilt - Rotate Up or Down
			cam.Rotate (Vector3.right * tiltDirection, rotateSpeed * Time.deltaTime);
			break;
		}

		if(increaseMoveSpeed)
		{
			moveSpeed += Time.deltaTime;
		}

		if(increaseRotateSpeed)
		{
			rotateSpeed += Time.deltaTime;
		}
	}

	//This Procedure is used to Change the Position of the Camera
	public void SetPosition(int _index)
	{
		//Set Current Position
		currentPosition = _index;

		camNode.position = cameraPositions[currentPosition].position;

		nodeMove = 1;
	}

	//This Procedure is called to Move the Main node towards an object
	public void SetMoveTo(GameObject _object)
	{
		moveToPosition = _object.transform.position;
		mainMove = 1;
	}

	//This Procedure is called to Rotate the Main node towards an object
	public void SetLookAt(GameObject _object)
	{
		lookAtPosition = _object.transform.position;
		mainRotate = 1;
	}

	//This Procedure is called to Focus the camera onto a target
	public void SetFocus(GameObject _object)
	{
		focusTarget = _object.transform;
		camRotate = 1;
	}

	public void SetFocusHeight(float _height)
	{
		focusHeight = _height;
	}

	public void SetFocusOrigin()
	{
		camRotate = 2;
	}

	//This Procedure is called to Set the movement speed of the camera
	public void SetMoveSpeed(float _moveSpeed)
	{
		moveSpeed = _moveSpeed;
	}

	//This Prcedure is called to set the rotation speed of the camera
	public void SetRotateSpeed(float _rotateSpeed)
	{
		rotateSpeed = _rotateSpeed;
	}

	//This Procedure is called to zoom the camera 
	public void Zoom(int _direction)
	{
		zoomDirection = _direction;
		nodeMove = 2;
	}

	//Pan - Rotate Left or Right
	public void Pan(int _direction)
	{
		panDirection = _direction;
		camRotate = 3;
	}

	//Tilt - Rotate Up or Down
	public void Tilt(int _direction)
	{
		tiltDirection = _direction;
		camRotate = 4;
	}

	//Truck - Move Left or Right
	public void Truck(int _direction)
	{
		truckDirection = _direction;
		mainMove = 2;
	}

	//Pedestal - Move Up or Down
	public void Pedestal (int _direction)
	{
		pedestalDirection = _direction;
		mainMove = 3;
	}

	//Orbit
	public void Orbit (int _direction)
	{
		orbitDirection = _direction;
		mainRotate = 4;
	}

	//Set distance
	public void SetDistance (float _meters)
	{
		cameraPositionsNode.localScale = new Vector3(_meters,_meters,_meters);
	}

	//Set height
	public void SetHeight(float _meters)
	{
		Vector3 newPos = new Vector3(cameraPositionsNode.localPosition.x, 
		                             _meters, cameraPositionsNode.localPosition.z);
		cameraPositionsNode.localPosition = newPos;

		Vector3 camPos = new Vector3(cam.position.x, 
		                             cameraPositions[currentPosition].position.y,
		                             cam.position.z);
		cam.position = camPos;
	}

	//Set Follow
	public void SetFollow(GameObject _object)
	{
		followObject = _object.transform;
		mainMove = 5;
	}

	//Set Rotate Towards
	public void SetRotateTowards(GameObject _object)
	{
		rotateObject = _object.transform;
		mainRotate = 2;
	}

	//Set Animated
	public void SetAnimated(GameObject _object)
	{
		SetOrigins();
		CameraReset ();
		Stop ();
		animationObject = _object.transform;
		mainRotate = 3;
		mainMove = 6;
	}

	//Screen Effect
	public void ScreenEffect(int _index)
	{
		if(effectScreen)
		{
			Transform spawn = Instantiate (screenEffects[_index], effectScreen.position, effectScreen.rotation) as Transform;
			spawn.SetParent (effectScreen);
		}
	}

	public void ScreenEffect(GameObject _object)
	{
		if(effectScreen)
		{
			Transform spawn = Instantiate (_object.transform, effectScreen.position, effectScreen.rotation) as Transform;
			spawn.SetParent (effectScreen);
		}
	}

	public void SetTransform(Vector3 _position)
	{
		mainNode.position = _position;
	}

	public void SetRotation(Vector3 _rotation)
	{
		mainNode.eulerAngles = _rotation;
	}
	
	public void SpawnGlobalAnimation(Transform _object, string _trigger, float _destroyTime)
	{
		CameraReset ();
		Stop ();

		//For smooth transitions
		ScreenEffect (0);

		//Check if a global animator exists
		if(currentGlobalAnimator)
		{
			//Destroy to start replacement
			Destroy (currentGlobalAnimator.gameObject);
		}

		//Spawn Global Node
		Transform animator;
		if(_object)
		{
			animator = Instantiate (globalCameraAnimator, _object.position, _object.rotation) as Transform;
		}
		else
		{
			animator = Instantiate (globalCameraAnimator, Vector3.zero, Quaternion.identity) as Transform;
		}
		//Access Animator
		currentGlobalAnimator = animator.gameObject.GetComponent<Animator>();
		//Start the Animation
		currentGlobalAnimator.SetTrigger (_trigger);
		//Set Animated Object
		Transform animatorNode = currentGlobalAnimator.gameObject.GetComponentsInChildren <Transform>()[1];
		SetAnimated (animatorNode.gameObject);
		//Set Destroy Time
		Destroy (currentGlobalAnimator.gameObject, _destroyTime);
	}

	public void SpawnGlobalAnimation(Transform _object, string _trigger)
	{
		CameraReset ();
		Stop ();

		//For smooth transitions
		ScreenEffect (0);
		
		//Check if a global animator exists
		if(currentGlobalAnimator)
		{
			//Destroy to start replacement
			Destroy (currentGlobalAnimator.gameObject);
		}
		
		//Spawn Global Node
		Transform animator;
		if(_object)
		{
			animator = Instantiate (globalCameraAnimator, _object.position, _object.rotation) as Transform;
		}
		else
		{
			animator = Instantiate (globalCameraAnimator, Vector3.zero, Quaternion.identity) as Transform;
		}
		//Access Animator
		currentGlobalAnimator = animator.gameObject.GetComponent<Animator>();
		//Start the Animation
		currentGlobalAnimator.SetTrigger (_trigger);
		//Set Animated Object
		Transform animatorNode = currentGlobalAnimator.gameObject.GetComponentsInChildren <Transform>()[1];
		SetAnimated (animatorNode.gameObject);
	}
}
