using UnityEngine;
using System.Collections;

public class DialogueTrigger : MonoBehaviour 
{
	private bool[] currentCharacters;
	public bool[] characters;

	public Transform dialogueBox;
	public Transform dialogue;

	void Awake()
	{
		if(!gameObject.GetComponent<CollectiblesScript>())
		{
			CollectiblesScript col = gameObject.AddComponent<CollectiblesScript>();
			col.interactible = false;
		}
	}

	// Use this for initialization
	void Start () 
	{
		if(GameObject.FindGameObjectWithTag ("Adventure Manager"))
		{
			currentCharacters = GameObject.FindGameObjectWithTag ("Adventure Manager").GetComponent<CharacterManager>().companions;

			for(int i = 0; i < currentCharacters.Length; i++)
			{
				if(characters[i] != currentCharacters[i])
				{
					gameObject.SetActive (false);
					break;
				}
			}
		}
		else
		{
			gameObject.SetActive (false);
		}
	}
	
	// Update is called once per frame
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag ("Player"))
		{
			Transform spawn = Instantiate (dialogue, dialogueBox.transform.position, dialogueBox.transform.rotation) as Transform;
			spawn.SetParent (dialogueBox);

			transform.parent.gameObject.SendMessage ("MarkObject", transform, SendMessageOptions.DontRequireReceiver);

			gameObject.SetActive (false);
		}
	}
}
