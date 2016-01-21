using UnityEngine;
using System.Collections;

public class SupportEmergency : MonoBehaviour 
{
	public bool indicateForLowHealth = false;
	public float lowHealthPercent = 0.3f;

	public bool indicateForLowAP = false;
	public float lowAPPercent = 0.3f;

	private bool indicate = false;

	private float currentHealthPercent;
	private float currentAPPercent;

	public UIFade emergencyButton;

	// Use this for initialization
	void Start () 
	{	
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(CombatUIManager.mainPlayerScript)
		{
			//If Indicating for low health
			if(indicateForLowHealth)
			{
				currentHealthPercent = (float)CombatUIManager.mainPlayerScript.combatStats.stat.health /
					(float)CombatUIManager.mainPlayerScript.combatStats.stat.healthMax;
			}

			//If Indicating for low ap
			if(indicateForLowAP)
			{
				currentAPPercent = (float)CombatUIManager.mainPlayerScript.combatStats.stat.actionPoints /
					(float)CombatUIManager.mainPlayerScript.combatStats.stat.actionPointMax;
			}


			//If only indicating for health
			if(indicateForLowHealth && !indicateForLowAP)
			{
				if(currentHealthPercent < lowHealthPercent)
				{
					if(!indicate)
					{
						IndicateEmergency (true);
					}
				}
				else
				{
					if(indicate)
					{
						IndicateEmergency (false);
					}
				}
			}
			//If only indicating for ap
			else if (!indicateForLowHealth && indicateForLowAP)
			{
				if(currentAPPercent < lowAPPercent)
				{
					if(!indicate)
					{
						IndicateEmergency (true);
					}
				}
				else
				{
					if(indicate)
					{
						IndicateEmergency (false);
					}
				}
			}
			//If indicating for both
			else if(indicateForLowHealth && indicateForLowAP)
			{
				if(currentHealthPercent < lowHealthPercent || currentAPPercent < lowAPPercent)
				{
					if(!indicate)
					{
						IndicateEmergency (true);
					}
				}
				else
				{
					if(indicate)
					{
						IndicateEmergency (false);
					}
				}
			}
		}
	}

	void IndicateEmergency(bool _indicate)
	{
		if(!_indicate)
		{
			emergencyButton.animateFade = false;
			emergencyButton.fade = false;
			indicate = false;
		}
		else
		{
			emergencyButton.animateFade = true;
			emergencyButton.fade = true;
			indicate = true;
		}
	}
}
