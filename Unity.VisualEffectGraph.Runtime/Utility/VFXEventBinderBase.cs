using System;

namespace UnityEngine.VFX.Utility
{
	internal abstract class VFXEventBinderBase : MonoBehaviour
	{
		protected virtual void OnEnable()
		{
			this.UpdateCacheEventAttribute();
		}

		private void OnValidate()
		{
			this.UpdateCacheEventAttribute();
		}

		private void UpdateCacheEventAttribute()
		{
			if (this.target != null)
			{
				this.eventAttribute = this.target.CreateVFXEventAttribute();
				return;
			}
			this.eventAttribute = null;
		}

		protected abstract void SetEventAttribute(object[] parameters = null);

		protected void SendEventToVisualEffect(params object[] parameters)
		{
			if (this.target != null)
			{
				this.SetEventAttribute(parameters);
				this.target.SendEvent(this.EventName, this.eventAttribute);
			}
		}

		[SerializeField]
		protected VisualEffect target;

		public string EventName = "Event";

		[SerializeField]
		[HideInInspector]
		protected VFXEventAttribute eventAttribute;
	}
}
