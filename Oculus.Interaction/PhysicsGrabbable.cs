using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
	[Obsolete("Use Grabbable and/or RigidbodyKinematicLocker instead")]
	public class PhysicsGrabbable : MonoBehaviour
	{
		private IPointable Pointable { get; set; }

		public event Action<Vector3, Vector3> WhenVelocitiesApplied = delegate(Vector3 <p0>, Vector3 <p1>)
		{
		};

		private void Reset()
		{
			this._pointable = (base.GetComponent<IPointable>() as Object);
			this._rigidbody = base.GetComponent<Rigidbody>();
		}

		protected virtual void Awake()
		{
			this.Pointable = (this._pointable as IPointable);
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
				this.Pointable.WhenPointerEventRaised += this.HandlePointerEventRaised;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Pointable.WhenPointerEventRaised -= this.HandlePointerEventRaised;
				if (this._selectorsCount != 0)
				{
					this._selectorsCount = 0;
					this.ReenablePhysics();
				}
			}
		}

		private void HandlePointerEventRaised(PointerEvent evt)
		{
			switch (evt.Type)
			{
			case PointerEventType.Select:
				this.AddSelection();
				return;
			case PointerEventType.Unselect:
			case PointerEventType.Cancel:
				this.RemoveSelection();
				break;
			case PointerEventType.Move:
				break;
			default:
				return;
			}
		}

		private void AddSelection()
		{
			int selectorsCount = this._selectorsCount;
			this._selectorsCount = selectorsCount + 1;
			if (selectorsCount == 0)
			{
				this.DisablePhysics();
			}
		}

		private void RemoveSelection()
		{
			int num = this._selectorsCount - 1;
			this._selectorsCount = num;
			if (num == 0)
			{
				this.ReenablePhysics();
			}
			this._selectorsCount = Mathf.Max(0, this._selectorsCount);
		}

		private void DisablePhysics()
		{
			this.CachePhysicsState();
			this._rigidbody.LockKinematic();
		}

		private void ReenablePhysics()
		{
			if (this._scaleMassWithSize)
			{
				float num = this._initialScale.x * this._initialScale.y * this._initialScale.z;
				Vector3 localScale = this._rigidbody.transform.localScale;
				float num2 = localScale.x * localScale.y * localScale.z / num;
				this._rigidbody.mass *= num2;
			}
			this._rigidbody.UnlockKinematic();
		}

		public void ApplyVelocities(Vector3 linearVelocity, Vector3 angularVelocity)
		{
			this._hasPendingForce = true;
			this._linearVelocity = linearVelocity;
			this._angularVelocity = angularVelocity;
		}

		private void FixedUpdate()
		{
			if (this._hasPendingForce)
			{
				this._hasPendingForce = false;
				this._rigidbody.AddForce(this._linearVelocity, ForceMode.VelocityChange);
				this._rigidbody.AddTorque(this._angularVelocity, ForceMode.VelocityChange);
				this.WhenVelocitiesApplied(this._linearVelocity, this._angularVelocity);
			}
		}

		private void CachePhysicsState()
		{
			this._initialScale = this._rigidbody.transform.localScale;
		}

		public void InjectAllPhysicsGrabbable(IPointable pointable, Rigidbody rigidbody)
		{
			this.InjectPointable(pointable);
			this.InjectRigidbody(rigidbody);
		}

		[Obsolete("Use InjectAllPhysicsGrabbable with IPointable instead")]
		public void InjectAllPhysicsGrabbable(Grabbable grabbable, Rigidbody rigidbody)
		{
			this.InjectPointable(grabbable);
			this.InjectRigidbody(rigidbody);
		}

		[Obsolete("Use InjectPointable instead")]
		public void InjectGrabbable(Grabbable grabbable)
		{
			this.InjectPointable(grabbable);
		}

		public void InjectPointable(IPointable pointable)
		{
			this._pointable = (pointable as Object);
			this.Pointable = pointable;
		}

		public void InjectRigidbody(Rigidbody rigidbody)
		{
			this._rigidbody = rigidbody;
		}

		public void InjectOptionalScaleMassWithSize(bool scaleMassWithSize)
		{
			this._scaleMassWithSize = scaleMassWithSize;
		}

		[SerializeField]
		[Interface(typeof(IPointable), new Type[]
		{

		})]
		[FormerlySerializedAs("_grabbable")]
		private Object _pointable;

		[SerializeField]
		private Rigidbody _rigidbody;

		[SerializeField]
		[Tooltip("If enabled, the object's mass will scale appropriately as the scale of the object changes.")]
		private bool _scaleMassWithSize = true;

		private Vector3 _initialScale;

		private bool _hasPendingForce;

		private Vector3 _linearVelocity;

		private Vector3 _angularVelocity;

		private int _selectorsCount;

		protected bool _started;
	}
}
