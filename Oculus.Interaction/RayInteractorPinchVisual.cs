using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class RayInteractorPinchVisual : MonoBehaviour
	{
		public AnimationCurve RemapCurve
		{
			get
			{
				return this._remapCurve;
			}
			set
			{
				this._remapCurve = value;
			}
		}

		public Vector2 AlphaRange
		{
			get
			{
				return this._alphaRange;
			}
			set
			{
				this._alphaRange = value;
			}
		}

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._rayInteractor.WhenPostprocessed += this.UpdateVisual;
				this._rayInteractor.WhenStateChanged += this.UpdateVisualState;
				this.UpdateVisual();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._rayInteractor.WhenPostprocessed -= this.UpdateVisual;
				this._rayInteractor.WhenStateChanged -= this.UpdateVisualState;
			}
		}

		private void UpdateVisual()
		{
			if (!this.Hand.IsTrackedDataValid || this._rayInteractor.State == InteractorState.Disabled)
			{
				if (this._skinnedMeshRenderer.enabled)
				{
					this._skinnedMeshRenderer.enabled = false;
				}
				return;
			}
			if (!this._skinnedMeshRenderer.enabled)
			{
				this._skinnedMeshRenderer.enabled = true;
			}
			Pose pose;
			if (!this.Hand.GetJointPose(HandJointId.HandIndex3, out pose))
			{
				return;
			}
			Pose pose2;
			if (!this.Hand.GetJointPose(HandJointId.HandThumb3, out pose2))
			{
				return;
			}
			bool flag = this._rayInteractor.State == InteractorState.Select;
			Vector3 position = Vector3.Lerp(pose2.position, pose.position, 0.5f);
			Transform transform = base.transform;
			Vector3 normalized = (this._rayInteractor.End - transform.position).normalized;
			transform.position = position;
			transform.rotation = Quaternion.LookRotation(normalized, Vector3.up);
			transform.localScale = Vector3.one * this.Hand.Scale;
			float num = this._remapCurve.Evaluate(this.Hand.GetFingerPinchStrength(HandFinger.Index));
			this._skinnedMeshRenderer.material.color = (flag ? Color.white : new Color(1f, 1f, 1f, Mathf.Lerp(this._alphaRange.x, this._alphaRange.y, num)));
			this._skinnedMeshRenderer.SetBlendShapeWeight(0, num * 100f);
			this._skinnedMeshRenderer.SetBlendShapeWeight(1, num * 100f);
		}

		private void UpdateVisualState(InteractorStateChangeArgs args)
		{
			this.UpdateVisual();
		}

		public void InjectAllRayInteractorPinchVisual(IHand hand, RayInteractor rayInteractor, SkinnedMeshRenderer skinnedMeshRenderer)
		{
			this.InjectHand(hand);
			this.InjectRayInteractor(rayInteractor);
			this.InjectSkinnedMeshRenderer(skinnedMeshRenderer);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectRayInteractor(RayInteractor rayInteractor)
		{
			this._rayInteractor = rayInteractor;
		}

		public void InjectSkinnedMeshRenderer(SkinnedMeshRenderer skinnedMeshRenderer)
		{
			this._skinnedMeshRenderer = skinnedMeshRenderer;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		private IHand Hand;

		[SerializeField]
		private RayInteractor _rayInteractor;

		[SerializeField]
		private SkinnedMeshRenderer _skinnedMeshRenderer;

		[SerializeField]
		private AnimationCurve _remapCurve;

		[SerializeField]
		private Vector2 _alphaRange = new Vector2(0.1f, 0.4f);

		protected bool _started;
	}
}
