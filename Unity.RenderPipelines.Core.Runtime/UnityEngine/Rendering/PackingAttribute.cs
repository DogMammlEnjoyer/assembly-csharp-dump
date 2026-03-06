using System;

namespace UnityEngine.Rendering
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class PackingAttribute : Attribute
	{
		public PackingAttribute(string[] displayNames, FieldPacking packingScheme = FieldPacking.NoPacking, int bitSize = 32, int offsetInSource = 0, float minValue = 0f, float maxValue = 1f, bool isDirection = false, bool sRGBDisplay = false, bool checkIsNormalized = false, string preprocessor = "")
		{
			this.displayNames = displayNames;
			this.packingScheme = packingScheme;
			this.offsetInSource = offsetInSource;
			this.isDirection = isDirection;
			this.sRGBDisplay = sRGBDisplay;
			this.checkIsNormalized = checkIsNormalized;
			this.sizeInBits = bitSize;
			this.range = new float[]
			{
				minValue,
				maxValue
			};
			this.preprocessor = preprocessor;
		}

		public PackingAttribute(string displayName = "", FieldPacking packingScheme = FieldPacking.NoPacking, int bitSize = 0, int offsetInSource = 0, float minValue = 0f, float maxValue = 1f, bool isDirection = false, bool sRGBDisplay = false, bool checkIsNormalized = false, string preprocessor = "")
		{
			this.displayNames = new string[1];
			this.displayNames[0] = displayName;
			this.packingScheme = packingScheme;
			this.offsetInSource = offsetInSource;
			this.isDirection = isDirection;
			this.sRGBDisplay = sRGBDisplay;
			this.checkIsNormalized = checkIsNormalized;
			this.sizeInBits = bitSize;
			this.range = new float[]
			{
				minValue,
				maxValue
			};
			this.preprocessor = preprocessor;
		}

		public string[] displayNames;

		public float[] range;

		public FieldPacking packingScheme;

		public int offsetInSource;

		public int sizeInBits;

		public bool isDirection;

		public bool sRGBDisplay;

		public bool checkIsNormalized;

		public string preprocessor;
	}
}
