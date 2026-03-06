using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public class RigEffectorData
	{
		public Transform transform
		{
			get
			{
				return this.m_Transform;
			}
		}

		public RigEffectorData.Style style
		{
			get
			{
				return this.m_Style;
			}
		}

		public bool visible
		{
			get
			{
				return this.m_Visible;
			}
			set
			{
				this.m_Visible = value;
			}
		}

		public void Initialize(Transform transform, RigEffectorData.Style style)
		{
			this.m_Transform = transform;
			this.m_Style = style;
		}

		[SerializeField]
		private Transform m_Transform;

		[SerializeField]
		private RigEffectorData.Style m_Style;

		[SerializeField]
		private bool m_Visible = true;

		[Serializable]
		public struct Style
		{
			public Mesh shape;

			public Color color;

			public float size;

			public Vector3 position;

			public Vector3 rotation;
		}
	}
}
