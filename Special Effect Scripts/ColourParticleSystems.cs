using UnityEngine;
using System.Collections;

public class ColourParticleSystems : MonoBehaviour 
{
	public Color particleColor = Color.white;

	// Use this for initialization
	void Awake () 
	{
		ParticleSystem[] particles = gameObject.GetComponentsInChildren<ParticleSystem>();

		foreach(ParticleSystem particle in particles)
		{
			particle.startColor = particleColor;
		}

		this.enabled = false;
	}
}
