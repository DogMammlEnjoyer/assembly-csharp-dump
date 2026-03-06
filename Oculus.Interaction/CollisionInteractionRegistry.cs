using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class CollisionInteractionRegistry<TInteractor, TInteractable> : InteractableRegistry<TInteractor, TInteractable> where TInteractor : Interactor<TInteractor, TInteractable>, IRigidbodyRef where TInteractable : Interactable<TInteractor, TInteractable>, IRigidbodyRef
	{
		public CollisionInteractionRegistry()
		{
			this._rigidbodyCollisionMap = new Dictionary<Rigidbody, HashSet<TInteractable>>();
			this._broadcasters = new Dictionary<TInteractable, InteractableTriggerBroadcaster>();
		}

		public override void Register(TInteractable interactable)
		{
			base.Register(interactable);
			GameObject gameObject = interactable.Rigidbody.gameObject;
			InteractableTriggerBroadcaster interactableTriggerBroadcaster;
			if (!this._broadcasters.TryGetValue(interactable, out interactableTriggerBroadcaster))
			{
				interactableTriggerBroadcaster = gameObject.AddComponent<InteractableTriggerBroadcaster>();
				interactableTriggerBroadcaster.InjectAllInteractableTriggerBroadcaster(interactable);
				this._broadcasters.Add(interactable, interactableTriggerBroadcaster);
				InteractableTriggerBroadcaster interactableTriggerBroadcaster2 = interactableTriggerBroadcaster;
				interactableTriggerBroadcaster2.WhenTriggerEntered = (Action<IInteractable, Rigidbody>)Delegate.Combine(interactableTriggerBroadcaster2.WhenTriggerEntered, new Action<IInteractable, Rigidbody>(this.HandleTriggerEntered));
				InteractableTriggerBroadcaster interactableTriggerBroadcaster3 = interactableTriggerBroadcaster;
				interactableTriggerBroadcaster3.WhenTriggerExited = (Action<IInteractable, Rigidbody>)Delegate.Combine(interactableTriggerBroadcaster3.WhenTriggerExited, new Action<IInteractable, Rigidbody>(this.HandleTriggerExited));
			}
		}

		public override void Unregister(TInteractable interactable)
		{
			base.Unregister(interactable);
			InteractableTriggerBroadcaster interactableTriggerBroadcaster;
			if (this._broadcasters.TryGetValue(interactable, out interactableTriggerBroadcaster))
			{
				this._broadcasters.Remove(interactable);
				if (interactableTriggerBroadcaster != null)
				{
					interactableTriggerBroadcaster.enabled = false;
					InteractableTriggerBroadcaster interactableTriggerBroadcaster2 = interactableTriggerBroadcaster;
					interactableTriggerBroadcaster2.WhenTriggerEntered = (Action<IInteractable, Rigidbody>)Delegate.Remove(interactableTriggerBroadcaster2.WhenTriggerEntered, new Action<IInteractable, Rigidbody>(this.HandleTriggerEntered));
					InteractableTriggerBroadcaster interactableTriggerBroadcaster3 = interactableTriggerBroadcaster;
					interactableTriggerBroadcaster3.WhenTriggerExited = (Action<IInteractable, Rigidbody>)Delegate.Remove(interactableTriggerBroadcaster3.WhenTriggerExited, new Action<IInteractable, Rigidbody>(this.HandleTriggerExited));
					Object.Destroy(interactableTriggerBroadcaster);
				}
			}
		}

		private void HandleTriggerEntered(IInteractable interactable, Rigidbody rigidbody)
		{
			TInteractable item = interactable as TInteractable;
			if (!this._rigidbodyCollisionMap.ContainsKey(rigidbody))
			{
				this._rigidbodyCollisionMap.Add(rigidbody, new HashSet<TInteractable>());
			}
			this._rigidbodyCollisionMap[rigidbody].Add(item);
		}

		private void HandleTriggerExited(IInteractable interactable, Rigidbody rigidbody)
		{
			TInteractable item = interactable as TInteractable;
			HashSet<TInteractable> hashSet = this._rigidbodyCollisionMap[rigidbody];
			hashSet.Remove(item);
			if (hashSet.Count == 0)
			{
				this._rigidbodyCollisionMap.Remove(rigidbody);
			}
		}

		public override InteractableRegistry<TInteractor, TInteractable>.InteractableSet List(TInteractor interactor)
		{
			HashSet<TInteractable> onlyInclude;
			if (this._rigidbodyCollisionMap.TryGetValue(interactor.Rigidbody, out onlyInclude))
			{
				return base.List(interactor, onlyInclude);
			}
			return CollisionInteractionRegistry<TInteractor, TInteractable>._empty;
		}

		private Dictionary<Rigidbody, HashSet<TInteractable>> _rigidbodyCollisionMap;

		private Dictionary<TInteractable, InteractableTriggerBroadcaster> _broadcasters;

		private static readonly InteractableRegistry<TInteractor, TInteractable>.InteractableSet _empty;
	}
}
