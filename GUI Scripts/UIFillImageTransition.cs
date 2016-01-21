using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIFillImageTransition : MonoBehaviour 
{
	private bool fillImage;
	public float fillSpeed = 2f;

	public Image fillTarget;

	// Use this for initialization
	void Start () 
	{
		fillImage = true;
		fillTarget.fillAmount = 0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(fillImage)
		{
			fillTarget.fillAmount += Time.deltaTime * fillSpeed;

			if(fillTarget.fillAmount >= 1f)
			{
				this.enabled = false;
			}
		}
	}
}
