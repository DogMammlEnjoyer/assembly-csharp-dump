using System;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/UI Slider Binder")]
	[VFXBinder("UI/Slider")]
	internal class VFXUISliderBinder : VFXBinderBase
	{
		public string Property
		{
			get
			{
				return (string)this.m_Property;
			}
			set
			{
				this.m_Property = value;
			}
		}

		public override bool IsValid(VisualEffect component)
		{
			return this.Target != null && component.HasFloat(this.m_Property);
		}

		public override void UpdateBinding(VisualEffect component)
		{
			component.SetFloat(this.m_Property, this.Target.value);
		}

		public override string ToString()
		{
			return string.Format("UI Slider : '{0}' -> {1}", this.m_Property, (this.Target == null) ? "(null)" : this.Target.name);
		}

		[VFXPropertyBinding(new string[]
		{
			"System.Single"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_Parameter")]
		protected ExposedProperty m_Property = "FloatParameter";

		public Slider Target;
	}
}
