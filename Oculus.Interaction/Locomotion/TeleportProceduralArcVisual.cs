using System;
using Oculus.Interaction.DistanceReticles;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class TeleportProceduralArcVisual : MonoBehaviour
	{
		public int ArcPointsCount
		{
			get
			{
				return this._arcPointsCount;
			}
			set
			{
				this._arcPointsCount = value;
			}
		}

		public Color NoDestinationTint
		{
			get
			{
				return this._noDestinationTint;
			}
			set
			{
				this._noDestinationTint = value;
			}
		}

		protected virtual void Awake()
		{
			this.Progress = (this._progress as IAxis1D);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._progress != null;
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._interactor.WhenPostprocessed += this.HandleInteractorPostProcessed;
				this._interactor.WhenStateChanged += this.HandleInteractorStateChanged;
				this._interactor.WhenInteractableSet.Action += this.HandleInteractableSet;
				this._interactor.WhenInteractableUnset.Action += this.HandleInteractableUnset;
				this._tubeRenderer.Hide();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._interactor.WhenPostprocessed -= this.HandleInteractorPostProcessed;
				this._interactor.WhenStateChanged -= this.HandleInteractorStateChanged;
				this._interactor.WhenInteractableSet.Action -= this.HandleInteractableSet;
				this._interactor.WhenInteractableUnset.Action -= this.HandleInteractableUnset;
				this._tubeRenderer.Hide();
			}
		}

		private void HandleInteractableSet(TeleportInteractable interactable)
		{
			if (interactable != null)
			{
				this._reticleData = interactable.GetComponent<IReticleData>();
			}
		}

		private void HandleInteractableUnset(TeleportInteractable obj)
		{
			this._reticleData = null;
		}

		private void HandleInteractorStateChanged(InteractorStateChangeArgs stateChange)
		{
			if (stateChange.NewState == InteractorState.Disabled)
			{
				this._tubeRenderer.Hide();
			}
		}

		private void HandleInteractorPostProcessed()
		{
			if (this._interactor.State == InteractorState.Disabled)
			{
				return;
			}
			Color tint = Color.white;
			if (!this._interactor.HasValidDestination())
			{
				tint = this._noDestinationTint;
			}
			Vector3 vector = this._interactor.ArcEnd.Point;
			if (this._reticleData != null)
			{
				vector = this._reticleData.ProcessHitPoint(vector);
			}
			this.UpdateVisualArcPoints(this._interactor.ArcOrigin, vector);
			this._tubeRenderer.Tint = tint;
			this._tubeRenderer.Progress = ((this.Progress != null) ? this.Progress.Value() : 0f);
			this._tubeRenderer.RenderTube(this._arcPoints, Space.World);
			this.UpdatePointer(tint, vector);
		}

		private void UpdatePointer(Color tint, Vector3 target)
		{
			if (this._pointer == null)
			{
				return;
			}
			this._pointer.Tint = tint;
			Vector3 vector = (this._pointerAnchor != null) ? this._pointerAnchor.position : this._interactor.ArcOrigin.position;
			Quaternion rotation = Quaternion.LookRotation(target - vector);
			this._pointer.SetPositionAndRotation(vector, rotation);
		}

		private void UpdateVisualArcPoints(Pose origin, Vector3 target)
		{
			if (this._arcPoints == null || this._arcPoints.Length != this.ArcPointsCount)
			{
				this._arcPoints = new TubePoint[this.ArcPointsCount];
			}
			float d = TeleportProceduralArcVisual.CalculateMidpointFactor(Vector3.Dot(origin.forward, Vector3.up));
			float magnitude = Vector3.ProjectOnPlane(target - origin.position, Vector3.up).magnitude;
			Vector3 middle = origin.position + origin.forward * magnitude * d;
			Vector3 b = origin.position - origin.forward;
			Vector3 b2 = new Vector3(1f / base.transform.lossyScale.x, 1f / base.transform.lossyScale.y, 1f / base.transform.lossyScale.z);
			float num = 0f;
			for (int i = 0; i < this.ArcPointsCount; i++)
			{
				float t = (float)i / ((float)this.ArcPointsCount - 1f);
				Vector3 vector = TeleportProceduralArcVisual.EvaluateBezierArc(origin.position, middle, target, t);
				Vector3 vector2 = vector - b;
				this._arcPoints[i].position = Vector3.Scale(vector, b2);
				this._arcPoints[i].rotation = Quaternion.LookRotation(vector2.normalized);
				if (i > 0)
				{
					num += vector2.magnitude;
				}
				b = vector;
			}
			for (int j = 1; j < this.ArcPointsCount; j++)
			{
				float magnitude2 = (this._arcPoints[j - 1].position - this._arcPoints[j].position).magnitude;
				this._arcPoints[j].relativeLength = this._arcPoints[j - 1].relativeLength + magnitude2 / num;
			}
		}

		private static Vector3 EvaluateBezierArc(Vector3 start, Vector3 middle, Vector3 end, float t)
		{
			t = Mathf.Clamp01(t);
			float num = 1f - t;
			return num * num * start + 2f * num * t * middle + t * t * end;
		}

		private static float CalculateMidpointFactor(float pitchDot)
		{
			return Mathf.Pow(1f - pitchDot * pitchDot, -0.25f) - 0.5f;
		}

		public void InjectAllTeleportProceduralArcVisual(TeleportInteractor interactor)
		{
			this.InjectTeleportInteractor(interactor);
		}

		public void InjectTeleportInteractor(TeleportInteractor interactor)
		{
			this._interactor = interactor;
		}

		public void InjectOptionalProgress(IAxis1D progress)
		{
			this._progress = (progress as Object);
			this.Progress = progress;
		}

		public void InjectOptionalPointer(PinchPointerVisual pointer)
		{
			this._pointer = pointer;
		}

		public void InjectOptionalPointerAnchor(Transform pointerAnchor)
		{
			this._pointerAnchor = pointerAnchor;
		}

		[SerializeField]
		private TeleportInteractor _interactor;

		[SerializeField]
		private TubeRenderer _tubeRenderer;

		[SerializeField]
		[Optional]
		private PinchPointerVisual _pointer;

		[SerializeField]
		[Optional]
		private Transform _pointerAnchor;

		[SerializeField]
		[Optional]
		[Interface(typeof(IAxis1D), new Type[]
		{

		})]
		private Object _progress;

		private IAxis1D Progress;

		[SerializeField]
		[Min(2f)]
		private int _arcPointsCount = 30;

		[SerializeField]
		private Color _noDestinationTint = Color.red;

		private TubePoint[] _arcPoints;

		private IReticleData _reticleData;

		protected bool _started;
	}
}
