using System;
using System.Collections;
using Oculus.Interaction.Input;
using Oculus.Interaction.Locomotion;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.DistanceReticles
{
	public class TeleportReticleDrawer : InteractorReticle<ReticleDataTeleport>
	{
		private IAxis1D ProgressState { get; set; }

		private IActiveState HighlightState { get; set; }

		public Color AcceptColor
		{
			get
			{
				return this._acceptColor;
			}
			set
			{
				this._acceptColor = value;
			}
		}

		public Color RejectColor
		{
			get
			{
				return this._rejectColor;
			}
			set
			{
				this._rejectColor = value;
			}
		}

		public AnimationCurve AcceptAnimation
		{
			get
			{
				return this._acceptAnimation;
			}
			set
			{
				this._acceptAnimation = value;
			}
		}

		public AnimationCurve RejectAnimation
		{
			get
			{
				return this._rejectAnimation;
			}
			set
			{
				this._rejectAnimation = value;
			}
		}

		public float TransitionSpeed
		{
			get
			{
				return this._transitionSpeed;
			}
			set
			{
				this._transitionSpeed = value;
			}
		}

		protected override IInteractorView Interactor { get; set; }

		protected override Component InteractableComponent
		{
			get
			{
				return this._interactor.Interactable;
			}
		}

		protected virtual void Awake()
		{
			this.Interactor = this._interactor;
			this.ProgressState = (this._progressState as IAxis1D);
			this.HighlightState = (this._highlightState as IActiveState);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.EndStart(ref this._started);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this._started)
			{
				this._interactor.WhenStateChanged += this.HandleStateChanged;
				this.SetReticleProgress(0f);
				this._targetRenderer.enabled = false;
			}
		}

		protected override void OnDisable()
		{
			if (this._started)
			{
				this._interactor.WhenStateChanged -= this.HandleStateChanged;
			}
			base.OnDisable();
		}

		protected override void Align(ReticleDataTeleport data)
		{
			bool flag = (this.HighlightState != null) ? this.HighlightState.Active : this._selectionAnimation;
			data.Highlight(flag);
			if (data.HideReticle)
			{
				return;
			}
			Vector3 position = data.ProcessHitPoint(this._interactor.ArcEnd.Point);
			Quaternion rotation = Quaternion.LookRotation(this._interactor.ArcEnd.Normal);
			base.transform.SetPositionAndRotation(position, rotation);
			this._targetRenderer.enabled = true;
			float time = (this.ProgressState != null) ? this.ProgressState.Value() : this._animatedProgress;
			AnimationCurve animationCurve = this._acceptMode ? this._acceptAnimation : this._rejectAnimation;
			this.SetReticleProgress(animationCurve.Evaluate(time));
			this.SetReticleHighlight(flag);
		}

		protected override void Draw(ReticleDataTeleport data)
		{
			TeleportInteractable interactable = this._interactor.Interactable;
			this._acceptMode = interactable.AllowTeleport;
			this._selectionAnimation = false;
			this.SetReticleColor(this._acceptMode ? this._acceptColor : this._rejectColor);
		}

		protected override void Hide()
		{
			if (this._targetRenderer != null)
			{
				this._targetRenderer.enabled = false;
			}
			if (this._targetData != null)
			{
				this._targetData.Highlight(false);
			}
		}

		private void SetReticleColor(Color color)
		{
			this._targetRenderer.material.SetColor(TeleportReticleDrawer._colorKey, color);
			this._targetRenderer.material.SetColor(TeleportReticleDrawer._highlightColorKey, color);
		}

		private void SetReticleProgress(float progress)
		{
			if (this._selectionAnimation)
			{
				this._currentProgress = progress;
			}
			else
			{
				this._currentProgress = Mathf.MoveTowards(this._currentProgress, progress, this._transitionSpeed * Time.deltaTime);
			}
			this._targetRenderer.material.SetFloat(TeleportReticleDrawer._progressKey, this._currentProgress);
		}

		private void SetReticleHighlight(bool highlight)
		{
			this._targetRenderer.material.SetFloat(TeleportReticleDrawer._highlightKey, highlight ? 1f : 0f);
		}

		private void HandleStateChanged(InteractorStateChangeArgs obj)
		{
			if (this.ProgressState == null && obj.NewState == InteractorState.Select)
			{
				base.StopAllCoroutines();
				base.StartCoroutine(this.SelectionAnimation());
			}
		}

		private IEnumerator SelectionAnimation()
		{
			float targetProgress = 1f;
			this._animatedProgress = 0f;
			this._selectionAnimation = true;
			while (!Mathf.Approximately(targetProgress, this._animatedProgress))
			{
				this._animatedProgress = Mathf.MoveTowards(this._animatedProgress, targetProgress, this._transitionSpeed * Time.deltaTime);
				yield return null;
			}
			this._selectionAnimation = false;
			yield break;
		}

		public void InjectAllTeleportReticleDrawer(TeleportInteractor interactor, Renderer targetRenderer)
		{
			this.InjectInteractor(interactor);
			this.InjectTargetRenderer(targetRenderer);
		}

		public void InjectInteractor(TeleportInteractor interactor)
		{
			this._interactor = interactor;
		}

		public void InjectTargetRenderer(Renderer targetRenderer)
		{
			this._targetRenderer = targetRenderer;
		}

		[Obsolete("Use InjectTargetRenderer instead")]
		public void InjectOptionalValidTargetRenderer(Renderer validTargetRenderer)
		{
			this._targetRenderer = validTargetRenderer;
		}

		[Obsolete("Not in use")]
		public void InjectOptionalInalidTargetRenderer(Renderer invalidTargetRenderer)
		{
			this._invalidTargetRenderer = invalidTargetRenderer;
		}

		public void InjectOptionalProgress(IAxis1D progressState)
		{
			this._progressState = (progressState as Object);
			this.ProgressState = progressState;
		}

		public void InjectOptionalHighlightState(IActiveState highlightState)
		{
			this._highlightState = (highlightState as Object);
			this.HighlightState = highlightState;
		}

		[SerializeField]
		private TeleportInteractor _interactor;

		[SerializeField]
		[FormerlySerializedAs("_validTargetRenderer")]
		private Renderer _targetRenderer;

		[SerializeField]
		[Optional(OptionalAttribute.Flag.Obsolete)]
		[Obsolete("This renderer is not in use")]
		private Renderer _invalidTargetRenderer;

		[SerializeField]
		[Optional]
		[Interface(typeof(IAxis1D), new Type[]
		{

		})]
		[FormerlySerializedAs("_progress")]
		private Object _progressState;

		[SerializeField]
		[Optional]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _highlightState;

		[SerializeField]
		private Color _acceptColor = Color.white;

		[SerializeField]
		private Color _rejectColor = Color.red;

		[SerializeField]
		private AnimationCurve _acceptAnimation = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		[SerializeField]
		private AnimationCurve _rejectAnimation = AnimationCurve.Linear(0f, 0f, 1f, 0.3f);

		[SerializeField]
		private float _transitionSpeed = 8f;

		private static readonly int _progressKey = Shader.PropertyToID("_Progress");

		private static readonly int _highlightKey = Shader.PropertyToID("_Highlight");

		private static readonly int _colorKey = Shader.PropertyToID("_Color");

		private static readonly int _highlightColorKey = Shader.PropertyToID("_HighlightColor");

		private bool _selectionAnimation;

		private float _animatedProgress;

		private float _currentProgress;

		private bool _acceptMode = true;
	}
}
