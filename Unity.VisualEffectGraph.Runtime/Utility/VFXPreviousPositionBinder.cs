using System;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Previous Position Binder")]
	[VFXBinder("Transform/Position (Previous)")]
	internal class VFXPreviousPositionBinder : VFXSpaceableBinder
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			this.oldPosition = ((this.Target != null) ? this.Target.position : Vector3.zero);
		}

		public override bool IsValid(VisualEffect component)
		{
			return this.Target != null && component.HasVector3(this.m_Property);
		}

		public override void UpdateBinding(VisualEffect component)
		{
			component.SetVector3(this.m_Property, this.oldPosition);
			Vector3 vector = base.ApplySpacePosition(component, this.m_Property, this.Target.position);
			this.oldPosition = vector;
		}

		public override string ToString()
		{
			return string.Format("Previous Position : '{0}' -> {1}", this.m_Property, (this.Target == null) ? "(null)" : this.Target.name);
		}

		[VFXPropertyBinding(new string[]
		{
			"UnityEngine.Vector3"
		})]
		public ExposedProperty m_Property = "PreviousPosition";

		public Transform Target;

		private Vector3 oldPosition;
	}
}
