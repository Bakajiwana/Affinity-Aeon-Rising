using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour 
{
	private CanvasGroup canvas;

	public float fadeSpeed = 1f;

	// Use this for initialization
	void Awake () 
	{
		canvas = gameObject.GetComponent<CanvasGroup>();
	}

	void OnEnable()
	{
		canvas.alpha = 0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(canvas.alpha <= 1f)
		{
			canvas.alpha += Time.deltaTime * fadeSpeed;
		}
	}
}
