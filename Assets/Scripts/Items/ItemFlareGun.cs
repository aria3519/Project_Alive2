using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlareGun : Item
{
	private Rigidbody rigidbody;
	private Light flarelight;
	private AudioSource flaresound;
	private bool myCoroutine;
	private float smooth = 2.4f; // 2.4
	public float flareTimer = 9;
	public AudioClip flareBurningSound;
	public AudioClip flareShotSound;
	//public float forceResist = 1f;

	private void Start()
    {
		StartCoroutine("flareLightoff");
		GetComponent<AudioSource>().PlayOneShot(flareShotSound);
		GetComponent<AudioSource>().PlayOneShot(flareBurningSound);
		flarelight = GetComponent<Light>();
		flaresound = GetComponent<AudioSource>();
		rigidbody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if (myCoroutine == true)
		{
			flarelight.intensity = Random.Range(2f, 6.0f);

		}
		else
		{
			//rigidbody.AddForce(Vector3.up * forceResist);
			var value = Random.value;
			flarelight.intensity = Mathf.Lerp(flarelight.intensity, 0f, value);
			flarelight.range = Mathf.Lerp(flarelight.range, 0f, value);
			flaresound.volume = Mathf.Lerp(flaresound.volume, 0f, value);
		}

		var vel = rigidbody.velocity;
		vel.y -= 0.05f;
		rigidbody.velocity = vel;
	}

	IEnumerator flareLightoff()
	{
		myCoroutine = true;
		yield return new WaitForSeconds(flareTimer);
		myCoroutine = false;

		ItemManager.instance.InsertQueue(this, ItemKind.ItemFlare);
	}
}
