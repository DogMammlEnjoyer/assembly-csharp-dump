using System;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Transform Binder")]
	[VFXBinder("Transform/Transform")]
	internal class VFXTransformBinder : VFXSpaceableBinder
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
			this.Angles = this.m_Property + "_angles";
			this.Scale = this.m_Property + "_scale";
		}

		public override bool IsValid(VisualEffect component)
		{
			return this.Target != null && component.HasVector3(this.Position) && component.HasVector3(this.Angles) && component.HasVector3(this.Scale);
		}

		public override void UpdateBinding(VisualEffect component)
		{
			Vector3 v;
			Vector3 v2;
			Vector3 v3;
			base.ApplySpaceTRS(component, this.Position, this.Target, out v, out v2, out v3);
			component.SetVector3(this.Position, v);
			component.SetVector3(this.Angles, v2);
			component.SetVector3(this.Scale, v3);
		}

		public override string ToString()
		{
			return string.Format("Transform : '{0}' -> {1}", this.m_Property, (this.Target == null) ? "(null)" : this.Target.name);
		}

		[VFXPropertyBinding(new string[]
		{
			"UnityEditor.VFX.Transform"
		})]
		[SerializeField]
		protected ExposedProperty m_Property = "Transform";

		public Transform Target;

		private ExposedProperty Position;

		private ExposedProperty Angles;

		private ExposedProperty Scale;
	}
}
