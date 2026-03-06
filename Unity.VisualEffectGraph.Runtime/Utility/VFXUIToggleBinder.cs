using System;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/UI Toggle Binder")]
	[VFXBinder("UI/Toggle")]
	internal class VFXUIToggleBinder : VFXBinderBase
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
			return this.Target != null && component.HasBool(this.m_Property);
		}

		public override void UpdateBinding(VisualEffect component)
		{
			component.SetBool(this.m_Property, this.Target.isOn);
		}

		public override string ToString()
		{
			return string.Format("UI Toggle : '{0}' -> {1}", this.m_Property, (this.Target == null) ? "(null)" : this.Target.name);
		}

		[VFXPropertyBinding(new string[]
		{
			"System.Boolean"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_Parameter")]
		protected ExposedProperty m_Property = "BoolParameter";

		public Toggle Target;
	}
}
