using System;

namespace UnityEngine.VFX.Utility
{
	[RequireComponent(typeof(Renderer))]
	internal class VFXVisibilityEventBinder : VFXEventBinderBase
	{
		protected override void SetEventAttribute(object[] parameters)
		{
		}

		private void OnBecameVisible()
		{
			if (this.activation != VFXVisibilityEventBinder.Activation.OnBecameVisible)
			{
				return;
			}
			base.SendEventToVisualEffect(Array.Empty<object>());
		}

		private void OnBecameInvisible()
		{
			if (this.activation != VFXVisibilityEventBinder.Activation.OnBecameInvisible)
			{
				return;
			}
			base.SendEventToVisualEffect(Array.Empty<object>());
		}

		public VFXVisibilityEventBinder.Activation activation;

		public enum Activation
		{
			OnBecameVisible,
			OnBecameInvisible
		}
	}
}
