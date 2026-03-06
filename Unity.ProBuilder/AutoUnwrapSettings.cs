using System;
using UnityEngine.Serialization;

namespace UnityEngine.ProBuilder
{
	[Serializable]
	public struct AutoUnwrapSettings
	{
		public static AutoUnwrapSettings defaultAutoUnwrapSettings
		{
			get
			{
				AutoUnwrapSettings result = default(AutoUnwrapSettings);
				result.Reset();
				return result;
			}
		}

		public bool useWorldSpace
		{
			get
			{
				return this.m_UseWorldSpace;
			}
			set
			{
				this.m_UseWorldSpace = value;
			}
		}

		public bool flipU
		{
			get
			{
				return this.m_FlipU;
			}
			set
			{
				this.m_FlipU = value;
			}
		}

		public bool flipV
		{
			get
			{
				return this.m_FlipV;
			}
			set
			{
				this.m_FlipV = value;
			}
		}

		public bool swapUV
		{
			get
			{
				return this.m_SwapUV;
			}
			set
			{
				this.m_SwapUV = value;
			}
		}

		public AutoUnwrapSettings.Fill fill
		{
			get
			{
				return this.m_Fill;
			}
			set
			{
				this.m_Fill = value;
			}
		}

		public Vector2 scale
		{
			get
			{
				return this.m_Scale;
			}
			set
			{
				this.m_Scale = value;
			}
		}

		public Vector2 offset
		{
			get
			{
				return this.m_Offset;
			}
			set
			{
				this.m_Offset = value;
			}
		}

		public float rotation
		{
			get
			{
				return this.m_Rotation;
			}
			set
			{
				this.m_Rotation = value;
			}
		}

		public AutoUnwrapSettings.Anchor anchor
		{
			get
			{
				return this.m_Anchor;
			}
			set
			{
				this.m_Anchor = value;
			}
		}

		public AutoUnwrapSettings(AutoUnwrapSettings unwrapSettings)
		{
			this.m_UseWorldSpace = unwrapSettings.m_UseWorldSpace;
			this.m_FlipU = unwrapSettings.m_FlipU;
			this.m_FlipV = unwrapSettings.m_FlipV;
			this.m_SwapUV = unwrapSettings.m_SwapUV;
			this.m_Fill = unwrapSettings.m_Fill;
			this.m_Scale = unwrapSettings.m_Scale;
			this.m_Offset = unwrapSettings.m_Offset;
			this.m_Rotation = unwrapSettings.m_Rotation;
			this.m_Anchor = unwrapSettings.m_Anchor;
		}

		public static AutoUnwrapSettings tile
		{
			get
			{
				AutoUnwrapSettings result = default(AutoUnwrapSettings);
				result.Reset();
				return result;
			}
		}

		public static AutoUnwrapSettings fit
		{
			get
			{
				AutoUnwrapSettings result = default(AutoUnwrapSettings);
				result.Reset();
				result.fill = AutoUnwrapSettings.Fill.Fit;
				return result;
			}
		}

		public static AutoUnwrapSettings stretch
		{
			get
			{
				AutoUnwrapSettings result = default(AutoUnwrapSettings);
				result.Reset();
				result.fill = AutoUnwrapSettings.Fill.Stretch;
				return result;
			}
		}

		public void Reset()
		{
			this.m_UseWorldSpace = false;
			this.m_FlipU = false;
			this.m_FlipV = false;
			this.m_SwapUV = false;
			this.m_Fill = AutoUnwrapSettings.Fill.Tile;
			this.m_Scale = new Vector2(1f, 1f);
			this.m_Offset = new Vector2(0f, 0f);
			this.m_Rotation = 0f;
			this.m_Anchor = AutoUnwrapSettings.Anchor.None;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"Use World Space: ",
				this.useWorldSpace.ToString(),
				"\nFlip U: ",
				this.flipU.ToString(),
				"\nFlip V: ",
				this.flipV.ToString(),
				"\nSwap UV: ",
				this.swapUV.ToString(),
				"\nFill Mode: ",
				this.fill.ToString(),
				"\nAnchor: ",
				this.anchor.ToString(),
				"\nScale: ",
				this.scale.ToString(),
				"\nOffset: ",
				this.offset.ToString(),
				"\nRotation: ",
				this.rotation.ToString()
			});
		}

		[SerializeField]
		[FormerlySerializedAs("useWorldSpace")]
		private bool m_UseWorldSpace;

		[SerializeField]
		[FormerlySerializedAs("flipU")]
		private bool m_FlipU;

		[SerializeField]
		[FormerlySerializedAs("flipV")]
		private bool m_FlipV;

		[SerializeField]
		[FormerlySerializedAs("swapUV")]
		private bool m_SwapUV;

		[SerializeField]
		[FormerlySerializedAs("fill")]
		private AutoUnwrapSettings.Fill m_Fill;

		[SerializeField]
		[FormerlySerializedAs("scale")]
		private Vector2 m_Scale;

		[SerializeField]
		[FormerlySerializedAs("offset")]
		private Vector2 m_Offset;

		[SerializeField]
		[FormerlySerializedAs("rotation")]
		private float m_Rotation;

		[SerializeField]
		[FormerlySerializedAs("anchor")]
		private AutoUnwrapSettings.Anchor m_Anchor;

		public enum Anchor
		{
			UpperLeft,
			UpperCenter,
			UpperRight,
			MiddleLeft,
			MiddleCenter,
			MiddleRight,
			LowerLeft,
			LowerCenter,
			LowerRight,
			None
		}

		public enum Fill
		{
			Fit,
			Tile,
			Stretch
		}
	}
}
