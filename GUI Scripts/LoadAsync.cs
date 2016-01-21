using UnityEngine;
using System.Collections;

public class LoadAsync : MonoBehaviour 
{
	AsyncOperation async;

	public int levelToLoad = 1;

	IEnumerator Start() 
	{
		async = Application.LoadLevelAsync(levelToLoad);
		async.allowSceneActivation = false;
		yield return async;
	}

	public void ActivateLevel()
	{
		async.allowSceneActivation = true;
	}
}

