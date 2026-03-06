using System;

namespace UnityEngine.UIElements.Layout
{
	internal struct LayoutDefaults
	{
		// Note: this type is marked as 'beforefieldinit'.
		unsafe static LayoutDefaults()
		{
			FixedBuffer9<LayoutValue> edgeValuesUnit = default(FixedBuffer9<LayoutValue>);
			*edgeValuesUnit[0] = LayoutValue.Undefined();
			*edgeValuesUnit[1] = LayoutValue.Undefined();
			*edgeValuesUnit[2] = LayoutValue.Undefined();
			*edgeValuesUnit[3] = LayoutValue.Undefined();
			*edgeValuesUnit[4] = LayoutValue.Undefined();
			*edgeValuesUnit[5] = LayoutValue.Undefined();
			*edgeValuesUnit[6] = LayoutValue.Undefined();
			*edgeValuesUnit[7] = LayoutValue.Undefined();
			*edgeValuesUnit[8] = LayoutValue.Undefined();
			LayoutDefaults.EdgeValuesUnit = edgeValuesUnit;
			LayoutDefaults.DimensionValues = new float[]
			{
				float.NaN,
				float.NaN
			};
			FixedBuffer2<LayoutValue> fixedBuffer = default(FixedBuffer2<LayoutValue>);
			*fixedBuffer[0] = LayoutValue.Undefined();
			*fixedBuffer[1] = LayoutValue.Undefined();
			LayoutDefaults.DimensionValuesUnit = fixedBuffer;
			fixedBuffer = default(FixedBuffer2<LayoutValue>);
			*fixedBuffer[0] = LayoutValue.Auto();
			*fixedBuffer[1] = LayoutValue.Auto();
			LayoutDefaults.DimensionValuesAutoUnit = fixedBuffer;
		}

		public static readonly FixedBuffer9<LayoutValue> EdgeValuesUnit;

		public static readonly float[] DimensionValues;

		public static readonly FixedBuffer2<LayoutValue> DimensionValuesUnit;

		public static readonly FixedBuffer2<LayoutValue> DimensionValuesAutoUnit;
	}
}
