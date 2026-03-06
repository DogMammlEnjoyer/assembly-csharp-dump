using System;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Position Binder")]
	[VFXBinder("Transform/Position")]
	internal class VFXPositionBinder : VFXSpaceableBinder
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
			return this.Target != null && component.HasVector3(this.m_Property);
		}

		public override void UpdateBinding(VisualEffect component)
		{
			Vector3 v = base.ApplySpacePosition(component, this.m_Property, this.Target.transform.position);
			component.SetVector3(this.m_Property, v);
		}

		public override string ToString()
		{
			return string.Format("Position : '{0}' -> {1}", this.m_Property, (this.Target == null) ? "(null)" : this.Target.name);
		}

		[VFXPropertyBinding(new string[]
		{
			"UnityEditor.VFX.Position",
			"UnityEngine.Vector3"
		})]
		[SerializeField]
		protected ExposedProperty m_Property = "Position";

		public Transform Target;
	}
}
