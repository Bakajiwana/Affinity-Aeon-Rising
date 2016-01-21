using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Script Objective: Handles the highlight selection

public class CombatSelection : MonoBehaviour 
{
	public GameObject lockOnImage;
	public CanvasGroup[] localUI;
	[HideInInspector]
	public PartyMemberStatus partyUI;

	// Use this for initialization
	void Start () 
	{
		if(lockOnImage)
		{
			lockOnImage.SetActive (false);
		}


		for(int i = 0; i < localUI.Length; i++)
		{
			localUI[i].alpha = 0f;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag ("Combat Selector"))
		{
			if(lockOnImage)
			{
				//Reveal Lock On Wheel
				lockOnImage.SetActive (true);
			}

			//Reveal Codex Information, Sendmessage to this object to send Codex Information
			gameObject.SendMessage ("SetCodex", SendMessageOptions.DontRequireReceiver);

			//Reveal Local UI
			for(int i = 0; i < localUI.Length; i++)
			{
				localUI[i].alpha = 1f;
			}

			if(partyUI)
			{
				partyUI.selectedPanel.gameObject.SetActive (true);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag ("Combat Selector"))
		{
			if(lockOnImage)
			{
				//Turn off lock on wheel
				lockOnImage.SetActive (false);
			}

			for(int i = 0; i < localUI.Length; i++)
			{
				localUI[i].alpha = 0f;
			}

			if(partyUI)
			{
				partyUI.selectedPanel.gameObject.SetActive (false);
			}
		}
	}
}
