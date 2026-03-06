using System;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/UI Dropdown Binder")]
	[VFXBinder("UI/Dropdown")]
	internal class VFXUIDropdownBinder : VFXBinderBase
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
			return this.Target != null && component.HasInt(this.m_Property);
		}

		public override void UpdateBinding(VisualEffect component)
		{
			component.SetInt(this.m_Property, this.Target.value);
		}

		public override string ToString()
		{
			return string.Format("UI Dropdown : '{0}' -> {1}", this.m_Property, (this.Target == null) ? "(null)" : this.Target.name);
		}

		[VFXPropertyBinding(new string[]
		{
			"System.Int32"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_Parameter")]
		protected ExposedProperty m_Property = "IntParameter";

		public Dropdown Target;
	}
}
