using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEngine.TextCoreTextEngineModule"
	})]
	[UsedByNativeCode]
	[Serializable]
	internal struct GlyphAnchorPoint
	{
		public float xCoordinate
		{
			get
			{
				return this.m_XCoordinate;
			}
			set
			{
				this.m_XCoordinate = value;
			}
		}

		public float yCoordinate
		{
			get
			{
				return this.m_YCoordinate;
			}
			set
			{
				this.m_YCoordinate = value;
			}
		}

		[SerializeField]
		[NativeName("xPositionAdjustment")]
		private float m_XCoordinate;

		[SerializeField]
		[NativeName("yPositionAdjustment")]
		private float m_YCoordinate;
	}
}
