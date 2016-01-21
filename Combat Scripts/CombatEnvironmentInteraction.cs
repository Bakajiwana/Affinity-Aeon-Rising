using UnityEngine;
using System.Collections;

//This Script Will Control the Environment Interactible Objects

public class CombatEnvironmentInteraction : MonoBehaviour 
{
	private Animator anim;

	public string objectName;
	public int index = 0;
	public int [] elementReaction;	//0 - Desired, 1 - Inapplicable, 2 - Undesired
	private int [] currentReaction = new int[4] {3, 3, 3, 3};	//3 - Unknown
	[Range(0, 100)]
	public int revealPercentage = 50;

	public string[] notes;

	private CombatUIManager ui; 

	//Reactant Values: Desired, Inapplicable, Undesired. If not revealed then marked as unknown

	// Use this for initialization
	void Awake () 
	{
		anim = gameObject.GetComponent<Animator>();

		//Get the UI component
		ui = GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<CombatUIManager>();
	}
	
	void EnvironmentInteract(int _element) //1 = Earth, 2 = Fire, 3 = Lightning, 4 = Water
	{
		switch(_element)
		{
		case 1:
			anim.SetInteger ("Earth", anim.GetInteger ("Earth")+ 1);
			break;
		case 2:
			anim.SetInteger ("Fire", anim.GetInteger ("Fire")+ 1);
			break;
		case 3:
			anim.SetInteger ("Lightning", anim.GetInteger ("Lightning")+ 1);
			break;
		case 4:
			anim.SetInteger ("Water", anim.GetInteger ("Water")+ 1);
			break;
		}
	}

	public void DamageClosestEnemy()
	{
		float closest = 0;
		int closestEnemy = 0;
		for(int i = 0; i < CombatManager.enemies.Count; i++)
		{
			float dist = Vector3.Distance (CombatManager.enemies[i].transform.position, transform.position);
			if(dist < closest)
			{
				closest = dist;
				closestEnemy = i;
			}
		}

		CombatManager.enemyStats[closestEnemy].SetStunned (3);
	}
	
	//This Function is called to send information to Codex
	void SetCodex()
	{
		//Send the Set Character Codex Function
		ui.SetObjectCodex (currentReaction, objectName, notes);
	}


	public void SetRevealedScan(int[] _scanValues)
	{
		for(int i = 0; i < 4; i++)
		{
			int randomChance = Random.Range (0, 100);

			if(randomChance <= _scanValues[i])
			{
				//Reveal Element Information 
				currentReaction[i] = elementReaction[i];
			}
		}
	}

	public void SetHiddenScan(int[] _values)
	{
		//In a random chance figure out whether revealed from the percentage set
		int randomChance = Random.Range (0, 100);

		if(randomChance <= revealPercentage)
		{
			//Reveal this Object - Send Message to Combat Manager to Include into List
			GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("AddEnvironmentObject", gameObject, SendMessageOptions.DontRequireReceiver);
		}

		SetRevealedScan (_values);
	}

	public void RemoveObject()
	{
		//Send Message to Combat Manager to Remove this object
		GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("RemoveEnvironmentObject", ObjectIndex(), SendMessageOptions.DontRequireReceiver);
	}

	public int ObjectIndex()
	{
		for(int i = 0; i < CombatManager.environmentObjects.Count; i++)
		{
			if(CombatManager.environmentObjects[i] == this.gameObject)
			{
				return i;
			}
		}
		
		return 0;
	}
}
