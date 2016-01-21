using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Script Objective: Async Load to the next specified level by the interlevel traveller (a game object from another level)

public class LoadingScreenTravel : MonoBehaviour 
{
	//public Slider loadProgress;

	public GameObject[] images;

	private string loadingLevel;

	// Use this for initialization
	void Start () 
	{

		//loadProgress.value = 0f;
	}

	void LoadingScreen()
	{
		if(loadingLevel.Equals ("Eidolesse Ruins"))
		{
			images[0].SetActive (true);
		}
		else if(loadingLevel.Equals ("Alshard Valley"))
		{
			images[1].SetActive (true);
		}
		else if(loadingLevel.Equals ("Crystal Cave"))
		{
			images[2].SetActive (true);
		}
		else if(loadingLevel.Equals ("Grenmoss Stretch"))
		{
			images[3].SetActive (true);
		}
		else
		{
			images[Random.Range (0,images.Length)].SetActive (true);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void LoadTargetLevel(string levelToLoad)
	{
		loadingLevel = levelToLoad;
		LoadingScreen();
		StartCoroutine (DisplayLoadingScreen(levelToLoad));
		Application.backgroundLoadingPriority = ThreadPriority.Normal;
	}

	IEnumerator DisplayLoadingScreen(string level)
	{
		yield return new WaitForSeconds(0.3f);
		AsyncOperation async = Application.LoadLevelAsync (level);
		while(!async.isDone)
		{
			//loadProgress.value = async.progress;
			yield return null;
		}
	}
}
