using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Script Objective: Manage the Hover Info Tasks

public class HoverInfoManager : MonoBehaviour 
{
	public RectTransform hoverRect;
	public Text hoverText;
	public CanvasGroup hoverCanvas;

	private bool hover = false;

	public float hoverActivateMaxTimer = 1f;
	private float hoverActivateTimer;

	// Use this for initialization
	void Start () 
	{
		hoverActivateTimer = hoverActivateMaxTimer;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(hover)
		{
			hoverActivateTimer -= Time.deltaTime;

			if(hoverActivateTimer <= 0f)
			{
				HoverReveal ();
				hover = false;
			}
		}
	}

	public void HoverActivate()
	{
		hover = true;
		hoverActivateTimer = hoverActivateMaxTimer;
	}

	public void HoverDeactivate()
	{
		hover = false;
		hoverActivateTimer = hoverActivateMaxTimer;
		hoverCanvas.alpha = 0f;
		hoverCanvas.blocksRaycasts = false;
	}

	public void HoverReveal()
	{
		hoverCanvas.alpha = 1f;
		hoverCanvas.blocksRaycasts = true;
	}

	public void SetText(string _text)
	{
		hoverText.text = _text;
	}

	public void SetCurrentPosition(Transform _position)
	{
		hoverRect.position = _position.position;
	}

	public void SetPivot (int _corner)
	{
		switch(_corner)
		{
		case 1:	//Upper Left
			hoverRect.pivot = new Vector2(0f, 1f);
			break;
		case 2:	//Upper Right
			hoverRect.pivot = new Vector2(1f, 1f);
			break;
		case 3:	//Lower Left
			hoverRect.pivot = new Vector2(0f, 0f);
			break;
		case 4:	//Lower Right
			hoverRect.pivot = new Vector2(1f, 0f);
			break;
		}
	}

	public void SetElementLevelText(int _element)
	{
		//Find the main Player
		for(int i = 0; i < CombatManager.playerStats.Count; i++)
		{
			if(CombatManager.playerStats[i].isMain == true)
			{
				Character character = CombatManager.playerStats[i].character;

				//Set String
				switch(_element)
				{
				case 1:
					hoverText.text = "Lv. " + character.earthAffinity.ToString () + " Earth (E)";
					break;
				case 2:
					hoverText.text = "Lv. " + character.fireAffinity.ToString ()  + " Fire (Q)";
					break;
				case 3:
					hoverText.text = "Lv. " + character.lightningAffinity.ToString ()  + " Lightning (R)";
					break;
				case 4:
					hoverText.text = "Lv. " + character.waterAffinity.ToString ()  + " Water (W)";
					break;
				}			
				break;
			}
		}
	}
}
