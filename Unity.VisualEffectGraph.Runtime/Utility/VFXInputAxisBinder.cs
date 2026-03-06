using System;
using UnityEngine.Serialization;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Input Axis Binder")]
	[VFXBinder("Input/Axis")]
	internal class VFXInputAxisBinder : VFXBinderBase
	{
		public string AxisProperty
		{
			get
			{
				return (string)this.m_AxisProperty;
			}
			set
			{
				this.m_AxisProperty = value;
			}
		}

		public override bool IsValid(VisualEffect component)
		{
			return component.HasFloat(this.m_AxisProperty);
		}

		public override void UpdateBinding(VisualEffect component)
		{
		}

		public override string ToString()
		{
			return string.Format("Input Axis: '{0}' -> {1}", this.m_AxisProperty, this.AxisName.ToString());
		}

		[VFXPropertyBinding(new string[]
		{
			"System.Single"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_AxisParameter")]
		protected ExposedProperty m_AxisProperty = "Axis";

		public string AxisName = "Horizontal";

		public float AccumulateSpeed = 1f;

		public bool Accumulate = true;
	}
}
