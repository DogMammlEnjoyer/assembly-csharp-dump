using System;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Raycast Binder")]
	[VFXBinder("Physics/Raycast")]
	internal class VFXRaycastBinder : VFXBinderBase
	{
		public string TargetPosition
		{
			get
			{
				return (string)this.m_TargetPosition;
			}
			set
			{
				this.m_TargetPosition = value;
				this.UpdateSubProperties();
			}
		}

		public string TargetNormal
		{
			get
			{
				return (string)this.m_TargetNormal;
			}
			set
			{
				this.m_TargetNormal = value;
				this.UpdateSubProperties();
			}
		}

		public string TargetHit
		{
			get
			{
				return (string)this.m_TargetHit;
			}
			set
			{
				this.m_TargetHit = value;
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
			this.m_TargetPosition_position = this.m_TargetPosition + "_position";
			this.m_TargetNormal_direction = this.m_TargetNormal + "_direction";
		}

		public override bool IsValid(VisualEffect component)
		{
			return component.HasVector3(this.m_TargetPosition_position) && component.HasVector3(this.m_TargetNormal_direction) && component.HasBool(this.m_TargetHit) && this.RaycastSource != null;
		}

		public override void UpdateBinding(VisualEffect component)
		{
			Vector3 direction = (this.RaycastDirectionSpace == VFXRaycastBinder.Space.Local) ? this.RaycastSource.transform.TransformDirection(this.RaycastDirection) : this.RaycastDirection;
			bool b = Physics.Raycast(new Ray(this.RaycastSource.transform.position, direction), out this.m_HitInfo, this.MaxDistance, this.Layers);
			component.SetVector3(this.m_TargetPosition_position, this.m_HitInfo.point);
			component.SetVector3(this.m_TargetNormal_direction, this.m_HitInfo.normal);
			component.SetBool(this.TargetHit, b);
		}

		public override string ToString()
		{
			return string.Format(string.Format("Raycast : {0} -> {1} ({2})", (this.RaycastSource == null) ? "null" : this.RaycastSource.name, this.RaycastDirection, this.RaycastDirectionSpace), Array.Empty<object>());
		}

		[VFXPropertyBinding(new string[]
		{
			"UnityEditor.VFX.Position"
		})]
		[SerializeField]
		protected ExposedProperty m_TargetPosition = "TargetPosition";

		[VFXPropertyBinding(new string[]
		{
			"UnityEditor.VFX.DirectionType"
		})]
		[SerializeField]
		protected ExposedProperty m_TargetNormal = "TargetNormal";

		[VFXPropertyBinding(new string[]
		{
			"System.Boolean"
		})]
		[SerializeField]
		protected ExposedProperty m_TargetHit = "TargetHit";

		protected ExposedProperty m_TargetPosition_position;

		protected ExposedProperty m_TargetNormal_direction;

		public GameObject RaycastSource;

		public Vector3 RaycastDirection = Vector3.forward;

		public VFXRaycastBinder.Space RaycastDirectionSpace;

		public LayerMask Layers = -1;

		public float MaxDistance = 100f;

		private RaycastHit m_HitInfo;

		public enum Space
		{
			Local,
			World
		}
	}
}
