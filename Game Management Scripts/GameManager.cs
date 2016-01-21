using UnityEngine;
using System.Collections;

//Handles the Whole Game

public class GameManager : MonoBehaviour 
{
	public string[] combatLevels;

	AsyncOperation async;

	private bool allowAsync = true;

	private float startBattleDelay = 1f;
	private bool startBattle = false;


	// Use this for initialization
	void Start () 
	{
		startBattleDelay = 1f;
		startBattle = false;

		if(!Application.isEditor)
		{
			allowAsync = true;
		}
		else
		{
			allowAsync = true;
		}

		//Asynchronously load a level
		LoadRandomCombatLevel();
		//Application.LoadLevel ("Combat Prototype Scene");

		//Look for a Combat Information Node at the start
		if(GameObject.FindGameObjectWithTag ("Combat Spawner"))
		{
			//If Found Destroy it and obtain overworld information

			Destroy (GameObject.FindGameObjectWithTag ("Combat Spawner"));
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		/* Test Buttons Overworld to Combat Switch
		if(Input.GetKeyDown (KeyCode.O))
		{
			CombatSceneActivate ();
		}
		if(Input.GetKeyDown (KeyCode.P))
		{
			GameObject.FindGameObjectWithTag ("Combat Manager").SendMessage ("ReturnToOverworld", SendMessageOptions.DontRequireReceiver);
		}
		*/

		if(startBattle)
		{
			if(startBattleDelay > 0f)
			{
				startBattleDelay -= Time.unscaledDeltaTime;
			}
			else
			{
				if(!CharacterManager.isBusy)
				{
					async.allowSceneActivation = true;
				}
			}
		}
	}

	public void LoadRandomCombatLevel()
	{
		int randomLevel = Random.Range (0, combatLevels.Length - 1);
		if(allowAsync)
		{
			StartCoroutine (LoadCombatLevel(combatLevels[randomLevel]));
		}
		Application.backgroundLoadingPriority = ThreadPriority.Normal;
	}
	
	IEnumerator LoadCombatLevel(string level)
	{
		async = Application.LoadLevelAsync (level);
		async.allowSceneActivation = false;
		yield return async;
	}

	public void CombatSceneActivate()
	{
		//Application.LoadLevel ("Combat Prototype Scene");
		if(allowAsync)
		{
			startBattle = true;
			Time.timeScale = 0.1f;
			GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<UIFade>().FadeTrigger (true);
			GlowEffect effect = Camera.main.gameObject.GetComponent<GlowEffect>();
			effect.enabled = true;
		}
		else
		{
			int randomLevel = Random.Range (0, combatLevels.Length - 1);
			Application.LoadLevel (combatLevels[randomLevel]);
		}
	}
}
