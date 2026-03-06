using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class SquishyToy : MonoBehaviour
	{
		private void Start()
		{
			if (this.rigidbody == null)
			{
				this.rigidbody = base.GetComponent<Rigidbody>();
			}
			if (this.interactable == null)
			{
				this.interactable = base.GetComponent<Interactable>();
			}
			if (this.renderer == null)
			{
				this.renderer = base.GetComponent<SkinnedMeshRenderer>();
			}
		}

		private void Update()
		{
			float num = 0f;
			float num2 = 0f;
			if (this.interactable.attachedToHand)
			{
				num = this.gripSqueeze.GetAxis(this.interactable.attachedToHand.handType);
				num2 = this.pinchSqueeze.GetAxis(this.interactable.attachedToHand.handType);
			}
			this.renderer.SetBlendShapeWeight(0, Mathf.Lerp(this.renderer.GetBlendShapeWeight(0), num * 100f, Time.deltaTime * 10f));
			if (this.renderer.sharedMesh.blendShapeCount > 1)
			{
				this.renderer.SetBlendShapeWeight(1, Mathf.Lerp(this.renderer.GetBlendShapeWeight(1), num2 * 100f, Time.deltaTime * 10f));
			}
			if (this.affectMaterial)
			{
				this.renderer.material.SetFloat("_Deform", Mathf.Pow(num * 1f, 0.5f));
				if (this.renderer.material.HasProperty("_PinchDeform"))
				{
					this.renderer.material.SetFloat("_PinchDeform", Mathf.Pow(num2 * 1f, 0.5f));
				}
			}
		}

		public Interactable interactable;

		public SkinnedMeshRenderer renderer;

		public bool affectMaterial = true;

		public SteamVR_Action_Single gripSqueeze = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze", false);

		public SteamVR_Action_Single pinchSqueeze = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze", false);

		private Rigidbody rigidbody;
	}
}
