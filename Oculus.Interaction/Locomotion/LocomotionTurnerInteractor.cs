using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class LocomotionTurnerInteractor : Interactor<LocomotionTurnerInteractor, LocomotionTurnerInteractable>, IAxis1D
	{
		public float DragThresold
		{
			get
			{
				return this._dragThresold;
			}
			set
			{
				this._dragThresold = value;
			}
		}

		public Pose MidPoint
		{
			get
			{
				return this.Transformer.ToWorldPose(this._midPoint);
			}
		}

		public Pose Origin
		{
			get
			{
				return this._origin.GetPose(Space.World);
			}
		}

		public event Action<float> WhenTurnDirectionChanged
		{
			add
			{
				this._whenTurnDirectionChanged = (Action<float>)Delegate.Combine(this._whenTurnDirectionChanged, value);
			}
			remove
			{
				this._whenTurnDirectionChanged = (Action<float>)Delegate.Remove(this._whenTurnDirectionChanged, value);
			}
		}

		public override bool ShouldHover
		{
			get
			{
				return base.State == InteractorState.Normal;
			}
		}

		public override bool ShouldUnhover
		{
			get
			{
				return false;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.Transformer = (this._transformer as ITrackingToWorldTransformer);
			base.Selector = (this._selector as ISelector);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.EndStart(ref this._started);
		}

		protected override void HandleEnabled()
		{
			base.HandleEnabled();
			Pose pose = this._origin.GetPose(Space.World);
			this.InitializeMidPoint(pose);
		}

		protected override void DoHoverUpdate()
		{
			base.DoHoverUpdate();
			this.UpdatePointers();
		}

		protected override void DoSelectUpdate()
		{
			base.DoSelectUpdate();
			this.UpdatePointers();
		}

		private void UpdatePointers()
		{
			Pose pose = this._origin.GetPose(Space.World);
			this.UpdateMidPoint(pose, this.MidPoint);
			this.DragMidPoint(this.MidPoint);
			this.UpdateAxisValue(pose, this.MidPoint);
		}

		private void InitializeMidPoint(Pose pointer)
		{
			Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(pointer.position - this._stabilizationPoint.position, Vector3.up).normalized, Vector3.up);
			Vector3 position = pointer.position;
			ITrackingToWorldTransformer transformer = this.Transformer;
			Pose pose = new Pose(position, rotation);
			this._midPoint = transformer.ToTrackingPose(pose);
		}

		private void UpdateMidPoint(Pose pointer, Pose midPoint)
		{
			float magnitude = Vector3.ProjectOnPlane(pointer.position - this._stabilizationPoint.position, Vector3.up).magnitude;
			Vector3 position = this._stabilizationPoint.position + midPoint.forward * magnitude;
			position.y = pointer.position.y;
			Quaternion rotation = midPoint.rotation;
			ITrackingToWorldTransformer transformer = this.Transformer;
			Pose pose = new Pose(position, rotation);
			this._midPoint = transformer.ToTrackingPose(pose);
		}

		private void DragMidPoint(Pose worldMidPoint)
		{
			Vector3 vector = worldMidPoint.position;
			float num = Mathf.Abs(this._axisValue) - this._dragThresold * base.transform.lossyScale.x;
			if (num <= 0f)
			{
				return;
			}
			Vector3 right = worldMidPoint.right;
			float d = (float)Math.Sign(this._axisValue);
			vector += right * d * num;
			Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(vector - this._stabilizationPoint.position, Vector3.up).normalized, Vector3.up);
			ITrackingToWorldTransformer transformer = this.Transformer;
			Pose pose = new Pose(vector, rotation);
			this._midPoint = transformer.ToTrackingPose(pose);
		}

		private void UpdateAxisValue(Pose pointer, Pose origin)
		{
			float num = Mathf.Sign(this._axisValue);
			Vector3 rhs = pointer.position - origin.position;
			this._axisValue = Vector3.Project(pointer.position - origin.position, origin.right).magnitude * Mathf.Sign(Vector3.Dot(origin.right, rhs));
			if (num != Mathf.Sign(this._axisValue))
			{
				this._whenTurnDirectionChanged(num);
			}
		}

		public float Value()
		{
			return Mathf.Clamp(this._axisValue / (this._dragThresold * base.transform.lossyScale.x), -1f, 1f);
		}

		protected override LocomotionTurnerInteractable ComputeCandidate()
		{
			return null;
		}

		public void InjectAllLocomotionTurnerInteractor(Transform origin, ISelector selector, Transform stabilizationPoint, ITrackingToWorldTransformer transformer)
		{
			this.InjectOrigin(origin);
			this.InjectSelector(selector);
			this.InjectStabilizationPoint(stabilizationPoint);
			this.InjectTransformer(transformer);
		}

		public void InjectOrigin(Transform origin)
		{
			this._origin = origin;
		}

		public void InjectSelector(ISelector selector)
		{
			this._selector = (selector as Object);
			base.Selector = selector;
		}

		public void InjectStabilizationPoint(Transform stabilizationPoint)
		{
			this._stabilizationPoint = stabilizationPoint;
		}

		public void InjectTransformer(ITrackingToWorldTransformer transformer)
		{
			this._transformer = (transformer as Object);
			this.Transformer = transformer;
		}

		[SerializeField]
		[Tooltip("Point in space used to drive the axis.")]
		private Transform _origin;

		[SerializeField]
		[Interface(typeof(ISelector), new Type[]
		{

		})]
		[Tooltip("Selector for the interactor.")]
		private Object _selector;

		[SerializeField]
		[Tooltip("Point used to stabilize the rotation of the point")]
		private Transform _stabilizationPoint;

		[SerializeField]
		[Interface(typeof(ITrackingToWorldTransformer), new Type[]
		{

		})]
		[Tooltip("Transformer is required so calculations can be done in Tracking space")]
		private Object _transformer;

		public ITrackingToWorldTransformer Transformer;

		[SerializeField]
		[Tooltip("Offset from the center point at which the pointer will be dragged")]
		private float _dragThresold = 0.1f;

		private Pose _midPoint = Pose.identity;

		private float _axisValue;

		private Action<float> _whenTurnDirectionChanged = delegate(float <p0>)
		{
		};
	}
}
