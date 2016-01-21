using UnityEngine;
using System.Collections;

public class UtilityTextScript : MonoBehaviour 
{
	private Transform parentNode;
	private UIFade parentLayout;

	public float timer = 2f;

	// Use this for initialization
	void Start () 
	{
		Invoke ("Initialise", 0.2f);
	}

	void Initialise()
	{
		parentNode = transform.parent;
		parentLayout = parentNode.gameObject.GetComponent<UIFade>();
		
		parentLayout.FadeTrigger (true);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(timer > 0f)
		{
			timer -= Time.deltaTime;

			if(timer <= 0f)
			{
				DestroySelf ();
			}
		}
	}

	void DestroySelf()
	{
		if(parentNode.childCount == 1)
		{
			Destroy (gameObject, 1f);
			parentLayout.FadeTrigger (false);
		}
		else
		{
			Destroy (gameObject);
		}
	}
}
