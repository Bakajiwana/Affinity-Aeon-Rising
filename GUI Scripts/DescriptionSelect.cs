using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DescriptionSelect : MonoBehaviour 
{
	private Animator anim;

	public GameObject actionButton;

	void Start()
	{
		anim = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//If Game Object is selected turn anim bool to reveal
		if(EventSystem.current.currentSelectedGameObject == actionButton)
		{
			anim.SetBool ("Reveal", true);
		}
		else
		{
			//Hide animation
			anim.SetBool ("Reveal", true);
		}
	}
}
