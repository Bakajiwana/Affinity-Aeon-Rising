using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SequencerScript : MonoBehaviour 
{
	public GameObject sequencer;
	
	public List<GameObject> sequence = new List<GameObject>();

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void CreateSequence(string _text)
	{
		//Transform panel = Instantiate (partyMemberPanel, partyLayoutPanel.position, partyLayoutPanel.rotation) as Transform;
		GameObject sequencerSpawn = Instantiate (sequencer, transform.position, transform.rotation) as GameObject;
		sequencerSpawn.transform.SetParent (transform,true);
		sequence.Add (sequencerSpawn);

		Text sequenceText = sequencerSpawn.GetComponentInChildren<Text>();
		sequenceText.text = _text;

		Animator anim = sequencerSpawn.GetComponent<Animator>();
		anim.SetTrigger ("Trigger");

		LatestColourChange();
	}

	void LatestColourChange()
	{
		if(sequence[sequence.Count - 1])
		{
			//Make sure latest Sequence shows title more effectively by showing alpha and colour
			Image[] images = sequence[sequence.Count - 1].GetComponentsInChildren<Image>();
			foreach(Image fills in images)
			{
				fills.color = new Color(1f,1f,1f,1f);
			}
		}

		int lastImageIndex = sequence.Count - 2;
		if(lastImageIndex >= 0)
		{
			if(sequence[lastImageIndex])
			{
				Image[] lastImages = sequence[sequence.Count - 2].GetComponentsInChildren<Image>();
				foreach(Image fills in lastImages)
				{
					fills.color = new Color(1f,1f,1f,0.4f);
				}
			}
		}
	}

	public void UpdateLatestSequence(string _text)
	{
		DestroyLatestSequence();

		CreateSequence (_text);
	}

	public void DestroyLatestSequence()
	{
		//Destroy Latest Sequence
		Destroy (sequence[sequence.Count - 1]);
		sequence.Remove (sequence[sequence.Count - 1]);

		LatestColourChange();
	}

	public void ClearSequence()
	{
		sequence = new List<GameObject>();

		//Destroy Every GameObject
		foreach(Transform child in transform)
		{
			Destroy (child.gameObject);
		}
	}
}
