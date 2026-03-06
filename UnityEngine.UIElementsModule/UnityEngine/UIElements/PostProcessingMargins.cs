using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	internal struct PostProcessingMargins
	{
		public float left
		{
			get
			{
				return this.m_Left;
			}
			set
			{
				this.m_Left = value;
			}
		}

		public float top
		{
			get
			{
				return this.m_Top;
			}
			set
			{
				this.m_Top = value;
			}
		}

		public float right
		{
			get
			{
				return this.m_Right;
			}
			set
			{
				this.m_Right = value;
			}
		}

		public float bottom
		{
			get
			{
				return this.m_Bottom;
			}
			set
			{
				this.m_Bottom = value;
			}
		}

		[SerializeField]
		private float m_Left;

		[SerializeField]
		private float m_Top;

		[SerializeField]
		private float m_Right;

		[SerializeField]
		private float m_Bottom;
	}
}
