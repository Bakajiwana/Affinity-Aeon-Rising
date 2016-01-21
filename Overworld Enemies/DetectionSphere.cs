using UnityEngine;
using System.Collections;

public class DetectionSphere : MonoBehaviour 
{
	//This script will detect player and update parent
	void Start()
	{
		transform.localEulerAngles = Vector3.zero;
	}

	public void CreateDetectionSphere()
	{
		//From this Game Object create a rigidbody and create a sphere collider with 4m radius and 2m forward
		Rigidbody rigbod = gameObject.AddComponent<Rigidbody>();
		rigbod.useGravity = false;
		
		SphereCollider col = gameObject.AddComponent<SphereCollider>();
		col.isTrigger = true;
		col.radius = 4f;
		col.center = new Vector3(0f,0f,2f);
	}

	public void CreateDetectionSphere(float _distance, float _radius)
	{
		//From this Game Object create a rigidbody and create a sphere collider with 4m radius and 2m forward
		Rigidbody rigbod = gameObject.AddComponent<Rigidbody>();
		rigbod.useGravity = false;
		
		SphereCollider col = gameObject.AddComponent<SphereCollider>();
		col.isTrigger = true;
		col.radius = _radius;
		col.center = new Vector3(0f,0f,_distance);
	}

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.CompareTag ("Player"))
		{
			transform.parent.gameObject.SendMessage ("Detected", other.gameObject.transform, SendMessageOptions.DontRequireReceiver);
		}
	}
}
