using System;
using System.Collections.Generic;

namespace UnityEngine.VFX.Utility
{
	[RequireComponent(typeof(Collider))]
	internal class VFXTriggerEventBinder : VFXEventBinderBase
	{
		protected override void SetEventAttribute(object[] parameters)
		{
			Collider collider = (Collider)parameters[0];
			this.eventAttribute.SetVector3(this.positionParameter, collider.transform.position);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (this.activation != VFXTriggerEventBinder.Activation.OnEnter)
			{
				return;
			}
			if (!this.colliders.Contains(other))
			{
				return;
			}
			base.SendEventToVisualEffect(new object[]
			{
				other
			});
		}

		private void OnTriggerExit(Collider other)
		{
			if (this.activation != VFXTriggerEventBinder.Activation.OnExit)
			{
				return;
			}
			if (!this.colliders.Contains(other))
			{
				return;
			}
			base.SendEventToVisualEffect(new object[]
			{
				other
			});
		}

		private void OnTriggerStay(Collider other)
		{
			if (this.activation != VFXTriggerEventBinder.Activation.OnStay)
			{
				return;
			}
			if (!this.colliders.Contains(other))
			{
				return;
			}
			base.SendEventToVisualEffect(new object[]
			{
				other
			});
		}

		public List<Collider> colliders = new List<Collider>();

		public VFXTriggerEventBinder.Activation activation;

		private ExposedProperty positionParameter = "position";

		public enum Activation
		{
			OnEnter,
			OnExit,
			OnStay
		}
	}
}
