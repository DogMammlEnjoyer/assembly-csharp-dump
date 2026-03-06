using System;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Velocity Binder")]
	[VFXBinder("Transform/Velocity")]
	internal class VFXVelocityBinder : VFXSpaceableBinder
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

		public override void Reset()
		{
			this.m_PreviousTime = VFXVelocityBinder.invalidPreviousTime;
		}

		public override void UpdateBinding(VisualEffect component)
		{
			Vector3 v = Vector3.zero;
			float time = Time.time;
			Vector3 vector = base.ApplySpacePosition(component, this.m_Property, this.Target.position);
			if (this.m_PreviousTime != VFXVelocityBinder.invalidPreviousTime)
			{
				Vector3 vector2 = vector - this.m_PreviousPosition;
				float num = time - this.m_PreviousTime;
				if (Vector3.SqrMagnitude(vector2) > Mathf.Epsilon && num > Mathf.Epsilon)
				{
					v = vector2 / num;
				}
			}
			component.SetVector3(this.m_Property, v);
			this.m_PreviousPosition = vector;
			this.m_PreviousTime = time;
		}

		public override string ToString()
		{
			return string.Format("Velocity : '{0}' -> {1}", this.m_Property, (this.Target == null) ? "(null)" : this.Target.name);
		}

		[VFXPropertyBinding(new string[]
		{
			"UnityEngine.Vector3"
		})]
		[SerializeField]
		public ExposedProperty m_Property = "Velocity";

		public Transform Target;

		private static readonly float invalidPreviousTime = -1f;

		private float m_PreviousTime = VFXVelocityBinder.invalidPreviousTime;

		private Vector3 m_PreviousPosition = Vector3.zero;
	}
}
