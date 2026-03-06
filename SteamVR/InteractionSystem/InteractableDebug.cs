using System;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class InteractableDebug : MonoBehaviour
	{
		private bool isThrowable
		{
			get
			{
				return this.throwable != null;
			}
		}

		private void Awake()
		{
			this.selfRenderers = base.GetComponentsInChildren<Renderer>();
			this.throwable = base.GetComponent<Throwable>();
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.colliders = base.GetComponentsInChildren<Collider>();
		}

		private void OnAttachedToHand(Hand hand)
		{
			this.attachedToHand = hand;
			this.CreateMarker(Color.green, 10f);
		}

		protected virtual void HandAttachedUpdate(Hand hand)
		{
			Color color;
			switch (hand.currentAttachedObjectInfo.Value.grabbedWithType)
			{
			case GrabTypes.Trigger:
				color = Color.yellow;
				goto IL_56;
			case GrabTypes.Pinch:
				color = Color.green;
				goto IL_56;
			case GrabTypes.Grip:
				color = Color.blue;
				goto IL_56;
			case GrabTypes.Scripted:
				color = Color.red;
				goto IL_56;
			}
			color = Color.white;
			IL_56:
			if (color != this.lastColor)
			{
				this.ColorSelf(color);
			}
			this.lastColor = color;
		}

		private void OnDetachedFromHand(Hand hand)
		{
			if (this.isThrowable)
			{
				Vector3 vector;
				Vector3 vector2;
				this.throwable.GetReleaseVelocities(hand, out vector, out vector2);
				this.CreateMarker(Color.cyan, vector.normalized, 10f);
			}
			this.CreateMarker(Color.red, 10f);
			this.attachedToHand = null;
			if (!this.isSimulation && this.simulateReleasesForXSecondsAroundRelease != 0f)
			{
				float num = -this.simulateReleasesForXSecondsAroundRelease;
				float num2 = this.simulateReleasesForXSecondsAroundRelease;
				List<InteractableDebug> list = new List<InteractableDebug>();
				list.Add(this);
				for (float num3 = num; num3 <= num2; num3 += this.simulateReleasesEveryXSeconds)
				{
					float t = Mathf.InverseLerp(num, num2, num3);
					InteractableDebug item = this.CreateSimulation(hand, num3, Color.Lerp(Color.red, Color.green, t));
					list.Add(item);
				}
				for (int i = 0; i < list.Count; i++)
				{
					for (int j = 0; j < list.Count; j++)
					{
						list[i].IgnoreObject(list[j]);
					}
				}
			}
		}

		public Collider[] GetColliders()
		{
			return this.colliders;
		}

		public void IgnoreObject(InteractableDebug otherInteractable)
		{
			Collider[] array = otherInteractable.GetColliders();
			for (int i = 0; i < this.colliders.Length; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					Physics.IgnoreCollision(this.colliders[i], array[j]);
				}
			}
		}

		public void SetIsSimulation()
		{
			this.isSimulation = true;
		}

		private InteractableDebug CreateSimulation(Hand fromHand, float timeOffset, Color copyColor)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(base.gameObject);
			InteractableDebug component = gameObject.GetComponent<InteractableDebug>();
			component.SetIsSimulation();
			component.ColorSelf(copyColor);
			gameObject.name = string.Format("{0} [offset: {1:0.000}]", gameObject.name, timeOffset);
			Vector3 vector = fromHand.GetTrackedObjectVelocity(timeOffset);
			vector *= this.throwable.scaleReleaseVelocity;
			component.rigidbody.linearVelocity = vector;
			return component;
		}

		private void CreateMarker(Color markerColor, float destroyAfter = 10f)
		{
			this.CreateMarker(markerColor, this.attachedToHand.GetTrackedObjectVelocity(0f).normalized, destroyAfter);
		}

		private void CreateMarker(Color markerColor, Vector3 forward, float destroyAfter = 10f)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Object.DestroyImmediate(gameObject.GetComponent<Collider>());
			gameObject.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject);
			gameObject2.transform.localScale = new Vector3(0.01f, 0.01f, 0.25f);
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 0f, gameObject2.transform.localScale.z / 2f);
			gameObject.transform.position = this.attachedToHand.transform.position;
			gameObject.transform.forward = forward;
			this.ColorThing(markerColor, gameObject.GetComponentsInChildren<Renderer>());
			if (destroyAfter > 0f)
			{
				Object.Destroy(gameObject, destroyAfter);
			}
		}

		private void ColorSelf(Color newColor)
		{
			this.ColorThing(newColor, this.selfRenderers);
		}

		private void ColorThing(Color newColor, Renderer[] renderers)
		{
			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].material.color = newColor;
			}
		}

		[NonSerialized]
		public Hand attachedToHand;

		public float simulateReleasesForXSecondsAroundRelease;

		public float simulateReleasesEveryXSeconds = 0.005f;

		public bool setPositionsForSimulations;

		private Renderer[] selfRenderers;

		private Collider[] colliders;

		private Color lastColor;

		private Throwable throwable;

		private const bool onlyColorOnChange = true;

		public Rigidbody rigidbody;

		private bool isSimulation;
	}
}
