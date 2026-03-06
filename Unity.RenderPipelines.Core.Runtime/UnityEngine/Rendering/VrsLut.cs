using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering
{
	[Serializable]
	public class VrsLut
	{
		public static VrsLut CreateDefault()
		{
			VrsLut vrsLut = new VrsLut();
			vrsLut[ShadingRateFragmentSize.FragmentSize1x1] = Color.red;
			vrsLut[ShadingRateFragmentSize.FragmentSize1x2] = Color.yellow;
			vrsLut[ShadingRateFragmentSize.FragmentSize2x1] = Color.white;
			vrsLut[ShadingRateFragmentSize.FragmentSize2x2] = Color.green;
			vrsLut[ShadingRateFragmentSize.FragmentSize1x4] = new Color(0.75f, 0.75f, 0f, 1f);
			vrsLut[ShadingRateFragmentSize.FragmentSize4x1] = new Color(0f, 0.75f, 0.55f, 1f);
			vrsLut[ShadingRateFragmentSize.FragmentSize2x4] = new Color(0.5f, 0f, 0.5f, 1f);
			vrsLut[ShadingRateFragmentSize.FragmentSize4x2] = Color.grey;
			vrsLut[ShadingRateFragmentSize.FragmentSize4x4] = Color.blue;
			return vrsLut;
		}

		public Color this[ShadingRateFragmentSize fragmentSize]
		{
			get
			{
				return this.m_Data[(int)fragmentSize];
			}
			set
			{
				this.m_Data[(int)fragmentSize] = value;
			}
		}

		public GraphicsBuffer CreateBuffer(bool forVisualization = false)
		{
			Color[] array;
			if (forVisualization)
			{
				Array values = Enum.GetValues(typeof(ShadingRateFragmentSize));
				array = new Color[this.MapFragmentShadingRateToBinary(ShadingRateFragmentSize.FragmentSize4x4) + 1U];
				for (int i = values.Length - 1; i >= 0; i--)
				{
					ShadingRateFragmentSize shadingRateFragmentSize = (ShadingRateFragmentSize)values.GetValue(i);
					byte b = ShadingRateInfo.QueryNativeValue(shadingRateFragmentSize);
					array[(int)b] = this.m_Data[(int)shadingRateFragmentSize].linear;
				}
			}
			else
			{
				array = new Color[this.m_Data.Length];
				for (int j = 0; j < this.m_Data.Length; j++)
				{
					array[j] = this.m_Data[j].linear;
				}
			}
			GraphicsBuffer graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, array.Length, Marshal.SizeOf(typeof(Color)));
			graphicsBuffer.SetData(array);
			return graphicsBuffer;
		}

		private uint MapFragmentShadingRateToBinary(ShadingRateFragmentSize fs)
		{
			switch (fs)
			{
			default:
				return this.EncodeShadingRate(0U, 0U);
			case ShadingRateFragmentSize.FragmentSize1x2:
				return this.EncodeShadingRate(0U, 1U);
			case ShadingRateFragmentSize.FragmentSize2x1:
				return this.EncodeShadingRate(1U, 0U);
			case ShadingRateFragmentSize.FragmentSize2x2:
				return this.EncodeShadingRate(1U, 1U);
			case ShadingRateFragmentSize.FragmentSize1x4:
				return this.EncodeShadingRate(0U, 2U);
			case ShadingRateFragmentSize.FragmentSize4x1:
				return this.EncodeShadingRate(2U, 0U);
			case ShadingRateFragmentSize.FragmentSize2x4:
				return this.EncodeShadingRate(1U, 2U);
			case ShadingRateFragmentSize.FragmentSize4x2:
				return this.EncodeShadingRate(2U, 1U);
			case ShadingRateFragmentSize.FragmentSize4x4:
				return this.EncodeShadingRate(2U, 2U);
			}
		}

		private uint EncodeShadingRate(uint x, uint y)
		{
			return x << 2 | y;
		}

		[SerializeField]
		private Color[] m_Data = new Color[Vrs.shadingRateFragmentSizeCount];

		private const uint Rate1x = 0U;

		private const uint Rate2x = 1U;

		private const uint Rate4x = 2U;
	}
}
