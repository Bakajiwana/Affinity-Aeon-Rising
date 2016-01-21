using UnityEngine;
using System.Collections;

public class AnimationTrigger : MonoBehaviour 
{
	public Animator anim;

	// Use this for initialization
	void Start () 
	{
		anim.SetTrigger ("AnimToggle");
	}
}
