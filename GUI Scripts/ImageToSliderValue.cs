using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageToSliderValue : MonoBehaviour 
{	
	private bool updateValue = false;
	public Image imageValue;
	public Slider sliderValue;
	public Transform sliderEffect;

	// Update is called once per frame
	void Update () 
	{
		if(updateValue)
		{
			if(imageValue.fillAmount < sliderValue.value)
			{
				sliderEffect.gameObject.SetActive (true);
			}
			else
			{
				sliderEffect.gameObject.SetActive (false);
			}

			if(sliderValue.value == imageValue.fillAmount)
			{
				updateValue = false;
				sliderEffect.gameObject.SetActive (false);
			}

			sliderValue.value = imageValue.fillAmount;
		}
		else
		{
			sliderValue.value = imageValue.fillAmount;
		}
	}

	public void StartUpdatingValue()
	{
		updateValue = true;
	}
}
