using System;
using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class ProceduralHats : MonoBehaviour
	{
		private void Start()
		{
			this.SwitchToHat(0);
		}

		private void OnEnable()
		{
			base.StartCoroutine(this.HatSwitcher());
		}

		private IEnumerator HatSwitcher()
		{
			for (;;)
			{
				yield return new WaitForSeconds(this.hatSwitchTime);
				Transform cam = Camera.main.transform;
				while (Vector3.Angle(cam.forward, base.transform.position - cam.position) < 90f)
				{
					yield return new WaitForSeconds(0.1f);
				}
				this.ChooseHat();
				cam = null;
			}
			yield break;
		}

		private void ChooseHat()
		{
			this.SwitchToHat(Random.Range(0, this.hats.Length));
		}

		private void SwitchToHat(int hat)
		{
			for (int i = 0; i < this.hats.Length; i++)
			{
				this.hats[i].SetActive(hat == i);
			}
		}

		public GameObject[] hats;

		public float hatSwitchTime;
	}
}
