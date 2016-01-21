using UnityEngine;
using System.Collections;

public class ShooterBotOverworld : MonoBehaviour 
{
	private OverworldEnemy self;

	public float shootMaxTimer = 3f;
	private float shootTimer = 0f;

	private float shotDelay;
	public float shotMaxDelay;
	public float shotSpeed = 100f;

	public UIFade alertFader;

	public Transform shot;
	public Transform shotNode;

	private OverworldProjectile projectile;

	// Use this for initialization
	void Start () 
	{
		self = gameObject.GetComponent<OverworldEnemy>();

		shootTimer = shootMaxTimer;

		alertFader.animateFade = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(self.playerDetected)
		{
			shootTimer -= Time.deltaTime;
			alertFader.fadeSpeed += Time.deltaTime;

			if(shootTimer <= 0f)
			{
				Shoot ();
			}
		}
		else
		{
			shootTimer = shootMaxTimer;
			alertFader.fadeSpeed = 0f;
		}

		if(projectile != null)
		{
			if(projectile.hitPlayer)
			{
				//Start Battle here
				print ("START BATTLE HERE");
				self.ActivateBattle (GameObject.FindGameObjectWithTag ("Player"));
			}
		}
	}

	void Shoot()
	{
		if(shotDelay <= 0f)
		{
			Transform bullet = Instantiate (shot, shotNode.position, shotNode.rotation) as Transform;
			bullet.gameObject.GetComponent<Rigidbody>().velocity = shotNode.TransformDirection (new Vector3(0f, 0f, shotSpeed));
			projectile = bullet.gameObject.GetComponent<OverworldProjectile>();
			shotDelay = shotMaxDelay;
		}
		else
		{
			shotDelay -= Time.deltaTime;
		}
	}
}
