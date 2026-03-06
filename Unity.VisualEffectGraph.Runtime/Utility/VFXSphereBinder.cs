using System;
using UnityEngine.Serialization;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Sphere Collider Binder")]
	[VFXBinder("Collider/Sphere")]
	internal class VFXSphereBinder : VFXSpaceableBinder
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
			this.m_Old_Center = this.m_Property + "_center";
			this.m_New_Center = this.m_Property + "_transform_position";
			this.m_Radius = this.m_Property + "_radius";
		}

		public override bool IsValid(VisualEffect component)
		{
			return this.Target != null && (component.HasVector3(this.m_New_Center) || component.HasVector3(this.m_Old_Center)) && component.HasFloat(this.m_Radius);
		}

		public override void UpdateBinding(VisualEffect component)
		{
			ExposedProperty exposedProperty = this.m_New_Center;
			if (!component.HasVector3(this.m_New_Center))
			{
				exposedProperty = this.m_Old_Center;
			}
			Vector3 a;
			Vector3 scale;
			base.ApplySpaceTS(component, exposedProperty, this.Target.transform, out a, out scale);
			Vector3 v = a + this.Target.center;
			if (this.m_New_Center == exposedProperty)
			{
				component.SetVector3(this.m_New_Center, v);
			}
			else
			{
				component.SetVector3(this.m_Old_Center, v);
			}
			component.SetFloat(this.m_Radius, this.Target.radius * this.GetSphereColliderScale(scale));
		}

		public float GetSphereColliderScale(Vector3 scale)
		{
			return Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
		}

		public override string ToString()
		{
			return string.Format("Sphere : '{0}' -> {1}", this.m_Property, (this.Target == null) ? "(null)" : this.Target.name);
		}

		[VFXPropertyBinding(new string[]
		{
			"UnityEditor.VFX.Sphere",
			"UnityEditor.VFX.TSphere"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_Parameter")]
		protected ExposedProperty m_Property = "Sphere";

		public SphereCollider Target;

		private ExposedProperty m_Old_Center;

		private ExposedProperty m_New_Center;

		private ExposedProperty m_Radius;
	}
}
