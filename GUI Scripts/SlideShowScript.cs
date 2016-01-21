using UnityEngine;
using System.Collections;

public class SlideShowScript : MonoBehaviour
{
	private int currentIndex = 0;

	public Transform[] images;
	public Transform[] extras;
	public Transform[] extras2;

	public bool isTimed = false;
	public float timer = 5f;

	void Start()
	{
		if(isTimed)
		{
			InvokeRepeating ("Next", 0f, timer);
		}
	}

	public void Next()
	{
		currentIndex ++;

		if(currentIndex >= images.Length)
		{
			currentIndex = 0;
		}
		
		if(currentIndex < 0)
		{
			currentIndex = images.Length - 1;
		}

		//Clear all 
		for(int i = 0; i < images.Length; i++)
		{
			if(images[i])
			{
				images[i].gameObject.SetActive (false);
			}

			if(extras[i])
			{
				extras[i].gameObject.SetActive (false);
			}

			if(extras2[i])
			{
				extras2[i].gameObject.SetActive (false);
			}
		}

		//Show relevant
		if(images[currentIndex])
		{
			images[currentIndex].gameObject.SetActive (true);
		}

		if(extras[currentIndex])
		{
			extras[currentIndex].gameObject.SetActive (true);
		}

		if(extras2[currentIndex])
		{
			extras2[currentIndex].gameObject.SetActive (true);
		}

	}

	public void Prev()
	{
		currentIndex --;
		
		if(currentIndex >= images.Length)
		{
			currentIndex = 0;
		}
		
		if(currentIndex < 0)
		{
			currentIndex = images.Length - 1;
		}
		
		//Clear all 
		for(int i = 0; i < images.Length; i++)
		{
			if(images[i])
			{
				images[i].gameObject.SetActive (false);
			}
			
			if(extras[i])
			{
				extras[i].gameObject.SetActive (false);
			}

			if(extras2[i])
			{
				extras2[i].gameObject.SetActive (false);
			}
		}
		
		//Show relevant
		if(images[currentIndex])
		{
			images[currentIndex].gameObject.SetActive (true);
		}
		
		if(extras[currentIndex])
		{
			extras[currentIndex].gameObject.SetActive (true);
		}

		if(extras2[currentIndex])
		{
			extras2[currentIndex].gameObject.SetActive (true);
		}
		
	}

}
