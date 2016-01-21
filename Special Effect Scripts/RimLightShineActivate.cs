using UnityEngine;
using System.Collections;

public class RimLightShineActivate : MonoBehaviour 
{
	private Material meshMaterials;
	private float originalRimStrength;
	private float originalRimPower;

	[Range(0f,1f)]
	public float rimStrength = 0.6f;

	public float timer = 2f;
	
	void Start()
	{
		if(gameObject.GetComponent<Renderer>().material)
		{
			meshMaterials = gameObject.GetComponent<Renderer>().material;
			originalRimPower = meshMaterials.GetFloat ("_RimPower");
			originalRimStrength = meshMaterials.GetFloat ("_RimStrength");

			ShineRim();

			Invoke ("ReturnToOriginalRim", timer);
		}
		else
		{
			this.enabled = false;
		}
	}

	void ShineRim()
	{
		meshMaterials.SetFloat ("_RimPower", 0f);
		meshMaterials.SetFloat ("_RimStrength", rimStrength);
	}

	void ReturnToOriginalRim()
	{
		meshMaterials.SetFloat ("_RimPower", originalRimPower);
		meshMaterials.SetFloat ("_RimStrength", originalRimStrength);

		this.enabled = false;
	}
}
