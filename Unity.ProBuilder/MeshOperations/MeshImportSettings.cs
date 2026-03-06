using System;

namespace UnityEngine.ProBuilder.MeshOperations
{
	[Serializable]
	public sealed class MeshImportSettings
	{
		public bool quads
		{
			get
			{
				return this.m_Quads;
			}
			set
			{
				this.m_Quads = value;
			}
		}

		public bool smoothing
		{
			get
			{
				return this.m_Smoothing;
			}
			set
			{
				this.m_Smoothing = value;
			}
		}

		public float smoothingAngle
		{
			get
			{
				return this.m_SmoothingThreshold;
			}
			set
			{
				this.m_SmoothingThreshold = value;
			}
		}

		public override string ToString()
		{
			return string.Format("quads: {0}\nsmoothing: {1}\nthreshold: {2}", this.quads, this.smoothing, this.smoothingAngle);
		}

		[SerializeField]
		private bool m_Quads = true;

		[SerializeField]
		private bool m_Smoothing = true;

		[SerializeField]
		private float m_SmoothingThreshold = 1f;
	}
}
