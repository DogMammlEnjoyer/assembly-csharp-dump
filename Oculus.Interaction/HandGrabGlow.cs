using System;
using Oculus.Interaction.Grab;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandGrabGlow : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this._glowFadeValue = 1f;
			this._state = HandGrabGlow.GlowState.None;
			this._grabState = HandGrabGlow.GrabState.None;
			this.HandGrabInteractor = (this._handGrabInteractor as IHandGrabInteractor);
			this.Interactor = (this._handGrabInteractor as IInteractor);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			HandFingerMaskGenerator.GenerateFingerMask(this._handRenderer, this._handVisual, this._materialEditor.MaterialPropertyBlock);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Interactor.WhenPostprocessed += this.UpdateVisual;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Interactor.WhenPostprocessed -= this.UpdateVisual;
			}
		}

		private void SetMaterialPropertyBlockValues()
		{
			MaterialPropertyBlock materialPropertyBlock = this._materialEditor.MaterialPropertyBlock;
			if (materialPropertyBlock == null)
			{
				return;
			}
			materialPropertyBlock.SetInt(this._generateGlowID, 1);
			materialPropertyBlock.SetColor(this._glowColorID, this._currentColor);
			if (this._glowType == HandGrabGlow.GlowType.Fill || this._glowType == HandGrabGlow.GlowType.Both)
			{
				float num = this._gradientLength;
				if (this._fadeOut)
				{
					num *= this._glowFadeValue;
				}
				materialPropertyBlock.SetFloat(this._glowParameterID, num);
			}
			else
			{
				materialPropertyBlock.SetFloat(this._glowParameterID, this._glowFadeValue);
			}
			for (int i = 0; i < this._fingersGlowIDs.Length; i++)
			{
				materialPropertyBlock.SetFloat(this._fingersGlowIDs[i], Mathf.Clamp01(this._glowStregth[i]));
			}
			materialPropertyBlock.SetInt(this._glowTypeID, (int)this._glowType);
		}

		private void UpdateFingerGlowStrength(int fingerIndex, float strength)
		{
			float num = Mathf.Lerp(this._glowStregth[fingerIndex], strength, this._glowStrengthChangeSpeed);
			this._glowStregth[fingerIndex] = num;
		}

		private bool FingerOptionalOrRequired(GrabbingRule rules, HandFinger finger)
		{
			return rules[finger] == FingerRequirement.Optional || rules[finger] == FingerRequirement.Required;
		}

		private void UpdateGlowStrength()
		{
			float b = 0f;
			for (int i = 1; i < 5; i++)
			{
				HandFinger finger = (HandFinger)i;
				bool flag = this.FingerOptionalOrRequired(this.HandGrabInteractor.TargetInteractable.PinchGrabRules, finger);
				object obj = (this.TargetSupportsPinch() && flag) ? this.HandGrabInteractor.HandGrabApi.GetFingerPinchStrength(finger) : 0f;
				bool flag2 = this.FingerOptionalOrRequired(this.HandGrabInteractor.TargetInteractable.PalmGrabRules, finger);
				float b2 = (this.TargetSupportsPalm() && flag2) ? this.HandGrabInteractor.HandGrabApi.GetFingerPalmStrength(finger) : 0f;
				object a = obj;
				float strength = Mathf.Max(a, b2);
				this.UpdateFingerGlowStrength(i, strength);
				b = Mathf.Max(a, b);
			}
			bool flag3 = this.FingerOptionalOrRequired(this.HandGrabInteractor.TargetInteractable.PalmGrabRules, HandFinger.Thumb);
			float a2 = (this.TargetSupportsPalm() && flag3) ? this.HandGrabInteractor.HandGrabApi.GetFingerPalmStrength(HandFinger.Thumb) : 0f;
			this.UpdateFingerGlowStrength(0, Mathf.Max(a2, b));
		}

		private void UpdateGlowState()
		{
			if (this.Interactor.State == InteractorState.Hover)
			{
				this._state = HandGrabGlow.GlowState.Hover;
				return;
			}
			if (this.Interactor.State == InteractorState.Select)
			{
				if (this._state == HandGrabGlow.GlowState.Hover || this._state == HandGrabGlow.GlowState.None)
				{
					this._accumulatedSelectedTime = 0f;
					this._state = HandGrabGlow.GlowState.Selected;
					return;
				}
				if (this._state == HandGrabGlow.GlowState.Selected)
				{
					this._accumulatedSelectedTime += Time.deltaTime;
					if (this._fadeOut && this._accumulatedSelectedTime >= this._glowFadeStartTime)
					{
						this._state = HandGrabGlow.GlowState.SelectedGlowOut;
						return;
					}
				}
			}
			else
			{
				this._state = HandGrabGlow.GlowState.None;
			}
		}

		private void UpdateGlowColorAndFade()
		{
			if (this._state == HandGrabGlow.GlowState.Hover)
			{
				this._glowFadeValue = 1f;
				this._currentColor = Color.Lerp(this._currentColor, this._fadeOut ? this._glowColorGrabing : this._glowColorHover, this._colorChangeSpeed);
				return;
			}
			if (this._state == HandGrabGlow.GlowState.Selected)
			{
				if (this._fadeOut)
				{
					this._glowFadeValue = Mathf.Lerp(this._glowFadeValue, 0.5f, 0.8f);
					this._currentColor = this._glowColorGrabing;
					return;
				}
				this._glowFadeValue = 1f;
				this._currentColor = Color.Lerp(this._currentColor, this._glowColorGrabing, this._colorChangeSpeed);
				return;
			}
			else
			{
				if (this._state == HandGrabGlow.GlowState.SelectedGlowOut)
				{
					this._glowFadeValue = Mathf.Lerp(this._glowFadeValue, 1.15f, 0.3f);
					this._currentColor = this._glowColorGrabing;
					return;
				}
				this._glowFadeValue = Mathf.Lerp(this._glowFadeValue, 0f, 0.15f);
				return;
			}
		}

		private bool TargetSupportsPinch()
		{
			return this.HandGrabInteractor.TargetInteractable != null && (this.HandGrabInteractor.SupportedGrabTypes & this.HandGrabInteractor.TargetInteractable.SupportedGrabTypes & GrabTypeFlags.Pinch) > GrabTypeFlags.None;
		}

		private bool TargetSupportsPalm()
		{
			return this.HandGrabInteractor.TargetInteractable != null && (this.HandGrabInteractor.SupportedGrabTypes & this.HandGrabInteractor.TargetInteractable.SupportedGrabTypes & GrabTypeFlags.Palm) > GrabTypeFlags.None;
		}

		private void UpdateGrabState()
		{
			if (this.HandGrabInteractor.TargetInteractable == null)
			{
				this._grabState = HandGrabGlow.GrabState.None;
				return;
			}
			GrabbingRule pinchGrabRules = this.HandGrabInteractor.TargetInteractable.PinchGrabRules;
			bool flag = this.HandGrabInteractor.HandGrabApi.IsHandPinchGrabbing(pinchGrabRules);
			if (this.TargetSupportsPinch() && flag && (this._grabState == HandGrabGlow.GrabState.None || this._grabState == HandGrabGlow.GrabState.Pinch))
			{
				this._grabState = HandGrabGlow.GrabState.Pinch;
				return;
			}
			GrabbingRule palmGrabRules = this.HandGrabInteractor.TargetInteractable.PalmGrabRules;
			bool flag2 = this.HandGrabInteractor.HandGrabApi.IsHandPalmGrabbing(palmGrabRules);
			if (this.TargetSupportsPalm() && flag2 && (this._grabState == HandGrabGlow.GrabState.None || this._grabState == HandGrabGlow.GrabState.Palm))
			{
				this._grabState = HandGrabGlow.GrabState.Palm;
				return;
			}
			this._grabState = HandGrabGlow.GrabState.None;
		}

		private void ClearGlow()
		{
			MaterialPropertyBlock materialPropertyBlock = this._materialEditor.MaterialPropertyBlock;
			foreach (int nameID in this._fingersGlowIDs)
			{
				materialPropertyBlock.SetFloat(nameID, 0f);
			}
			materialPropertyBlock.SetInt(this._generateGlowID, 0);
		}

		private void UpdateVisual()
		{
			HandGrabGlow.GlowState state = this._state;
			this.UpdateGrabState();
			this.UpdateGlowState();
			if (state != this._state && this._state == HandGrabGlow.GlowState.None)
			{
				this.ClearGlow();
				return;
			}
			if (this._state != HandGrabGlow.GlowState.None)
			{
				this.UpdateGlowStrength();
				this.UpdateGlowColorAndFade();
				this.SetMaterialPropertyBlockValues();
			}
		}

		public void InjectAllHandGrabGlow(IHandGrabInteractor handGrabInteractor, SkinnedMeshRenderer handRenderer, MaterialPropertyBlockEditor materialEditor, HandVisual handVisual, Color grabbingColor, Color hoverColor, float colorChangeSpeed, float fadeStartTime, float glowStrengthChangeSpeed, bool fadeOut, float gradientLength, HandGrabGlow.GlowType glowType)
		{
			this.InjectHandGrabInteractor(handGrabInteractor);
			this.InjectHandRenderer(handRenderer);
			this.InjectMaterialPropertyBlockEditor(materialEditor);
			this.InjectHandVisual(handVisual);
			this.InjectGlowColors(grabbingColor, hoverColor);
			this.InjectVisualChangeSpeed(colorChangeSpeed, fadeStartTime, glowStrengthChangeSpeed);
			this.InjectFadeOut(fadeOut);
			this.InjectGradientLength(gradientLength);
			this.InjectGlowType(glowType);
		}

		public void InjectHandGrabInteractor(IHandGrabInteractor handGrabInteractor)
		{
			this._handGrabInteractor = (handGrabInteractor as Object);
			this.Interactor = (handGrabInteractor as IInteractor);
			this.HandGrabInteractor = handGrabInteractor;
		}

		public void InjectHandRenderer(SkinnedMeshRenderer handRenderer)
		{
			this._handRenderer = handRenderer;
		}

		public void InjectMaterialPropertyBlockEditor(MaterialPropertyBlockEditor materialEditor)
		{
			this._materialEditor = materialEditor;
		}

		public void InjectHandVisual(HandVisual handVisual)
		{
			this._handVisual = handVisual;
		}

		public void InjectGlowColors(Color grabbingColor, Color hoverColor)
		{
			this._glowColorGrabing = grabbingColor;
			this._glowColorHover = hoverColor;
		}

		public void InjectVisualChangeSpeed(float colorChangeSpeed, float fadeStartTime, float glowStrengthChangeSpeed)
		{
			this._colorChangeSpeed = colorChangeSpeed;
			this._glowFadeStartTime = fadeStartTime;
			this._glowStrengthChangeSpeed = glowStrengthChangeSpeed;
		}

		public void InjectFadeOut(bool fadeOut)
		{
			this._fadeOut = fadeOut;
		}

		public void InjectGradientLength(float gradientLength)
		{
			this._gradientLength = Mathf.Clamp01(gradientLength);
		}

		public void InjectGlowType(HandGrabGlow.GlowType glowType)
		{
			this._glowType = glowType;
		}

		[SerializeField]
		[Interface(typeof(IHandGrabInteractor), new Type[]
		{
			typeof(IInteractor)
		})]
		private Object _handGrabInteractor;

		[SerializeField]
		private HandVisual _handVisual;

		[SerializeField]
		private SkinnedMeshRenderer _handRenderer;

		[SerializeField]
		private MaterialPropertyBlockEditor _materialEditor;

		[SerializeField]
		private Color _glowColorGrabing;

		[SerializeField]
		private Color _glowColorHover;

		[SerializeField]
		[Range(0f, 1f)]
		private float _colorChangeSpeed = 0.5f;

		[SerializeField]
		[Range(0f, 0.25f)]
		private float _glowFadeStartTime = 0.2f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _glowStrengthChangeSpeed = 0.5f;

		[SerializeField]
		private bool _fadeOut;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Recommended from 0.7 to 1.0")]
		private float _gradientLength = 0.85f;

		[SerializeField]
		private HandGrabGlow.GlowType _glowType = HandGrabGlow.GlowType.Outline;

		private HandGrabGlow.GlowState _state;

		private float _accumulatedSelectedTime;

		private HandGrabGlow.GrabState _grabState;

		private float _glowFadeValue;

		private Color _currentColor;

		private IHandGrabInteractor HandGrabInteractor;

		private IInteractor Interactor;

		private float[] _glowStregth = new float[5];

		private readonly int _generateGlowID = Shader.PropertyToID("_GenerateGlow");

		private readonly int _glowColorID = Shader.PropertyToID("_GlowColor");

		private readonly int _glowTypeID = Shader.PropertyToID("_GlowType");

		private readonly int _glowParameterID = Shader.PropertyToID("_GlowParameter");

		private readonly int[] _fingersGlowIDs = new int[]
		{
			Shader.PropertyToID("_ThumbGlowValue"),
			Shader.PropertyToID("_IndexGlowValue"),
			Shader.PropertyToID("_MiddleGlowValue"),
			Shader.PropertyToID("_RingGlowValue"),
			Shader.PropertyToID("_PinkyGlowValue")
		};

		protected bool _started;

		public enum GlowType
		{
			Fill = 27,
			Outline,
			Both
		}

		private enum GlowState
		{
			None,
			Hover,
			Selected,
			SelectedGlowOut
		}

		private enum GrabState
		{
			None,
			Pinch,
			Palm
		}
	}
}
