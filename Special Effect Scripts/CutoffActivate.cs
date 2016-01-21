using UnityEngine;
using System.Collections;

public class CutoffActivate : MonoBehaviour 
{
	//using _Cutoff
	private Material objectMaterial;

	private float currentCutoff = 1f;
	public float cutoffFade = 2f;

	// Use this for initialization
	void Start () 
	{
		objectMaterial = gameObject.GetComponent<Renderer>().material;
		objectMaterial.SetFloat ("_Cutoff", currentCutoff);
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentCutoff -= Time.deltaTime * cutoffFade;
		objectMaterial.SetFloat ("_Cutoff", currentCutoff);

		if(currentCutoff <= 0f)
		{
			this.enabled = false;
		}
	}
}
