using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class LocomotionEventsConnection : MonoBehaviour, ILocomotionEventHandler, ILocomotionEventBroadcaster
	{
		private List<ILocomotionEventBroadcaster> Broadcasters { get; set; }

		private List<ILocomotionEventHandler> Handlers { get; set; }

		public event Action<LocomotionEvent> WhenLocomotionPerformed = delegate(LocomotionEvent <p0>)
		{
		};

		public event Action<LocomotionEvent, Pose> WhenLocomotionEventHandled = delegate(LocomotionEvent <p0>, Pose <p1>)
		{
		};

		protected virtual void Awake()
		{
			if (this.Broadcasters == null)
			{
				this.Broadcasters = this._broadcasters.ConvertAll<ILocomotionEventBroadcaster>((Object b) => b as ILocomotionEventBroadcaster);
			}
			if (this.Handlers == null)
			{
				this.Handlers = this._handlers.ConvertAll<ILocomotionEventHandler>((Object b) => b as ILocomotionEventHandler);
				ILocomotionEventHandler locomotionEventHandler = this._handler as ILocomotionEventHandler;
				if (locomotionEventHandler != null && !this.Handlers.Contains(locomotionEventHandler))
				{
					this.Handlers.Add(locomotionEventHandler);
				}
			}
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
				foreach (ILocomotionEventBroadcaster locomotionEventBroadcaster in this.Broadcasters)
				{
					locomotionEventBroadcaster.WhenLocomotionPerformed += this.HandleLocomotionEvent;
				}
				foreach (ILocomotionEventHandler locomotionEventHandler in this.Handlers)
				{
					locomotionEventHandler.WhenLocomotionEventHandled += this.HandlerWhenLocomotionEventHandled;
				}
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				foreach (ILocomotionEventBroadcaster locomotionEventBroadcaster in this.Broadcasters)
				{
					locomotionEventBroadcaster.WhenLocomotionPerformed -= this.HandleLocomotionEvent;
				}
				foreach (ILocomotionEventHandler locomotionEventHandler in this.Handlers)
				{
					locomotionEventHandler.WhenLocomotionEventHandled -= this.HandlerWhenLocomotionEventHandled;
				}
			}
		}

		private void HandlerWhenLocomotionEventHandled(LocomotionEvent arg1, Pose arg2)
		{
			this.WhenLocomotionEventHandled(arg1, arg2);
		}

		public void HandleLocomotionEvent(LocomotionEvent locomotionEvent)
		{
			if (this._started && base.isActiveAndEnabled)
			{
				this.WhenLocomotionPerformed(locomotionEvent);
				foreach (ILocomotionEventHandler locomotionEventHandler in this.Handlers)
				{
					locomotionEventHandler.HandleLocomotionEvent(locomotionEvent);
				}
			}
		}

		public void InjectAllLocomotionBroadcastersHandlerConnection(List<ILocomotionEventHandler> handlers)
		{
			this.InjectHandlers(handlers);
		}

		public void InjectOptionalBroadcasters(List<ILocomotionEventBroadcaster> broadcasters)
		{
			this.Broadcasters = broadcasters;
			this._broadcasters = broadcasters.ConvertAll<Object>((ILocomotionEventBroadcaster b) => b as Object);
		}

		public void InjectHandlers(List<ILocomotionEventHandler> handlers)
		{
			this.Handlers = handlers;
			this._handlers = handlers.ConvertAll<Object>((ILocomotionEventHandler b) => b as Object);
		}

		[Obsolete("Use the list version instead")]
		public void InjectHandler(ILocomotionEventHandler handler)
		{
			this._handler = (handler as Object);
		}

		[SerializeField]
		[Interface(typeof(ILocomotionEventBroadcaster), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.DontHide)]
		private List<Object> _broadcasters;

		[Obsolete("Use the list of Handlers instead")]
		[SerializeField]
		[Interface(typeof(ILocomotionEventHandler), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.Obsolete)]
		private Object _handler;

		[SerializeField]
		[Interface(typeof(ILocomotionEventHandler), new Type[]
		{

		})]
		private List<Object> _handlers;

		private bool _started;
	}
}
