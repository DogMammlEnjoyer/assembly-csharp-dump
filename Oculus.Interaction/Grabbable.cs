using System;
using System.Collections.Generic;
using Oculus.Interaction.Throw;
using UnityEngine;
using UnityEngine.Pool;

namespace Oculus.Interaction
{
	public class Grabbable : PointableElement, IGrabbable, ITimeConsumer
	{
		public int MaxGrabPoints
		{
			get
			{
				return this._maxGrabPoints;
			}
			set
			{
				this._maxGrabPoints = value;
			}
		}

		public Transform Transform
		{
			get
			{
				return this._targetTransform;
			}
		}

		public List<Pose> GrabPoints
		{
			get
			{
				return this._selectingPoints;
			}
		}

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
			if (this._throw != null)
			{
				this._throw.SetTimeProvider(timeProvider);
			}
		}

		protected virtual void Reset()
		{
			this._rigidbody = base.GetComponent<Rigidbody>();
		}

		protected override void Awake()
		{
			base.Awake();
			this.OneGrabTransformer = (this._oneGrabTransformer as ITransformer);
			this.TwoGrabTransformer = (this._twoGrabTransformer as ITransformer);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			if (this._targetTransform == null)
			{
				this._targetTransform = base.transform;
			}
			if (this._oneGrabTransformer != null)
			{
				this.OneGrabTransformer.Initialize(this);
			}
			if (this._twoGrabTransformer != null)
			{
				this.TwoGrabTransformer.Initialize(this);
			}
			if (this.OneGrabTransformer == null && this.TwoGrabTransformer == null)
			{
				this.GenerateTransformer().Initialize(this);
			}
			if (this._rigidbody != null && this._throwWhenUnselected)
			{
				this._throw = new Grabbable.ThrowWhenUnselected(this._rigidbody, this);
				this._throw.SetTimeProvider(this._timeProvider);
			}
			this.EndStart(ref this._started);
		}

		protected override void OnDisable()
		{
			if (this._started)
			{
				this.EndTransform();
			}
			base.OnDisable();
		}

		protected virtual void OnDestroy()
		{
			if (this._throw != null)
			{
				this._throw.Dispose();
				this._throw = null;
			}
		}

		private ITransformer GenerateTransformer()
		{
			ITransformer transformer = base.gameObject.AddComponent<GrabFreeTransformer>();
			this.InjectOptionalOneGrabTransformer(transformer);
			this.InjectOptionalTwoGrabTransformer(transformer);
			return transformer;
		}

		public override void ProcessPointerEvent(PointerEvent evt)
		{
			switch (evt.Type)
			{
			case PointerEventType.Select:
				this.EndTransform();
				break;
			case PointerEventType.Unselect:
				this.ForceMove(evt);
				this.EndTransform();
				break;
			case PointerEventType.Cancel:
				this.EndTransform();
				break;
			}
			base.ProcessPointerEvent(evt);
			switch (evt.Type)
			{
			case PointerEventType.Select:
				this.BeginTransform();
				return;
			case PointerEventType.Unselect:
				this.BeginTransform();
				return;
			case PointerEventType.Move:
				this.UpdateTransform();
				return;
			default:
				return;
			}
		}

		protected override void PointableElementUpdated(PointerEvent evt)
		{
			this.UpdateKinematicLock(base.SelectingPointsCount > 0);
			base.PointableElementUpdated(evt);
		}

		private void UpdateKinematicLock(bool isGrabbing)
		{
			if (this._rigidbody == null || !this._kinematicWhileSelected)
			{
				return;
			}
			if (!this._isKinematicLocked && isGrabbing)
			{
				this._isKinematicLocked = true;
				this._rigidbody.LockKinematic();
				return;
			}
			if (this._isKinematicLocked && !isGrabbing)
			{
				this._isKinematicLocked = false;
				this._rigidbody.UnlockKinematic();
			}
		}

		private void ForceMove(PointerEvent releaseEvent)
		{
			PointerEvent evt = new PointerEvent(releaseEvent.Identifier, PointerEventType.Move, releaseEvent.Pose, releaseEvent.Data);
			this.ProcessPointerEvent(evt);
		}

		private void BeginTransform()
		{
			this.EndTransform();
			int num = this._selectingPoints.Count;
			if (this._maxGrabPoints != -1)
			{
				num = Mathf.Min(num, this._maxGrabPoints);
			}
			if (num != 1)
			{
				if (num != 2)
				{
					this._activeTransformer = null;
				}
				else
				{
					this._activeTransformer = this.TwoGrabTransformer;
				}
			}
			else
			{
				this._activeTransformer = this.OneGrabTransformer;
			}
			if (this._activeTransformer == null)
			{
				return;
			}
			this._activeTransformer.BeginTransform();
		}

		private void UpdateTransform()
		{
			if (this._activeTransformer == null)
			{
				return;
			}
			this._activeTransformer.UpdateTransform();
		}

		private void EndTransform()
		{
			if (this._activeTransformer == null)
			{
				return;
			}
			this._activeTransformer.EndTransform();
			this._activeTransformer = null;
		}

		public void InjectOptionalOneGrabTransformer(ITransformer transformer)
		{
			this._oneGrabTransformer = (transformer as Object);
			this.OneGrabTransformer = transformer;
		}

		public void InjectOptionalTwoGrabTransformer(ITransformer transformer)
		{
			this._twoGrabTransformer = (transformer as Object);
			this.TwoGrabTransformer = transformer;
		}

		public void InjectOptionalTargetTransform(Transform targetTransform)
		{
			this._targetTransform = targetTransform;
		}

		public void InjectOptionalRigidbody(Rigidbody rigidbody)
		{
			this._rigidbody = rigidbody;
		}

		public void InjectOptionalThrowWhenUnselected(bool throwWehenUnselected)
		{
			this._throwWhenUnselected = throwWehenUnselected;
		}

		public void InjectOptionalKinematicWhileSelected(bool kinematicWhileSelected)
		{
			this._kinematicWhileSelected = kinematicWhileSelected;
		}

		[Tooltip("A One Grab...Transformer component, which should be attached to the grabbable object. Defaults to One Grab Free Transformer. If you set the Two Grab Transformer property and still want to use one hand for grabs, you must set this property as well.")]
		[SerializeField]
		[Interface(typeof(ITransformer), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.AutoGenerated)]
		private Object _oneGrabTransformer;

		[Tooltip("A Two Grab...Transformer component, which should be attached to the grabbable object. If you set this property but also want to use one hand for grabs, you must set the One Grab Transformer property.")]
		[SerializeField]
		[Interface(typeof(ITransformer), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.AutoGenerated)]
		private Object _twoGrabTransformer;

		[Tooltip("The target transform of the Grabbable. If unassigned, the transform of this GameObject will be used.")]
		[SerializeField]
		[Optional(OptionalAttribute.Flag.AutoGenerated)]
		private Transform _targetTransform;

		[Tooltip("The maximum number of grab points. Can be either -1 (unlimited), 1, or 2.")]
		[SerializeField]
		[Min(-1f)]
		private int _maxGrabPoints = -1;

		[Header("Physics")]
		[SerializeField]
		[Optional(OptionalAttribute.Flag.DontHide)]
		[Tooltip("Use this rigidbody to control its physics properties while grabbing.")]
		private Rigidbody _rigidbody;

		[SerializeField]
		[Tooltip("Locks the referenced rigidbody to a kinematic while selected.")]
		private bool _kinematicWhileSelected = true;

		[SerializeField]
		[Tooltip("Applies throwing velocities to the rigidbody when fully released.")]
		private bool _throwWhenUnselected = true;

		private Func<float> _timeProvider = () => Time.time;

		private ITransformer _activeTransformer;

		private ITransformer OneGrabTransformer;

		private ITransformer TwoGrabTransformer;

		private Grabbable.ThrowWhenUnselected _throw;

		private bool _isKinematicLocked;

		private class ThrowWhenUnselected : ITimeConsumer, IDisposable
		{
			public void SetTimeProvider(Func<float> timeProvider)
			{
				this._timeProvider = timeProvider;
			}

			public ThrowWhenUnselected(Rigidbody rigidbody, IPointable pointable)
			{
				this._rigidbody = rigidbody;
				this._pointable = pointable;
				this._pointable.WhenPointerEventRaised += this.HandlePointerEventRaised;
			}

			public void Dispose()
			{
				this._pointable.WhenPointerEventRaised -= this.HandlePointerEventRaised;
			}

			private void AddSelection(int selectorId)
			{
				if (this._selectors == null)
				{
					this.Initialize();
				}
				this._selectors.Add(selectorId);
			}

			private void RemoveSelection(int selectorId, bool canThrow)
			{
				this._selectors.Remove(selectorId);
				if (this._selectors.Count == 0)
				{
					if (canThrow)
					{
						this.Process(false);
						this.LoadThrowVelocities();
					}
					this.Teardown();
				}
			}

			private void HandlePointerEventRaised(PointerEvent evt)
			{
				switch (evt.Type)
				{
				case PointerEventType.Select:
					this.AddSelection(evt.Identifier);
					return;
				case PointerEventType.Unselect:
					this.MarkFrameConfidence(evt.Identifier);
					this.RemoveSelection(evt.Identifier, true);
					break;
				case PointerEventType.Move:
					if (this._selectors != null && this._selectors.Contains(evt.Identifier))
					{
						this.Process(true);
						this.MarkFrameConfidence(evt.Identifier);
						return;
					}
					break;
				case PointerEventType.Cancel:
					this.RemoveSelection(evt.Identifier, false);
					return;
				default:
					return;
				}
			}

			private void Initialize()
			{
				this._selectors = Grabbable.ThrowWhenUnselected._selectorsPool.Get();
				this._ransacVelocity = Grabbable.ThrowWhenUnselected._ransacVelocityPool.Get();
				this._ransacVelocity.Initialize();
			}

			private void Teardown()
			{
				Grabbable.ThrowWhenUnselected._selectorsPool.Release(this._selectors);
				this._selectors = null;
				Grabbable.ThrowWhenUnselected._ransacVelocityPool.Release(this._ransacVelocity);
				this._ransacVelocity = null;
			}

			private void MarkFrameConfidence(int emitterKey)
			{
				if (!this._isHighConfidence)
				{
					return;
				}
				bool flag;
				if (HandTrackingConfidenceProvider.TryGetTrackingConfidence(emitterKey, out flag) && !flag)
				{
					this._isHighConfidence = false;
				}
			}

			private void Process(bool saveAsPreviousFrame)
			{
				float num = this._timeProvider();
				Pose pose = this._rigidbody.transform.GetPose(Space.World);
				if (num > this._prevTime || !saveAsPreviousFrame)
				{
					float time = saveAsPreviousFrame ? this._prevTime : num;
					this._isHighConfidence &= (pose.position != this._prevPose.position);
					this._ransacVelocity.Process(pose, time, this._isHighConfidence);
					this._isHighConfidence = true;
				}
				this._prevTime = num;
				this._prevPose = pose;
			}

			private void LoadThrowVelocities()
			{
				Vector3 velocity;
				Vector3 angularVelocity;
				this._ransacVelocity.GetVelocities(out velocity, out angularVelocity);
				this._rigidbody.velocity = velocity;
				this._rigidbody.angularVelocity = angularVelocity;
			}

			private Rigidbody _rigidbody;

			private IPointable _pointable;

			private HashSet<int> _selectors;

			private Func<float> _timeProvider = () => Time.time;

			private static IObjectPool<RANSACVelocity> _ransacVelocityPool = new ObjectPool<RANSACVelocity>(() => new RANSACVelocity(10, 2), null, null, null, false, 2, 10000);

			private static IObjectPool<HashSet<int>> _selectorsPool = new ObjectPool<HashSet<int>>(() => new HashSet<int>(), null, delegate(HashSet<int> s)
			{
				s.Clear();
			}, null, false, 2, 10000);

			private RANSACVelocity _ransacVelocity;

			private Pose _prevPose = Pose.identity;

			private float _prevTime;

			private bool _isHighConfidence = true;
		}
	}
}
