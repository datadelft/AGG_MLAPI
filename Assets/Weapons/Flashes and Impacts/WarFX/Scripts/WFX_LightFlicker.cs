using UnityEngine;
using System.Collections;

/**
 *	Rapidly sets a light on/off.
 *	
 *	(c) 2015, Jean Moreno
**/

[RequireComponent(typeof(Light))]

public class WFX_LightFlicker : MonoBehaviour
{
	public float time = 0.05f;
	private float timer;
	public float runTimer;
	public int itterarions = 2;

	void Start ()
	{
		timer = time;
		runTimer = itterarions * time;
		StartCoroutine("Flicker");
	}
	
	IEnumerator Flicker()
	{
		if (runTimer > 0)
		{
			runTimer -= Time.deltaTime;
			GetComponent<Light>().enabled = !GetComponent<Light>().enabled;

			do
			{
				timer -= Time.deltaTime;
				yield return null;
			}
			while (timer > 0);
			timer = time;
		}
		else
		{
			runTimer = 0;
		}
	}
}
