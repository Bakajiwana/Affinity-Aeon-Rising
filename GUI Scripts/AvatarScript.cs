using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AvatarScript : MonoBehaviour 
{
	private float speed = 500f;

	public float moveTimer = 3f;
	private float timer;

	private bool moveToTarget = false;
	private Transform target;
	public Transform avatar;

	private RectTransform objectSize;

	//Colour Changing
	private bool fast;
	public Image portrait;

	void Awake()
	{
		timer = moveTimer;

		objectSize = avatar.gameObject.GetComponent<RectTransform>();
	}

	void Update()
	{
		if(moveToTarget)
		{
			if(timer >= 0f)
			{
				timer -= Time.deltaTime;

				float step = speed * Time.deltaTime;
				avatar.position = Vector3.MoveTowards(avatar.position, target.position, step);
			}
			else
			{
				moveToTarget = false;
			}
		}
	}

	void MoveToObject(Transform _target)
	{
		target = _target;
		moveToTarget = true;
		timer = moveTimer;
	}

	void ScaleToLength(float _scaleSize)
	{
		objectSize.localScale = new Vector3(_scaleSize, _scaleSize, 1f);
	}

	public void SetStatIndex(bool _isEnemy, int _statIndex)
	{

	}
}
