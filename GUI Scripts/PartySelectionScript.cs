using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PartySelectionScript : MonoBehaviour 
{
	public int maxCharacters = 2;
	private int currCharacters = 0;

	public Transform[] selected;

	public Text numberCharacter;

	private TravelManager currentTeleporter;

	void Start()
	{
		gameObject.SetActive(false);

		for(int i = 0; i < selected.Length; i++)
		{
			selected[i].gameObject.SetActive (false);
		}
	}

	public void SetCurrentTeleport(TravelManager _currentTeleporter)
	{
		currentTeleporter = _currentTeleporter;
	}

	public void SelectCharacter(int _index)
	{

		if(selected[_index].gameObject.activeInHierarchy)
		{
			selected[_index].gameObject.SetActive (false);
			currCharacters --;
		}
		else
		{
			if(currCharacters < maxCharacters)
			{
				selected[_index].gameObject.SetActive (true);
				currCharacters ++;
			}
		}


		if(numberCharacter)
		{
			numberCharacter.text = (maxCharacters - currCharacters).ToString ();
		}
	}

	public void SubmitCharacter()
	{
		for(int i = 0; i < selected.Length; i++)
		{
			SaveLoadManager.selectedCharacters[i+1] = selected[i].gameObject.activeInHierarchy;
		}

		SaveLoadManager.selectedCharacters[0] = true; //always, because main character....

		currentTeleporter.InitiateTravel (currentTeleporter.destination, currentTeleporter.levelName);
	}
}
