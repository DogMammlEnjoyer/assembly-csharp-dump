using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class InteractableTriggerBroadcaster : MonoBehaviour
	{
		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._rigidbodyTriggers = new Dictionary<Rigidbody, bool>();
			this._rigidbodies = new List<Rigidbody>();
			this._skippedPhysics = false;
			this._forcedGlobalPhysicsUpdate = false;
			this.EndStart(ref this._started);
		}

		protected virtual void OnTriggerStay(Collider collider)
		{
			if (!this._started)
			{
				return;
			}
			Rigidbody attachedRigidbody = collider.attachedRigidbody;
			if (attachedRigidbody == null)
			{
				return;
			}
			if (!this._rigidbodyTriggers.ContainsKey(attachedRigidbody))
			{
				this.WhenTriggerEntered(this._interactable, attachedRigidbody);
				this._rigidbodyTriggers.Add(attachedRigidbody, true);
				return;
			}
			this._rigidbodyTriggers[attachedRigidbody] = true;
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				InteractableTriggerBroadcaster._broadcasters.Add(this);
			}
		}

		protected virtual void FixedUpdate()
		{
			if (Physics.autoSimulation)
			{
				this.UpdateTriggers();
				return;
			}
			this._skippedPhysics = true;
		}

		private void UpdateTriggers()
		{
			this._rigidbodies.Clear();
			this._rigidbodies.AddRange(this._rigidbodyTriggers.Keys);
			foreach (Rigidbody rigidbody in this._rigidbodies)
			{
				if (!this._rigidbodyTriggers[rigidbody])
				{
					this._rigidbodyTriggers.Remove(rigidbody);
					this.WhenTriggerExited(this._interactable, rigidbody);
				}
				else
				{
					this._rigidbodyTriggers[rigidbody] = false;
				}
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				foreach (Rigidbody arg in this._rigidbodyTriggers.Keys)
				{
					this.WhenTriggerExited(this._interactable, arg);
				}
				InteractableTriggerBroadcaster._broadcasters.Remove(this);
				this._rigidbodies.Clear();
			}
		}

		protected virtual void OnDestroy()
		{
			if (this._started)
			{
				this.WhenTriggerEntered = null;
				this.WhenTriggerExited = null;
			}
		}

		public static void ForceGlobalUpdateTriggers()
		{
			foreach (InteractableTriggerBroadcaster interactableTriggerBroadcaster in InteractableTriggerBroadcaster._broadcasters)
			{
				interactableTriggerBroadcaster._forcedGlobalPhysicsUpdate = true;
				interactableTriggerBroadcaster.UpdateTriggers();
			}
		}

		public void InjectAllInteractableTriggerBroadcaster(IInteractable interactable)
		{
			this.InjectInteractable(interactable);
		}

		public void InjectInteractable(IInteractable interactable)
		{
			this._interactable = interactable;
		}

		public Action<IInteractable, Rigidbody> WhenTriggerEntered = delegate(IInteractable <p0>, Rigidbody <p1>)
		{
		};

		public Action<IInteractable, Rigidbody> WhenTriggerExited = delegate(IInteractable <p0>, Rigidbody <p1>)
		{
		};

		private IInteractable _interactable;

		private Dictionary<Rigidbody, bool> _rigidbodyTriggers;

		private List<Rigidbody> _rigidbodies;

		private static HashSet<InteractableTriggerBroadcaster> _broadcasters = new HashSet<InteractableTriggerBroadcaster>();

		protected bool _started;

		private bool _skippedPhysics;

		private bool _forcedGlobalPhysicsUpdate;
	}
}
