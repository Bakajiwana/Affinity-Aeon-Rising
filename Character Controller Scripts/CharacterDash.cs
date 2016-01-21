using UnityEngine;
using System.Collections;

public class CharacterDash : MonoBehaviour 
{
	public SkinnedMeshRenderer[] meshes; 
	public Cloth[] clothMeshes;

	public Transform dashEffect;

	public CharacterNavigator character;

	public Transform teleportBeforeEffect;
	public Transform teleportAfterEffect;

	public float dashSpeed = 8f;

	private bool sprinting = false;

	public float maxDelay = 1f;
	private float delay;

	public Color transitionColour;
	private Material[] meshMaterials;
	private Color[] originalColour;
	private float[] originalRimStrength;
	private float[] originalRimPower;

	void Start()
	{
		delay = maxDelay;

		dashEffect.gameObject.SetActive (false);

		originalColour = new Color[meshes.Length];
		meshMaterials = new Material[meshes.Length];
		originalRimStrength = new float[meshes.Length];
		originalRimPower = new float[meshes.Length];

		for(int i = 0; i < meshes.Length; i++)
		{
			meshMaterials[i] = meshes[i].gameObject.GetComponent<Renderer>().material;
			originalColour[i] = meshMaterials[i].GetColor ("_RimColor");
			originalRimPower[i] = meshMaterials[i].GetFloat ("_RimPower");
			originalRimStrength[i] = meshMaterials[i].GetFloat ("_RimStrength");
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if(character)
		{
			if(character.anim.GetFloat ("Speed") >= dashSpeed)
			{
				if(delay > 0f)
				{
					delay -= Time.deltaTime;
					for(int i = 0; i < meshMaterials.Length; i++)
					{
						meshMaterials[i].SetColor ("_RimColor", transitionColour);
						meshMaterials[i].SetFloat ("_RimPower", meshMaterials[i].GetFloat ("_RimPower") - Time.deltaTime * 8f);
						meshMaterials[i].SetFloat ("_RimStrength", meshMaterials[i].GetFloat ("_RimStrength") + Time.deltaTime * 5f);
					}

					if(Input.GetAxis("Triggers") > -0.2f && !Input.GetKey (KeyCode.LeftShift))
					{
						for(int i = 0; i < meshMaterials.Length; i++)
						{
							meshMaterials[i].SetColor ("_RimColor", originalColour[i]);
							meshMaterials[i].SetFloat ("_RimPower", originalRimPower[i]);
							meshMaterials[i].SetFloat ("_RimStrength", originalRimStrength[i]);
						}
					}
				}
				else
				{
					SwitchEffect(true);
				}
			}
			else
			{
				SwitchEffect (false);
				delay = maxDelay;
			}
		}
	}

	void SwitchEffect(bool _switch)
	{
		if(_switch)
		{
			if(!sprinting)
			{
				character.anim.SetTrigger ("Roll");

				//Hide Meshes
				foreach(Cloth fabric in clothMeshes)
				{
					fabric.enabled = false;
				}

				foreach(SkinnedMeshRenderer skin in meshes)
				{
					skin.enabled = false;
				}

				Instantiate (teleportBeforeEffect, transform.position, transform.rotation);

				//Reveal Dash
				dashEffect.gameObject.SetActive (true);

				sprinting = true;
			}
		}
		else
		{
			if(sprinting)
			{
				//Reveal Meshes
				foreach(SkinnedMeshRenderer skin in meshes)
				{
					skin.enabled = true;
				}

				foreach(Cloth fabric in clothMeshes)
				{
					fabric.enabled = true;
				}

				Instantiate (teleportAfterEffect, transform.position, transform.rotation);

				//Hide Dash
				dashEffect.gameObject.SetActive (false);

				for(int i = 0; i < meshMaterials.Length; i++)
				{
					meshMaterials[i].SetColor ("_RimColor", originalColour[i]);
					meshMaterials[i].SetFloat ("_RimPower", originalRimPower[i]);
					meshMaterials[i].SetFloat ("_RimStrength", originalRimStrength[i]);
				}

				sprinting = false;
			}
		}
	}
}
