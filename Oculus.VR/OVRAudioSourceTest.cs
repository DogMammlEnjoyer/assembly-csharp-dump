using System;
using UnityEngine;

public class OVRAudioSourceTest : MonoBehaviour
{
	private void Start()
	{
		Material material = Object.Instantiate<Material>(base.GetComponent<Renderer>().material);
		material.color = Color.green;
		base.GetComponent<Renderer>().material = material;
		this.nextActionTime = Time.time + this.period;
	}

	private void Update()
	{
		if (Time.time > this.nextActionTime)
		{
			this.nextActionTime = Time.time + this.period;
			Material material = base.GetComponent<Renderer>().material;
			if (material.color == Color.green)
			{
				material.color = Color.red;
			}
			else
			{
				material.color = Color.green;
			}
			AudioSource component = base.GetComponent<AudioSource>();
			if (component == null)
			{
				Debug.LogError("Unable to find AudioSource");
				return;
			}
			component.Play();
		}
	}

	public float period = 2f;

	private float nextActionTime;
}
