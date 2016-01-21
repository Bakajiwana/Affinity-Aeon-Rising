using UnityEngine;
using System.Collections;

public class ExzaliaOverworld : MonoBehaviour 
{
	public CapsuleCollider col;

	public Transform dialogueBox;

	// Use this for initialization
	void Awake () 
	{
		col.enabled = false;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		if(dialogueBox.childCount <= 0)
		{
			if(Application.loadedLevelName.Equals ("Alshard Valley"))
			{
				GameObject.FindGameObjectWithTag ("Adventure Manager").GetComponent<CharacterManager>().companions[1] = true;
			}
			col.enabled = true;
		}
	}
}
