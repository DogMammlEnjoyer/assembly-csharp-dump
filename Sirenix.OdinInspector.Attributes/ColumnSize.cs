using System;

namespace Sirenix.OdinInspector
{
	public struct ColumnSize
	{
		public static ColumnSize Auto
		{
			get
			{
				return new ColumnSize(ColumnType.Auto, 0f);
			}
		}

		public ColumnSize(ColumnType columnType, float value)
		{
			this.ColumnType = columnType;
			this.Value = value;
		}

		public static ColumnSize Percent(float percentage)
		{
			return new ColumnSize(ColumnType.Percent, percentage);
		}

		public static ColumnSize Pixel(float pixels)
		{
			return new ColumnSize(ColumnType.Pixel, pixels);
		}

		public override string ToString()
		{
			switch (this.ColumnType)
			{
			case ColumnType.Auto:
				return "Auto";
			case ColumnType.Percent:
				return string.Format("{0} %", this.Value * 100f);
			case ColumnType.Pixel:
				return string.Format("{0} px", this.Value);
			default:
				return base.ToString();
			}
		}

		public ColumnType ColumnType;

		public float Value;
	}
}
