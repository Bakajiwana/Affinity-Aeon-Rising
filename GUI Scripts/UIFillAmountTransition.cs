using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIFillAmountTransition : MonoBehaviour 
{
	private Image transitionAmount;
	public float transition = 0.5f;

	private bool transitionStart = false;

	void Start()
	{
		transitionAmount = gameObject.GetComponent<Image>();
		transitionAmount.fillAmount = 0f;
	}

	void Update()
	{
		if(transitionStart)
		{
			transitionAmount.fillAmount += Time.deltaTime * transition;

			if(transitionAmount.fillAmount >= 1f)
			{
				transitionStart = false;
			}
		}
	}

	public void Transition()
	{
		transitionStart = true;
	}

	public void CompleteTransition()
	{
		transitionAmount.fillAmount = 1f;
	}
}
