using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class LocomotionActionsBroadcaster : MonoBehaviour, ILocomotionEventBroadcaster
	{
		public int Identifier
		{
			get
			{
				return this._identifier.ID;
			}
		}

		public event Action<LocomotionEvent> WhenLocomotionPerformed = delegate(LocomotionEvent <p0>)
		{
		};

		protected virtual void Awake()
		{
			this._identifier = UniqueIdentifier.Generate((this._context != null) ? this._context : Context.Global.GetInstance(), this);
		}

		public void SendLocomotionAction(LocomotionActionsBroadcaster.LocomotionAction action)
		{
			LocomotionEvent locomotionEvent = LocomotionActionsBroadcaster.CreateLocomotionEventAction(this.Identifier, action, Pose.identity, this._context);
			this.WhenLocomotionPerformed(locomotionEvent);
			LocomotionActionsBroadcaster.DisposeLocomotionAction(locomotionEvent, null);
		}

		public void Crouch()
		{
			this.SendLocomotionAction(LocomotionActionsBroadcaster.LocomotionAction.Crouch);
		}

		public void StandUp()
		{
			this.SendLocomotionAction(LocomotionActionsBroadcaster.LocomotionAction.StandUp);
		}

		public void ToggleCrouch()
		{
			this.SendLocomotionAction(LocomotionActionsBroadcaster.LocomotionAction.ToggleCrouch);
		}

		public void Run()
		{
			this.SendLocomotionAction(LocomotionActionsBroadcaster.LocomotionAction.Run);
		}

		public void Walk()
		{
			this.SendLocomotionAction(LocomotionActionsBroadcaster.LocomotionAction.Walk);
		}

		public void ToggleRun()
		{
			this.SendLocomotionAction(LocomotionActionsBroadcaster.LocomotionAction.ToggleRun);
		}

		public void Jump()
		{
			this.SendLocomotionAction(LocomotionActionsBroadcaster.LocomotionAction.Jump);
		}

		public void InjectOptionalContext(Context context)
		{
			this._context = context;
		}

		public static LocomotionEvent CreateLocomotionEventAction(int identifier, LocomotionActionsBroadcaster.LocomotionAction action, Pose pose = default(Pose), Context context = null)
		{
			LocomotionEvent result = new LocomotionEvent(identifier, pose, LocomotionEvent.TranslationType.None, LocomotionEvent.RotationType.None);
			LocomotionActionsBroadcaster.Decorator.GetFromContext(context).AddDecoration(result.EventId, action);
			return result;
		}

		public static bool TryGetLocomotionActions(LocomotionEvent locomotionEvent, out LocomotionActionsBroadcaster.LocomotionAction action, Context context = null)
		{
			LocomotionActionsBroadcaster.LocomotionAction locomotionAction;
			if (LocomotionActionsBroadcaster.Decorator.GetFromContext(context).TryGetDecoration(locomotionEvent.EventId, out locomotionAction))
			{
				action = locomotionAction;
				return true;
			}
			action = LocomotionActionsBroadcaster.LocomotionAction.Crouch;
			return false;
		}

		public static void DisposeLocomotionAction(LocomotionEvent locomotionEvent, Context context = null)
		{
			LocomotionActionsBroadcaster.Decorator.GetFromContext(context).RemoveDecoration(locomotionEvent.EventId);
		}

		[SerializeField]
		[Optional]
		private Context _context;

		private UniqueIdentifier _identifier;

		public enum LocomotionAction
		{
			Crouch,
			StandUp,
			ToggleCrouch,
			Run,
			Walk,
			ToggleRun,
			Jump
		}

		private class Decorator : ValueToValueDecorator<ulong, LocomotionActionsBroadcaster.LocomotionAction>
		{
			private Decorator()
			{
			}

			public static LocomotionActionsBroadcaster.Decorator GetFromContext(Context context = null)
			{
				if (context == null)
				{
					context = Context.Global.GetInstance();
				}
				return context.GetOrCreateSingleton<LocomotionActionsBroadcaster.Decorator>(() => new LocomotionActionsBroadcaster.Decorator());
			}
		}
	}
}
