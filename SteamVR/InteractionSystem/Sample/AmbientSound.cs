using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class AmbientSound : MonoBehaviour
	{
		private void Start()
		{
			AudioListener.volume = 1f;
			this.s = base.GetComponent<AudioSource>();
			this.s.time = Random.Range(0f, this.s.clip.length);
			if (this.fadeintime > 0f)
			{
				this.t = 0f;
			}
			this.vol = this.s.volume;
			SteamVR_Fade.Start(Color.black, 0f, false);
			SteamVR_Fade.Start(Color.clear, 7f, false);
		}

		private void Update()
		{
			if (this.fadeintime > 0f && this.t < 1f)
			{
				this.t += Time.deltaTime / this.fadeintime;
				this.s.volume = this.t * this.vol;
			}
		}

		private AudioSource s;

		public float fadeintime;

		private float t;

		public bool fadeblack;

		private float vol;
	}
}
