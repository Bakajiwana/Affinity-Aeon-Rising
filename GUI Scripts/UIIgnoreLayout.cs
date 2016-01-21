using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]

public class UIIgnoreLayout : MonoBehaviour 
{
	private LayoutElement layout;
	public float ignoreTimer = 0.1f;

	void Start()
	{
		if(!gameObject.GetComponent<LayoutElement>())
		{
			layout = gameObject.AddComponent<LayoutElement>();
		}
		else
		{
			layout = gameObject.GetComponent<LayoutElement>();
		}

		layout.flexibleHeight = 1f;
		layout.flexibleWidth = 1f;

		Invoke ("Ignore", ignoreTimer);
	}

	void Ignore()
	{
		layout.ignoreLayout = true;
	}
}
