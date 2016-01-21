using UnityEngine;
using System.Collections;

public class DialogueSequence : MonoBehaviour 
{
	private UIFillAmountTransition[] sequence; 

	private int currentSequence = 0;

	public bool spawnEvent;
	public GameObject[] spawnObjects;

	public GameObject dialogueChain;
	public bool disableCharacter = true;

	public bool isTimed;
	public float nextDialogueTimer = 5f;

	void Start()
	{
		sequence = gameObject.GetComponentsInChildren<UIFillAmountTransition>();

		sequence[currentSequence].Transition();

		if(isTimed)
		{
			StartCoroutine ("AdvanceDialogue");
		}
	}

	IEnumerator AdvanceDialogue()
	{
		while(true)
		{
			yield return new WaitForSeconds(nextDialogueTimer);
			ForwardDialogue();
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if(disableCharacter)
		{
			CharacterManager.isBusy = true;
		}

		if(Input.GetButtonUp ("Submit") || Input.GetButtonUp ("Earth") || Input.GetMouseButtonUp (0))
		{
			ForwardDialogue();
		}
	}

	void ForwardDialogue()
	{
		currentSequence++;
		
		if(currentSequence == sequence.Length)
		{
			if(spawnEvent)
			{
				for(int i = 0; i < spawnObjects.Length; i++)
				{
					Instantiate (spawnObjects[i], transform.position, transform.rotation);
				}
			}

			if(dialogueChain)
			{
				GameObject spawn = Instantiate (dialogueChain, transform.position, transform.rotation) as GameObject;
				spawn.transform.SetParent(transform.parent);
			}

			CharacterManager.isBusy = false;
			Destroy (gameObject);
		}
		else
		{
			
			sequence[currentSequence].Transition();

			for(int i = 0; i < currentSequence; i++)
			{
				sequence[i].CompleteTransition ();
			}
		}
	}
}
