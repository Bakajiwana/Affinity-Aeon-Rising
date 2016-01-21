using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Script Objective: Control the cross menus that appear for selection

public class CrossMenu : MonoBehaviour 
{
	//Button Variables
	public Button[] crossMenuBtns; 

	private int currentButton = 0;

	private float timer = 0.4f;

	public bool isDirectionalMenu;

	public Transform selector;

	private CombatUIManager ui;

	// Use this for initialization
	void Start () 
	{
		//Start selected
		//EventSystem.current.SetSelectedGameObject (crossMenuBtns[buttonSelected].gameObject);

		InputController.usingController = true;

		ui = GameObject.FindGameObjectWithTag ("Combat UI").GetComponent<CombatUIManager>();
	}

	void OnEnable()
	{
		timer = 0.4f;
		EventSystem.current.SetSelectedGameObject (crossMenuBtns[currentButton].gameObject);
	}

	void InitiateButton()
	{
		if(crossMenuBtns[currentButton].interactable)
		{
			//Call the Button
			crossMenuBtns[currentButton].onClick.Invoke ();


			//Play Sound
			//ui.nextSound.PlayOneShot(ui.nextAudioClip);

			timer = 0.4f;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Make sure the GetAxis detection doesn't double press button
		if(timer <= 0f && Time.timeScale == 1f)
		{
			//Custom Navigation for the Cross Menu Buttons
			if(Input.GetAxis ("VerticalWithMouse") >= 0.9f)
			{
				//Reveal Selector
				if(selector)
				{
					selector.gameObject.SetActive (true);
				}
				
				currentButton = 0;
				
				//If using a mouse then show selected button
				EventSystem.current.SetSelectedGameObject (crossMenuBtns[0].gameObject);
				
				//Rotate Compass
				if(selector)
				{
					selector.eulerAngles = new Vector3 (0f, 0f, 90f);

					//Play Hover Sound
					ui.hoverSound.PlayOneShot(ui.hoverAudioClip);

					timer = 0.2f;
				}
			}
			if(Input.GetAxis ("VerticalWithMouse") <= -0.9f)
			{
				//Reveal Selector
				if(selector)
				{
					selector.gameObject.SetActive (true);
				}
				
				currentButton = 1;
				
				EventSystem.current.SetSelectedGameObject (crossMenuBtns[1].gameObject);
				
				//Rotate Compass
				if(selector)
				{
					selector.eulerAngles = new Vector3 (0f, 0f, 270f);

					//Play Hover Sound
					ui.hoverSound.PlayOneShot(ui.hoverAudioClip);

					timer = 0.2f;
				}
			}
			if(Input.GetAxis ("HorizontalWithMouse") >= 0.9f)
			{
				//Reveal Selector
				if(selector)
				{
					selector.gameObject.SetActive (true);
				}
				
				currentButton = 2;
				
				EventSystem.current.SetSelectedGameObject (crossMenuBtns[2].gameObject);
				
				//Rotate Compass
				if(selector)
				{
					selector.eulerAngles = new Vector3 (0f, 0f, 0f);

					//Play Hover Sound
					ui.hoverSound.PlayOneShot(ui.hoverAudioClip);

					timer = 0.2f;
				}
			}
			if(Input.GetAxis ("HorizontalWithMouse") <= -0.9f)
			{
				//Reveal Selector
				if(selector)
				{
					selector.gameObject.SetActive (true);
				}
				
				currentButton = 3;
				
				EventSystem.current.SetSelectedGameObject (crossMenuBtns[3].gameObject);	
				
				//Rotate Compass
				if(selector)
				{
					selector.eulerAngles = new Vector3 (0f, 0f, 180f);

					//Play Hover Sound
					ui.hoverSound.PlayOneShot(ui.hoverAudioClip);

					timer = 0.2f;
				}
			}
			
			if(Input.GetMouseButtonUp(0))
			{
				//EventSystem.current.SetSelectedGameObject (null);
				InitiateButton ();
				ui.nextSound.PlayOneShot(ui.nextAudioClip);
				EventSystem.current.SetSelectedGameObject (crossMenuBtns[currentButton].gameObject);
			}		

			if(Input.GetButtonDown ("Submit") || Input.GetMouseButtonDown(1))
			{
				//Play Sound
				ui.nextSound.PlayOneShot(ui.nextAudioClip);
			}

			if(Input.GetButtonDown ("Cancel"))
			{
				//Play Sound
				ui.backSound.PlayOneShot(ui.backAudioClip);
			}
		}
		else
		{
			timer -= Time.deltaTime;
		}

		/* OLD
		//Make sure the GetAxis detection doesn't double press button
		if(timer <= 0f)
		{
			if(!InputController.usingController)
			{
				//Custom Navigation for the Cross Menu Buttons
				if(Input.GetAxis ("VerticalWithMouse") >= 0.9f)
				{
					//Reveal Selector
					if(selector)
					{
						selector.gameObject.SetActive (true);
					}

					currentButton = 0;

					//If using a mouse then show selected button
					EventSystem.current.SetSelectedGameObject (crossMenuBtns[0].gameObject);

					//Rotate Compass
					if(selector)
					{
						selector.eulerAngles = new Vector3 (0f, 0f, 90f);
					}
				}
				if(Input.GetAxis ("VerticalWithMouse") <= -0.9f)
				{
					//Reveal Selector
					if(selector)
					{
						selector.gameObject.SetActive (true);
					}

					currentButton = 1;

					EventSystem.current.SetSelectedGameObject (crossMenuBtns[1].gameObject);

					//Rotate Compass
					if(selector)
					{
						selector.eulerAngles = new Vector3 (0f, 0f, 270f);
					}
				}
				if(Input.GetAxis ("HorizontalWithMouse") >= 0.9f)
				{
					//Reveal Selector
					if(selector)
					{
						selector.gameObject.SetActive (true);
					}

					currentButton = 2;

					EventSystem.current.SetSelectedGameObject (crossMenuBtns[2].gameObject);

					//Rotate Compass
					if(selector)
					{
						selector.eulerAngles = new Vector3 (0f, 0f, 0f);
					}
				}
				if(Input.GetAxis ("HorizontalWithMouse") <= -0.9f)
				{
					//Reveal Selector
					if(selector)
					{
						selector.gameObject.SetActive (true);
					}

					currentButton = 3;

					EventSystem.current.SetSelectedGameObject (crossMenuBtns[3].gameObject);	

					//Rotate Compass
					if(selector)
					{
						selector.eulerAngles = new Vector3 (0f, 0f, 180f);
					}
				}

				if(Input.GetMouseButtonUp(0))
				{
					//EventSystem.current.SetSelectedGameObject (crossMenuBtns[currentButton].gameObject);
					InitiateButton ();
				}
			}
			else
			{
				if(isDirectionalMenu)
				{
					//Custom Navigation for the Cross Menu Buttons
					if(Input.GetAxis ("VerticalWithMouse") >= 0.9f || Input.GetButtonUp ("Lightning"))
					{
						currentButton = 0;

						EventSystem.current.SetSelectedGameObject (crossMenuBtns[0].gameObject);

						//Hide Selector
						if(selector)
						{
							selector.gameObject.SetActive (false);
						}

						InitiateButton ();
					}
					if(Input.GetAxis ("VerticalWithMouse") <= -0.9f || Input.GetButtonUp ("Earth"))
					{
						currentButton = 1;
						
						EventSystem.current.SetSelectedGameObject (crossMenuBtns[1].gameObject);

						//Hide Selector
						if(selector)
						{
							selector.gameObject.SetActive (false);
						}
						
						InitiateButton ();
					}
					if(Input.GetAxis ("HorizontalWithMouse") >= 0.9f || Input.GetButtonUp ("Fire"))
					{
						currentButton = 2;
						
						EventSystem.current.SetSelectedGameObject (crossMenuBtns[2].gameObject);

						//Hide Selector
						if(selector)
						{
							selector.gameObject.SetActive (false);
						}
						
						InitiateButton ();
					}
					if(Input.GetAxis ("HorizontalWithMouse") <= -0.9f || Input.GetButtonUp ("Water"))
					{
						currentButton = 3;
						
						EventSystem.current.SetSelectedGameObject (crossMenuBtns[3].gameObject);

						//Hide Selector
						if(selector)
						{
							selector.gameObject.SetActive (false);
						}
						
						InitiateButton ();	
					}

					if(Input.GetButtonUp ("Submit"))
					{
						if(EventSystem.current.currentSelectedGameObject == null)
						{
							EventSystem.current.SetSelectedGameObject (crossMenuBtns[0].gameObject);
						}
					}
				}
				else
				{
					if(Input.GetButtonUp ("Earth") || Input.GetAxis ("VerticalWithMouse") <= -0.9f)
					{
						currentButton = 1;
						InitiateButton ();

						//Hide Selector
						if(selector)
						{
							selector.gameObject.SetActive (false);
						}
					}
					if(Input.GetButtonUp ("Fire") || Input.GetAxis ("HorizontalWithMouse") >= 0.9f )
					{
						currentButton = 2;
						InitiateButton ();

						//Hide Selector
						if(selector)
						{
							selector.gameObject.SetActive (false);
						}
					}
					if(Input.GetButtonUp ("Lightning") || Input.GetAxis ("VerticalWithMouse") >= 0.9f)
					{
						currentButton = 0;
						InitiateButton ();

						//Hide Selector
						if(selector)
						{
							selector.gameObject.SetActive (false);
						}
					}
					if(Input.GetButtonUp ("Water") || Input.GetAxis ("HorizontalWithMouse") <= -0.9f)
					{
						currentButton = 3;
						InitiateButton ();

						//Hide Selector
						if(selector)
						{
							selector.gameObject.SetActive (false);
						}
					}
				}

				//Controller Description Selection
				if(Input.GetAxis ("RightVertical") >= 0.9f)
				{
					//Reveal Selector
					if(selector)
					{
						selector.gameObject.SetActive (true);
					}

					currentButton = 0;
					
					EventSystem.current.SetSelectedGameObject (crossMenuBtns[0].gameObject);

					//Rotate Compass
					if(selector)
					{
						selector.eulerAngles = new Vector3 (0f, 0f, 90f);
					}
				}
				if(Input.GetAxis ("RightVertical") <= -0.9f)
				{
					//Reveal Selector
					if(selector)
					{
						selector.gameObject.SetActive (true);
					}

					currentButton = 1;
					
					EventSystem.current.SetSelectedGameObject (crossMenuBtns[1].gameObject);

					//Rotate Compass
					if(selector)
					{
						selector.eulerAngles = new Vector3 (0f, 0f, 270f);
					}
				}
				if(Input.GetAxis ("RightHorizontal") >= 0.9f)
				{
					//Reveal Selector
					if(selector)
					{
						selector.gameObject.SetActive (true);
					}

					currentButton = 2;
					
					EventSystem.current.SetSelectedGameObject (crossMenuBtns[2].gameObject);

					//Rotate Compass
					if(selector)
					{
						selector.eulerAngles = new Vector3 (0f, 0f, 0f);
					}
				}
				if(Input.GetAxis ("RightHorizontal") <= -0.9f)
				{
					//Reveal Selector
					if(selector)
					{
						selector.gameObject.SetActive (true);
					}

					currentButton = 3;
					
					EventSystem.current.SetSelectedGameObject (crossMenuBtns[3].gameObject);

					//Rotate Compass
					if(selector)
					{
						selector.eulerAngles = new Vector3 (0f, 0f, 180f);
					}
				}
			}
		}
		else
		{
			timer -= Time.deltaTime;
		}
		*/
	}
}
