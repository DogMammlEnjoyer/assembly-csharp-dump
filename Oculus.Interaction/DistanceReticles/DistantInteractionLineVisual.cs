using System;
using UnityEngine;

namespace Oculus.Interaction.DistanceReticles
{
	public abstract class DistantInteractionLineVisual : MonoBehaviour
	{
		public IDistanceInteractor DistanceInteractor { get; protected set; }

		public float VisualOffset
		{
			get
			{
				return this._visualOffset;
			}
			set
			{
				this._visualOffset = value;
			}
		}

		protected int NumLinePoints
		{
			get
			{
				return this._numLinePoints;
			}
		}

		protected float TargetlessLength
		{
			get
			{
				return this._targetlessLength;
			}
		}

		private void Awake()
		{
			this.DistanceInteractor = (this._distanceInteractor as IDistanceInteractor);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._linePoints = new Vector3[this.NumLinePoints];
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.DistanceInteractor.WhenStateChanged += this.HandleStateChanged;
				this.DistanceInteractor.WhenPostprocessed += this.HandlePostProcessed;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.DistanceInteractor.WhenStateChanged -= this.HandleStateChanged;
				this.DistanceInteractor.WhenPostprocessed -= this.HandlePostProcessed;
			}
		}

		private void HandleStateChanged(InteractorStateChangeArgs args)
		{
			InteractorState newState = args.NewState;
			if (newState != InteractorState.Normal)
			{
				if (newState == InteractorState.Hover)
				{
					if (args.PreviousState == InteractorState.Normal)
					{
						this.InteractableSet(this.DistanceInteractor.DistanceInteractable);
					}
				}
			}
			else if (args.PreviousState != InteractorState.Disabled)
			{
				this.InteractableUnset();
			}
			if (args.NewState == InteractorState.Select || args.NewState == InteractorState.Disabled || args.PreviousState == InteractorState.Disabled)
			{
				this._shouldDrawLine = false;
				return;
			}
			if (args.NewState == InteractorState.Hover)
			{
				this._shouldDrawLine = true;
				return;
			}
			if (args.NewState == InteractorState.Normal)
			{
				this._shouldDrawLine = this._visibleDuringNormal;
			}
		}

		private void HandlePostProcessed()
		{
			if (this._shouldDrawLine)
			{
				this.UpdateLine();
				return;
			}
			this.HideLine();
		}

		protected virtual void InteractableSet(IRelativeToRef interactable)
		{
			Component component = interactable as Component;
			if (component == null)
			{
				this._target = null;
				return;
			}
			if (!component.TryGetComponent<IReticleData>(out this._target))
			{
				this._dummyTarget.Target = interactable.RelativeTo;
				this._target = this._dummyTarget;
			}
		}

		protected virtual void InteractableUnset()
		{
			this._target = null;
		}

		private void UpdateLine()
		{
			Vector3 forward = this.DistanceInteractor.Origin.forward;
			Vector3 vector = this.DistanceInteractor.Origin.position + forward * this.VisualOffset;
			Vector3 vector2 = this.TargetHit(this.DistanceInteractor.HitPoint);
			Vector3 middle = vector + forward * Vector3.Distance(vector, vector2) * 0.5f;
			for (int i = 0; i < this.NumLinePoints; i++)
			{
				float t = (float)i / ((float)this.NumLinePoints - 1f);
				Vector3 vector3 = DistantInteractionLineVisual.EvaluateBezier(vector, middle, vector2, t);
				this._linePoints[i] = vector3;
			}
			this.RenderLine(this._linePoints);
		}

		protected abstract void RenderLine(Vector3[] linePoints);

		protected abstract void HideLine();

		protected Vector3 TargetHit(Vector3 hitPoint)
		{
			if (this._target != null)
			{
				return this._target.ProcessHitPoint(hitPoint);
			}
			return this.DistanceInteractor.Origin.position + this.DistanceInteractor.Origin.forward * this._targetlessLength;
		}

		protected static Vector3 EvaluateBezier(Vector3 start, Vector3 middle, Vector3 end, float t)
		{
			t = Mathf.Clamp01(t);
			float num = 1f - t;
			return num * num * start + 2f * num * t * middle + t * t * end;
		}

		public void InjectAllDistantInteractionLineVisual(IDistanceInteractor interactor)
		{
			this.InjectDistanceInteractor(interactor);
		}

		public void InjectDistanceInteractor(IDistanceInteractor interactor)
		{
			this._distanceInteractor = (interactor as Object);
			this.DistanceInteractor = interactor;
		}

		[Tooltip("The distance interactor used as the origin of the line visual.")]
		[SerializeField]
		[Interface(typeof(IDistanceInteractor), new Type[]
		{

		})]
		private Object _distanceInteractor;

		[Tooltip("Where the line visual begins relative to the hand or controller. The lower the value, the closer the line.")]
		[SerializeField]
		private float _visualOffset = 0.07f;

		private Vector3[] _linePoints;

		[Tooltip("Should the line be visible when the distance interactor is in a normal state (not selecting, hovering, or disabled)?")]
		[SerializeField]
		private bool _visibleDuringNormal;

		private IReticleData _target;

		[Tooltip("The number of segments that make up the line. The more segments, the smoother the line.")]
		[SerializeField]
		private int _numLinePoints = 20;

		[Tooltip("The length of the line when the interactor is in a normal state. Only visible if the \"Visible during normal\" checkbox is also selected.")]
		[SerializeField]
		private float _targetlessLength = 0.5f;

		protected bool _started;

		private bool _shouldDrawLine;

		private DistantInteractionLineVisual.DummyPointReticle _dummyTarget = new DistantInteractionLineVisual.DummyPointReticle();

		private class DummyPointReticle : IReticleData
		{
			public Transform Target { get; set; }

			public Vector3 ProcessHitPoint(Vector3 hitPoint)
			{
				return this.Target.position;
			}
		}
	}
}
