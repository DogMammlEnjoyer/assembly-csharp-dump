using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class Grenade : MonoBehaviour
	{
		private void Start()
		{
			this.interactable = base.GetComponent<Interactable>();
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (this.interactable != null && this.interactable.attachedToHand != null)
			{
				return;
			}
			if (collision.impulse.magnitude > this.minMagnitudeToExplode)
			{
				for (int i = 0; i < this.explodeCount; i++)
				{
					Object.Instantiate<GameObject>(this.explodePartPrefab, base.transform.position, base.transform.rotation).GetComponentInChildren<MeshRenderer>().material.SetColor("_TintColor", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
				}
				Object.Destroy(base.gameObject);
			}
		}

		public GameObject explodePartPrefab;

		public int explodeCount = 10;

		public float minMagnitudeToExplode = 1f;

		private Interactable interactable;
	}
}
