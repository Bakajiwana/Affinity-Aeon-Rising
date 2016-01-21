using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatLevelReveal : MonoBehaviour 
{
	public int inputLevel = 1;

	// Use this for initialization
	void Start () 
	{
		if(inputLevel > SaveLoadManager.saveStats.level)
		{
			gameObject.SetActive (false);
		}
		else
		{
			gameObject.SetActive (true);
			gameObject.GetComponent<CanvasGroup>().alpha = 1f;
		}
	}
}
