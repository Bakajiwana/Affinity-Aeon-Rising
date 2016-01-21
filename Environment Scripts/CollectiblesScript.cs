using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Script Objective: This Script serves as the collectible... allows the player to collect things

public class CollectiblesScript : MonoBehaviour 
{
	public bool interactible = true;

	private bool available = false;
	private bool collected = false;

	public Animator anim;

	public Transform utilityNode;
	public Transform utilityText;

	public bool pickUpShield = false;
	public bool pickUpHealth = false;
	public bool pickUpAP = false;

	public int maxShield = 400;

	public int shieldGain = 10;
	[Range(0f, 1f)]
	public float healthRestore = 0.30f;
	[Range(0f, 1f)]
	public float apRestore = 0.30f;

	public Transform[] revealObjects;
	public Transform[] hideObjects;

	// Update is called once per frame
	void Update () 
	{
		if(available)
		{
			if(!collected && Input.GetKeyUp (KeyCode.E) ||
			   !collected && Input.GetButtonUp ("Earth"))
			{
				anim.SetTrigger ("Collect");
				AdventureInterface.helpText = "";
				collected = true;

				//print ("collected");

				if(pickUpShield)
				{
					ShieldPickUp ();
				}

				if(pickUpHealth)
				{
					HealthPickUp ();
				}

				if(pickUpAP)
				{
					ApPickUp ();
				}

				//Reveal and hide certain objects
				RevealObjects (true);
				HideObjects (false);

				transform.parent.parent.gameObject.SendMessage ("MarkObject", transform, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	void ShieldPickUp()
	{
		if(SaveLoadManager.saveShield[0] < maxShield)
		{
			for(int i = 0; i < SaveLoadManager.saveShield.Length; i++)
			{
				SaveLoadManager.saveShield[i] += shieldGain;
			}

			SpawnText ("Shields Enhanced +" + shieldGain.ToString ());
		}
		else
		{
			SpawnText ("Shields Maxed");
		}

	}

	void HealthPickUp()
	{
		int hp = (int)((float)SaveLoadManager.saveHealthMax * healthRestore);

		for(int i = 0; i < SaveLoadManager.saveHealth.Length; i++)
		{				
			SaveLoadManager.saveHealth[i] += hp;
		}

		SpawnText ("Health Restored +" + hp.ToString ());
	}

	void ApPickUp()
	{
		int ap = (int)((float)SaveLoadManager.saveAPMax * apRestore);

		for(int i = 0; i < SaveLoadManager.saveAP.Length; i++)
		{
			SaveLoadManager.saveAP[i] += ap;
		}

		SpawnText ("AP Restored +" + ap.ToString ());
	}

	void SpawnText(string _text)
	{
		Transform spawn = Instantiate (utilityText.transform, utilityNode.position, utilityNode.rotation) as Transform;
		spawn.SetParent (utilityNode);
		
		spawn.GetComponent<Text>().text = _text;
	}

	void OnTriggerEnter(Collider other)
	{
		if(interactible)
		{
			if(other.gameObject.CompareTag ("Player"))
			{
				if(!collected)
				{
					if(InputController.usingController)
					{
						AdventureInterface.helpText = "Press A to Collect";
					}
					else
					{
						AdventureInterface.helpText = "Press E to Collect";
					}
				}
				else
				{
					AdventureInterface.helpText = "";
				}
				available = true;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.CompareTag ("Player"))
		{
			AdventureInterface.helpText = "";
			available = false;
		}
	}

	void RevealObjects(bool _active)
	{
		for(int i = 0; i < revealObjects.Length; i++)
		{
			revealObjects[i].gameObject.SetActive (_active);
		}
	}

	void HideObjects(bool _active)
	{
		for(int i = 0; i < hideObjects.Length; i++)
		{
			hideObjects[i].gameObject.SetActive (_active);
		}
	}
}
