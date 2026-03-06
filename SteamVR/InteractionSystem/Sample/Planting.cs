using System;
using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class Planting : MonoBehaviour
	{
		private void OnEnable()
		{
			if (this.hand == null)
			{
				this.hand = base.GetComponent<Hand>();
			}
			if (this.plantAction == null)
			{
				Debug.LogError("<b>[SteamVR Interaction]</b> No plant action assigned", this);
				return;
			}
			this.plantAction.AddOnChangeListener(new SteamVR_Action_Boolean.ChangeHandler(this.OnPlantActionChange), this.hand.handType);
		}

		private void OnDisable()
		{
			if (this.plantAction != null)
			{
				this.plantAction.RemoveOnChangeListener(new SteamVR_Action_Boolean.ChangeHandler(this.OnPlantActionChange), this.hand.handType);
			}
		}

		private void OnPlantActionChange(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue)
		{
			if (newValue)
			{
				this.Plant();
			}
		}

		public void Plant()
		{
			base.StartCoroutine(this.DoPlant());
		}

		private IEnumerator DoPlant()
		{
			RaycastHit raycastHit;
			Vector3 position;
			if (Physics.Raycast(this.hand.transform.position, Vector3.down, out raycastHit))
			{
				position = raycastHit.point + Vector3.up * 0.05f;
			}
			else
			{
				position = this.hand.transform.position;
				position.y = Player.instance.transform.position.y;
			}
			GameObject planting = Object.Instantiate<GameObject>(this.prefabToPlant);
			planting.transform.position = position;
			planting.transform.rotation = Quaternion.Euler(0f, Random.value * 360f, 0f);
			planting.GetComponentInChildren<MeshRenderer>().material.SetColor("_TintColor", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
			Rigidbody rigidbody = planting.GetComponent<Rigidbody>();
			if (rigidbody != null)
			{
				rigidbody.isKinematic = true;
			}
			Vector3 initialScale = Vector3.one * 0.01f;
			Vector3 targetScale = Vector3.one * (1f + Random.value * 0.25f);
			float startTime = Time.time;
			float overTime = 0.5f;
			float endTime = startTime + overTime;
			while (Time.time < endTime)
			{
				planting.transform.localScale = Vector3.Slerp(initialScale, targetScale, (Time.time - startTime) / overTime);
				yield return null;
			}
			if (rigidbody != null)
			{
				rigidbody.isKinematic = false;
			}
			yield break;
		}

		public SteamVR_Action_Boolean plantAction;

		public Hand hand;

		public GameObject prefabToPlant;
	}
}
