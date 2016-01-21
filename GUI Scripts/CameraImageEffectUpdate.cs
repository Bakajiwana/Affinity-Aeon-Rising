﻿using UnityEngine;
using System.Collections;
using System.Reflection;

[RequireComponent(typeof(SunShafts))] 
[RequireComponent(typeof(Bloom))]
[RequireComponent(typeof(AntialiasingAsPostEffect))]
[RequireComponent(typeof(SSAOEffect))]
[RequireComponent(typeof(CameraMotionBlur))]
[RequireComponent(typeof(ContrastEnhance))]
[RequireComponent(typeof(GlowEffect))]

public class CameraImageEffectUpdate : MonoBehaviour 
{
	public static bool updateEffects = false;

	// Use this for initialization
	void Start () 
	{
		if(!GetComponent<GlowEffect>())
		{
			GlowEffect add = gameObject.AddComponent<GlowEffect>();
			add.enabled = false;
		}
		/* Check Component names
		Component[] components = gameObject.GetComponents<Component>();
		foreach (Component c in components) 
		{
			Debug.Log(c.GetType());
		}
		*/

		//If there's a tonemapper turn it off
		if(GetComponent<Tonemapping>())
		{
			GetComponent<Tonemapping>().enabled = false;	//Turn it off
		}

		UpdateCameraEffects ();
	}

	void Update () 
	{
		if(updateEffects)
		{
			UpdateCameraEffects ();
			//print ("Update Camera Effects");
			updateEffects = false;
		}

//		if(!Pause.isPaused && Input.GetButtonDown ("Cancel"))
//		{
//			UpdateCameraEffects ();
//			print ("Update Camera Effects");
//		}
	}

	public void UpdateCameraEffects()
	{
		//Update Post Effect Anti Aliasing
		if(PlayerPrefs.GetInt ("AA") == 0)
		{
			GetComponent<AntialiasingAsPostEffect>().enabled = false;
		}
		else
		{
			GetComponent<AntialiasingAsPostEffect>().enabled = true;
		}

		//Ambient Occlusion Update
		Component ssao = gameObject.GetComponent ("SSAOEffect");
		FieldInfo ssaoField = ssao.GetType ().GetField ("m_SampleCount");
		int currentSSAO = PlayerPrefs.GetInt ("AmbientOcclusion");
		switch(currentSSAO)
		{
		case 0:
			GetComponent<SSAOEffect>().enabled = false;
			break;
		case 1:
			ssaoField.SetValue (ssao, 0);
			GetComponent<SSAOEffect>().enabled = true;
			break;
		case 2:
			ssaoField.SetValue (ssao, 1);
			GetComponent<SSAOEffect>().enabled = true;
			break;
		case 3:
			ssaoField.SetValue (ssao, 2);
			GetComponent<SSAOEffect>().enabled = true;
			break;
		}

		//Motion Blur
		Component motionBlur = gameObject.GetComponent ("CameraMotionBlur");
		FieldInfo motionBlurField = motionBlur.GetType ().GetField ("velocityScale");
		float currentMotionBlur = PlayerPrefs.GetFloat ("MotionBlur");
		motionBlurField.SetValue (motionBlur, currentMotionBlur);
		if(currentMotionBlur == 0f)
		{
			GetComponent<CameraMotionBlur>().enabled = false;	//Turn it off
		}
		else
		{
			GetComponent<CameraMotionBlur>().enabled = true;
		}

		//Bloom
		Component bloom = gameObject.GetComponent ("Bloom");
		FieldInfo bloomField = bloom.GetType ().GetField ("bloomIntensity");
		float currentBloom = PlayerPrefs.GetFloat ("Bloom") * 20f;
		bloomField.SetValue (bloom, currentBloom);
		if(currentBloom == 0f)
		{
			GetComponent<Bloom>().enabled = false;	//Turn it off
		}
		else
		{
			GetComponent<Bloom>().enabled = true;
		}

		//Sun Shafts
		Component sunShaft = gameObject.GetComponent ("SunShafts");
		FieldInfo sunShaftField = sunShaft.GetType ().GetField ("sunShaftIntensity");
		float currentSunShaft = PlayerPrefs.GetFloat ("SunShaft") * 2f;
		sunShaftField.SetValue (sunShaft, currentSunShaft);
		if(currentSunShaft == 0f)
		{
			GetComponent<SunShafts>().enabled = false;	//Turn it off
		}
		else
		{
			GetComponent<SunShafts>().enabled = true;
		}

		//Brightness		
		float currentBrightness = PlayerPrefs.GetFloat ("Brightness");
		RenderSettings.ambientIntensity = currentBrightness;

		//Contrast
		float currentContrast = PlayerPrefs.GetFloat ("Contrast");
		Component contrast = gameObject.GetComponent ("ContrastEnhance");
		if(contrast)
		{
			FieldInfo contrastField = contrast.GetType ().GetField ("intensity");
			contrastField.SetValue (contrast, currentContrast);
		}


		if(currentContrast == 0f)
		{
			GetComponent<ContrastEnhance>().enabled = false;	//Turn it off
		}
		else
		{
			GetComponent<ContrastEnhance>().enabled = true;
		}

		//print (currentBrightness);
		//print (currentContrast);


//		Component brightness = gameObject.GetComponent ("Tonemapping");
//		FieldInfo brightnessField = brightness.GetType ().GetField ("middleGrey");
//
//		Component contrast = gameObject.GetComponent ("Tonemapping");
//		FieldInfo contrastField = contrast.GetType ().GetField ("white");
//
//		brightnessField.SetValue (brightness, currentBrightness);
//		contrastField.SetValue (contrast, currentContrast);
//
//
//		if(currentBrightness == 0f && currentContrast == 0f)
//		{
//			GetComponent<Tonemapping>().enabled = false;	//Turn it off
//		}
//		else
//		{
//			GetComponent<Tonemapping>().enabled = true;
//		}

	}

	void OnEnable()
	{
		UpdateCameraEffects ();
	}
}
