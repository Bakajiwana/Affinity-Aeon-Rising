using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIHoverHide : MonoBehaviour 
{
	public CanvasGroup canvas;

	private bool hideToggle = false;

	public GameObject thing;

	public void CanvasToggle()
	{
		hideToggle = !hideToggle;

		if(hideToggle)
		{
			if(canvas)
			{
				canvas.alpha = 0f;
			}
			if(thing)
			{
				thing.SetActive (false);
			}
		}
		else
		{
			if(canvas)
			{
				canvas.alpha = 1f;
			}
			if(thing)
			{
				thing.SetActive (true);
			}
		}
	}
}
