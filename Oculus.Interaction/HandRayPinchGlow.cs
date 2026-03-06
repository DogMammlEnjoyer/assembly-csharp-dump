using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandRayPinchGlow : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
		}

		protected virtual void Start()
		{
			this._glowEnabled = false;
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

		private void UpdateVisualState(InteractorStateChangeArgs args)
		{
			this.UpdateVisual();
		}

		private void UpdateGlow(Vector3 glowPosition, float pinchStrength, float glowMaxLength)
		{
			if (this._materialEditor == null)
			{
				return;
			}
			MaterialPropertyBlock materialPropertyBlock = this._materialEditor.MaterialPropertyBlock;
			materialPropertyBlock.SetInt(this._generateGlowID, 1);
			materialPropertyBlock.SetColor(this._glowColorID, this._glowColor);
			materialPropertyBlock.SetFloat(this._glowParameterID, pinchStrength);
			materialPropertyBlock.SetFloat(this._glowMaxLengthID, glowMaxLength);
			materialPropertyBlock.SetInt(this._glowTypeID, (int)this._glowType);
			materialPropertyBlock.SetVector(this._glowPositionID, glowPosition);
		}

		private void UpdateVisual()
		{
			if (this._rayInteractor.State == InteractorState.Disabled)
			{
				if (this._glowEnabled)
				{
					if (this._materialEditor == null)
					{
						return;
					}
					this._materialEditor.MaterialPropertyBlock.SetInt(this._generateGlowID, 0);
					this._glowEnabled = false;
					return;
				}
			}
			else
			{
				this._glowEnabled = true;
				Pose pose;
				if (!this.Hand.GetJointPose(HandJointId.HandThumbTip, out pose))
				{
					return;
				}
				Pose pose2;
				if (!this.Hand.GetJointPose(HandJointId.HandIndexTip, out pose2))
				{
					return;
				}
				Pose pose3;
				if (!this.Hand.GetRootPose(out pose3))
				{
					return;
				}
				float fingerPinchStrength = this.Hand.GetFingerPinchStrength(HandFinger.Index);
				Vector3 vector = (pose.position + pose2.position) / 2f;
				float glowMaxLength = Vector3.Distance(pose3.position, vector) * 0.9f;
				this.UpdateGlow(vector, fingerPinchStrength, glowMaxLength);
			}
		}

		public void InjectAllHandRayPinchGlow(IHand hand, RayInteractor interactor, MaterialPropertyBlockEditor materialEditor, Color color, HandRayPinchGlow.GlowType glowType)
		{
			this.InjectHand(hand);
			this.InjectRayInteractor(interactor);
			this.InjectMaterialPropertyBlockEditor(materialEditor);
			this.InjectGlowColor(color);
			this.InjectGlowType(glowType);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectRayInteractor(RayInteractor interactor)
		{
			this._rayInteractor = interactor;
		}

		public void InjectMaterialPropertyBlockEditor(MaterialPropertyBlockEditor materialEditor)
		{
			this._materialEditor = materialEditor;
		}

		public void InjectGlowColor(Color color)
		{
			this._glowColor = color;
		}

		public void InjectGlowType(HandRayPinchGlow.GlowType glowType)
		{
			this._glowType = glowType;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		private RayInteractor _rayInteractor;

		[SerializeField]
		private MaterialPropertyBlockEditor _materialEditor;

		[SerializeField]
		private Color _glowColor;

		[SerializeField]
		private HandRayPinchGlow.GlowType _glowType = HandRayPinchGlow.GlowType.Outline;

		private IHand Hand;

		private readonly int _generateGlowID = Shader.PropertyToID("_GenerateGlow");

		private readonly int _glowPositionID = Shader.PropertyToID("_GlowPosition");

		private readonly int _glowColorID = Shader.PropertyToID("_GlowColor");

		private readonly int _glowTypeID = Shader.PropertyToID("_GlowType");

		private readonly int _glowParameterID = Shader.PropertyToID("_GlowParameter");

		private readonly int _glowMaxLengthID = Shader.PropertyToID("_GlowMaxLength");

		private bool _glowEnabled;

		protected bool _started;

		public enum GlowType
		{
			Fill = 17,
			Outline,
			Both = 16
		}
	}
}
