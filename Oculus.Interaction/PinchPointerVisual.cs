using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class PinchPointerVisual : MonoBehaviour
	{
		public Vector3 LocalOffset
		{
			get
			{
				return this._localOffset;
			}
			set
			{
				this._localOffset = value;
			}
		}

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

		public Color Tint
		{
			get
			{
				return this._tint;
			}
			set
			{
				this._tint = value;
			}
		}

		protected virtual void Awake()
		{
			this.Interactor = (this._interactor as IInteractor);
			this.Progress = (this._progress as IAxis1D);
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
				this.Interactor.WhenStateChanged += this.HandleStateChanged;
				this.Interactor.WhenPreprocessed += this.HandlePostprocessed;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Interactor.WhenStateChanged -= this.HandleStateChanged;
				this.Interactor.WhenPreprocessed -= this.HandlePostprocessed;
			}
		}

		public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			position += rotation * this._localOffset;
			base.transform.SetPositionAndRotation(position, rotation);
		}

		private void HandleStateChanged(InteractorStateChangeArgs stateArgs)
		{
			if (stateArgs.NewState == InteractorState.Disabled)
			{
				this._skinnedMeshRenderer.enabled = false;
				return;
			}
			this._skinnedMeshRenderer.enabled = true;
		}

		private void HandlePostprocessed()
		{
			if (this.Progress == null)
			{
				return;
			}
			float num = this._remapCurve.Evaluate(this.Progress.Value());
			this._skinnedMeshRenderer.SetBlendShapeWeight(0, num * 100f);
			this._skinnedMeshRenderer.SetBlendShapeWeight(1, num * 100f);
			this.UpdateColor(this.Interactor.State == InteractorState.Select, num);
		}

		private void UpdateColor(bool highlight, float mappedPinchStrength)
		{
			Color tint = this.Tint;
			tint.a *= (highlight ? 1f : Mathf.Lerp(this._alphaRange.x, this._alphaRange.y, mappedPinchStrength));
			this._skinnedMeshRenderer.material.color = tint;
		}

		public void InjectAllPinchPointerVisual(IInteractor interactor, SkinnedMeshRenderer skinnedMeshRenderer)
		{
			this.InjectInteractor(interactor);
			this.InjectSkinnedMeshRenderer(skinnedMeshRenderer);
		}

		public void InjectInteractor(IInteractor interactor)
		{
			this.Interactor = interactor;
			this._interactor = (interactor as Object);
		}

		public void InjectSkinnedMeshRenderer(SkinnedMeshRenderer skinnedMeshRenderer)
		{
			this._skinnedMeshRenderer = skinnedMeshRenderer;
		}

		[SerializeField]
		[Interface(typeof(IInteractor), new Type[]
		{

		})]
		private Object _interactor;

		private IInteractor Interactor;

		[SerializeField]
		private SkinnedMeshRenderer _skinnedMeshRenderer;

		[SerializeField]
		private Vector3 _localOffset = Vector3.zero;

		[SerializeField]
		private AnimationCurve _remapCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private Vector2 _alphaRange = new Vector2(0.1f, 0.4f);

		[SerializeField]
		private Color _tint = Color.white;

		[SerializeField]
		[Interface(typeof(IAxis1D), new Type[]
		{

		})]
		[Optional]
		private Object _progress;

		private IAxis1D Progress;

		protected bool _started;
	}
}
