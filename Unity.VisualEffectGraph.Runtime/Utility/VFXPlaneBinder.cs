using System;
using UnityEngine.Serialization;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Plane Binder")]
	[VFXBinder("Utility/Plane")]
	internal class VFXPlaneBinder : VFXSpaceableBinder
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
				this.UpdateSubProperties();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.UpdateSubProperties();
		}

		private void OnValidate()
		{
			this.UpdateSubProperties();
		}

		private void UpdateSubProperties()
		{
			this.Position = this.m_Property + "_position";
			this.Normal = this.m_Property + "_normal";
		}

		public override bool IsValid(VisualEffect component)
		{
			return this.Target != null && component.HasVector3(this.Position) && component.HasVector3(this.Normal);
		}

		public override void UpdateBinding(VisualEffect component)
		{
			Vector3 v;
			Vector3 v2;
			base.ApplySpacePositionNormal(component, this.Position, this.Target.transform, out v, out v2);
			component.SetVector3(this.Position, v);
			component.SetVector3(this.Normal, v2);
		}

		public override string ToString()
		{
			return string.Format("Plane : '{0}' -> {1}", this.m_Property, (this.Target == null) ? "(null)" : this.Target.name);
		}

		[VFXPropertyBinding(new string[]
		{
			"UnityEditor.VFX.Plane"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_Parameter")]
		protected ExposedProperty m_Property = "Plane";

		public Transform Target;

		private ExposedProperty Position;

		private ExposedProperty Normal;
	}
}
