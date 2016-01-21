using UnityEngine;
using System.Collections;

//This Script is to Control the distance of an object via other objects, mainly controlling the specifed objects z axis

public class DistanceScript : MonoBehaviour 
{
	public Transform target;	

	public void SetDistance(Vector3 _position)
	{
		Vector3 newPos = new Vector3 (target.position.x, target.position.y, target.position.z);

		newPos.x = _position.x;

		newPos.z = _position.z;

		newPos.y = Random.Range (0f, _position.y);

		target.localPosition = newPos;
	}
}
