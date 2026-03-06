using System;

namespace UnityEngine.VFX.Utility
{
	[ExecuteAlways]
	[RequireComponent(typeof(VisualEffect))]
	public abstract class VFXOutputEventAbstractHandler : MonoBehaviour
	{
		public abstract bool canExecuteInEditor { get; }

		private protected VisualEffect m_VisualEffect { protected get; private set; }

		protected virtual void OnEnable()
		{
			this.m_VisualEffect = base.GetComponent<VisualEffect>();
			if (this.m_VisualEffect != null)
			{
				VisualEffect visualEffect = this.m_VisualEffect;
				visualEffect.outputEventReceived = (Action<VFXOutputEventArgs>)Delegate.Combine(visualEffect.outputEventReceived, new Action<VFXOutputEventArgs>(this.OnOutputEventRecieved));
			}
		}

		protected virtual void OnDisable()
		{
			if (this.m_VisualEffect != null)
			{
				VisualEffect visualEffect = this.m_VisualEffect;
				visualEffect.outputEventReceived = (Action<VFXOutputEventArgs>)Delegate.Remove(visualEffect.outputEventReceived, new Action<VFXOutputEventArgs>(this.OnOutputEventRecieved));
			}
		}

		private void OnOutputEventRecieved(VFXOutputEventArgs args)
		{
			if ((Application.isPlaying || (this.executeInEditor && this.canExecuteInEditor)) && args.nameId == this.outputEvent)
			{
				this.OnVFXOutputEvent(args.eventAttribute);
			}
		}

		public abstract void OnVFXOutputEvent(VFXEventAttribute eventAttribute);

		public bool executeInEditor = true;

		public ExposedProperty outputEvent = "On Received Event";
	}
}
