using UnityEngine;
using System.Collections;

//Script Objective: Go Back to Intro Menu when Right Click 

public class BackToIntroMenu : MonoBehaviour 
{
	public Animator anim;

	public string menu;

	private float timer;
	private float maxTimer = 0.5f;

	public SequencerScript sequencer;
	public CombatUIManager ui;
	public UIFade turnOfActionFade;
	public UIFade partyLayoutFade;

	void OnEnable()
	{
		timer = maxTimer;
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If Right Click or L1 is pressed go back to Intro Menu
		if(timer <= 0f)
		{
			if(Input.GetButtonUp ("Back") && !ui.codexMode)
			{
				//Figure out if it's ok to go back. Make sure nothing in Sequencer
				if(ui.currentBars.Length <= 0)
				{
					anim.SetTrigger (menu);
					sequencer.ClearSequence ();
					sequencer.CreateSequence (CombatUIManager.mainPlayerScript.combatStats.character.name);
					turnOfActionFade.FadeTrigger (true);
					partyLayoutFade.FadeTrigger (true);
					timer = maxTimer;
					GameObject.FindGameObjectWithTag("Combat Selector").SendMessage ("ColliderSwitch", false, SendMessageOptions.DontRequireReceiver);
					//Hide the Action Description
					ui.actionDescription.gameObject.SendMessage ("FadeTrigger", false, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					if(ui.chargeSliderObject)
					{
						ui.ClearLevelBars();
						ui.ClearChargeSlider ();
					}
					else
					{
						ui.DeactivateLevelBar(CombatUIManager.mainPlayerScript.currentLevelBar);
					}
				}
			}
		}
		else
		{
			timer -= Time.deltaTime;
		}
	}
}
