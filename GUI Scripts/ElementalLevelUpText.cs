using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ElementalLevelUpText : MonoBehaviour 
{	
	private Text levelText;
	public int element;

	void Start()
	{
		levelText = gameObject.GetComponent<Text>();
	}

	// Update is called once per frame
	void Update () 
	{
		switch(element)
		{
		case 1:
			levelText.text = CombatUIManager.mainPlayerScript.combatStats.character.earthAffinity.ToString ();
			break;
		case 2:
			levelText.text = CombatUIManager.mainPlayerScript.combatStats.character.fireAffinity.ToString ();
			break;
		case 3:
			levelText.text = CombatUIManager.mainPlayerScript.combatStats.character.lightningAffinity.ToString ();
			break;
		case 4:
			levelText.text = CombatUIManager.mainPlayerScript.combatStats.character.waterAffinity.ToString ();
			break;
		}
	}
}
