using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DefaultButtonBehaviours : MonoBehaviour 
{
	private Button btn;
	public bool resumeButton;

	// Use this for initialization
	void Start () 
	{
		btn = gameObject.GetComponent<Button>();
		if(resumeButton)
		{
			EventSystem.current.SetSelectedGameObject (gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(resumeButton)
		{
			if(Input.GetButtonUp ("Pause"))
			{
				EventSystem.current.SetSelectedGameObject (gameObject);
			}
			if(Input.GetButtonUp ("Cancel"))
			{
				EventSystem.current.SetSelectedGameObject (gameObject);
			}
		}
		else
		{
			if(Input.GetButtonUp ("Cancel"))
			{
				btn.onClick.Invoke ();
			}
		}
	}

	void LateUpdate()
	{
		if(EventSystem.current.currentSelectedGameObject == null)
		{
			if(InputController.usingController && resumeButton)
			{
				EventSystem.current.SetSelectedGameObject (gameObject);
			}
		}
	}
}
