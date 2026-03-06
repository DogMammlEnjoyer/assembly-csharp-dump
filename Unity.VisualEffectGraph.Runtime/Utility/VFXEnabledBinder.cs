using System;
using UnityEngine.Serialization;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Enabled Binder")]
	[VFXBinder("GameObject/Enabled")]
	internal class VFXEnabledBinder : VFXBinderBase
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
			component.SetBool(this.m_Property, (this.check == VFXEnabledBinder.Check.ActiveInHierarchy) ? this.Target.activeInHierarchy : this.Target.activeSelf);
		}

		public override string ToString()
		{
			return string.Format("{2} : '{0}' -> {1}", this.m_Property, (this.Target == null) ? "(null)" : this.Target.name, this.check);
		}

		public VFXEnabledBinder.Check check;

		[VFXPropertyBinding(new string[]
		{
			"System.Boolean"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_Parameter")]
		protected ExposedProperty m_Property = "Enabled";

		public GameObject Target;

		public enum Check
		{
			ActiveInHierarchy,
			ActiveSelf
		}
	}
}
