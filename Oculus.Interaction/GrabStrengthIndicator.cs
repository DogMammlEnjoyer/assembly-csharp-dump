using System;
using Oculus.Interaction.Grab;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class GrabStrengthIndicator : MonoBehaviour
	{
		private IHandGrabInteractor HandGrab { get; set; }

		private IInteractor Interactor { get; set; }

		public float GlowLerpSpeed
		{
			get
			{
				return this._glowLerpSpeed;
			}
			set
			{
				this._glowLerpSpeed = value;
			}
		}

		public float GlowColorLerpSpeed
		{
			get
			{
				return this._glowColorLerpSpeed;
			}
			set
			{
				this._glowColorLerpSpeed = value;
			}
		}

		public Color FingerGlowColorWithInteractable
		{
			get
			{
				return this._fingerGlowColorWithInteractable;
			}
			set
			{
				this._fingerGlowColorWithInteractable = value;
			}
		}

		public Color FingerGlowColorWithNoInteractable
		{
			get
			{
				return this._fingerGlowColorWithNoInteractable;
			}
			set
			{
				this._fingerGlowColorWithNoInteractable = value;
			}
		}

		public Color FingerGlowColorHover
		{
			get
			{
				return this._fingerGlowColorHover;
			}
			set
			{
				this._fingerGlowColorHover = value;
			}
		}

		private void Awake()
		{
			this.HandGrab = (this._handGrabInteractor as IHandGrabInteractor);
			this.Interactor = (this._handGrabInteractor as IInteractor);
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
				this.Interactor.WhenPostprocessed += this.UpdateVisual;
				this._currentGlowColor = this._fingerGlowColorWithNoInteractable;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Interactor.WhenPostprocessed -= this.UpdateVisual;
			}
		}

		private void UpdateVisual()
		{
			bool flag = this.Interactor.State == InteractorState.Select;
			bool hasSelectedInteractable = this.Interactor.HasSelectedInteractable;
			bool hasCandidate = this.Interactor.HasCandidate;
			Color b = this._fingerGlowColorHover;
			if (flag)
			{
				b = (hasSelectedInteractable ? this._fingerGlowColorWithInteractable : this._fingerGlowColorWithNoInteractable);
			}
			this._currentGlowColor = Color.Lerp(this._currentGlowColor, b, Time.deltaTime * this._glowColorLerpSpeed);
			this._handMaterialPropertyBlockEditor.MaterialPropertyBlock.SetColor(this._fingerGlowColorPropertyId, this._currentGlowColor);
			for (int i = 0; i < 5; i++)
			{
				if ((flag && !hasSelectedInteractable) || (!flag && !hasCandidate))
				{
					this.UpdateGlowValue(i, 0f);
				}
				else
				{
					float num = 0f;
					HandFinger handFinger = (HandFinger)i;
					if ((this.HandGrab.SupportedGrabTypes & GrabTypeFlags.Pinch) != GrabTypeFlags.None && this.HandGrab.TargetInteractable != null && (this.HandGrab.TargetInteractable.SupportedGrabTypes & GrabTypeFlags.Pinch) != GrabTypeFlags.None && this.HandGrab.TargetInteractable.PinchGrabRules[handFinger] != FingerRequirement.Ignored)
					{
						num = Mathf.Max(num, this.HandGrab.HandGrabApi.GetFingerPinchStrength(handFinger));
					}
					if ((this.HandGrab.SupportedGrabTypes & GrabTypeFlags.Palm) != GrabTypeFlags.None && this.HandGrab.TargetInteractable != null && (this.HandGrab.TargetInteractable.SupportedGrabTypes & GrabTypeFlags.Palm) != GrabTypeFlags.None && this.HandGrab.TargetInteractable.PalmGrabRules[handFinger] != FingerRequirement.Ignored)
					{
						num = Mathf.Max(num, this.HandGrab.HandGrabApi.GetFingerPalmStrength(handFinger));
					}
					this.UpdateGlowValue(i, num);
				}
			}
			this._handMaterialPropertyBlockEditor.UpdateMaterialPropertyBlock();
		}

		private void UpdateGlowValue(int fingerIndex, float glowValue)
		{
			float value = Mathf.MoveTowards(this._handMaterialPropertyBlockEditor.MaterialPropertyBlock.GetFloat(this._handShaderGlowPropertyIds[fingerIndex]), glowValue, this._glowLerpSpeed * Time.deltaTime);
			this._handMaterialPropertyBlockEditor.MaterialPropertyBlock.SetFloat(this._handShaderGlowPropertyIds[fingerIndex], value);
		}

		public void InjectAllGrabStrengthIndicator(IHandGrabInteractor handGrabInteractor, MaterialPropertyBlockEditor handMaterialPropertyBlockEditor)
		{
			this.InjectHandGrab(handGrabInteractor);
			this.InjectHandMaterialPropertyBlockEditor(handMaterialPropertyBlockEditor);
		}

		public void InjectHandGrab(IHandGrabInteractor handGrab)
		{
			this._handGrabInteractor = (handGrab as Object);
			this.HandGrab = handGrab;
			this.Interactor = (handGrab as IInteractor);
		}

		public void InjectHandMaterialPropertyBlockEditor(MaterialPropertyBlockEditor handMaterialPropertyBlockEditor)
		{
			this._handMaterialPropertyBlockEditor = handMaterialPropertyBlockEditor;
		}

		[SerializeField]
		[Interface(typeof(IHandGrabInteractor), new Type[]
		{
			typeof(IInteractor)
		})]
		private Object _handGrabInteractor;

		[SerializeField]
		private MaterialPropertyBlockEditor _handMaterialPropertyBlockEditor;

		[SerializeField]
		private float _glowLerpSpeed = 2f;

		[SerializeField]
		private float _glowColorLerpSpeed = 2f;

		[SerializeField]
		private Color _fingerGlowColorWithInteractable;

		[SerializeField]
		private Color _fingerGlowColorWithNoInteractable;

		[SerializeField]
		private Color _fingerGlowColorHover;

		private readonly int[] _handShaderGlowPropertyIds = new int[]
		{
			Shader.PropertyToID("_ThumbGlowValue"),
			Shader.PropertyToID("_IndexGlowValue"),
			Shader.PropertyToID("_MiddleGlowValue"),
			Shader.PropertyToID("_RingGlowValue"),
			Shader.PropertyToID("_PinkyGlowValue")
		};

		private readonly int _fingerGlowColorPropertyId = Shader.PropertyToID("_FingerGlowColor");

		private Color _currentGlowColor;

		protected bool _started;
	}
}
