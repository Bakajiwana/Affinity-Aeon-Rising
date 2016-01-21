using UnityEngine;
using System.Collections;

public class SymbolActionModeSwitch : MonoBehaviour 
{
	public GameObject[] actionSymbols;
	
	// Update is called once per frame
	void Update () 
	{
		switch(CombatUIManager.mainPlayerScript.actionMode)
		{
		case 0: //Attack
			actionSymbols[0].SetActive (true);
			actionSymbols[1].SetActive (false);
			actionSymbols[2].SetActive (false);
			actionSymbols[3].SetActive (false);
			break;
		case 1: 	//Def
			actionSymbols[0].SetActive (false);
			actionSymbols[1].SetActive (true);
			actionSymbols[2].SetActive (false);
			actionSymbols[3].SetActive (false);
			break;
		case 2:		//sup
			actionSymbols[0].SetActive (false);
			actionSymbols[1].SetActive (false);
			actionSymbols[2].SetActive (true);
			actionSymbols[3].SetActive (false);
			break;
		case 3:		//curse
			actionSymbols[0].SetActive (false);
			actionSymbols[1].SetActive (false);
			actionSymbols[2].SetActive (false);
			actionSymbols[3].SetActive (true);
			break;
		}
	}
}
