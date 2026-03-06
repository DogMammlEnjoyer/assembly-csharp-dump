using System;
using System.Collections;
using System.Xml;

namespace System.Data.Common
{
	internal sealed class TimeSpanStorage : DataStorage
	{
		public TimeSpanStorage(DataColumn column) : base(column, typeof(TimeSpan), TimeSpanStorage.s_defaultValue, StorageType.TimeSpan)
		{
		}

		public override object Aggregate(int[] records, AggregateType kind)
		{
			bool flag = false;
			try
			{
				switch (kind)
				{
				case AggregateType.Sum:
				{
					decimal num = 0m;
					foreach (int num2 in records)
					{
						if (!this.IsNull(num2))
						{
							num += this._values[num2].Ticks;
							flag = true;
						}
					}
					if (flag)
					{
						return TimeSpan.FromTicks((long)Math.Round(num));
					}
					return null;
				}
				case AggregateType.Mean:
				{
					decimal d = 0m;
					int num3 = 0;
					foreach (int num4 in records)
					{
						if (!this.IsNull(num4))
						{
							d += this._values[num4].Ticks;
							num3++;
						}
					}
					if (num3 > 0)
					{
						return TimeSpan.FromTicks((long)Math.Round(d / num3));
					}
					return null;
				}
				case AggregateType.Min:
				{
					TimeSpan timeSpan = TimeSpan.MaxValue;
					foreach (int num5 in records)
					{
						if (!this.IsNull(num5))
						{
							timeSpan = ((TimeSpan.Compare(this._values[num5], timeSpan) < 0) ? this._values[num5] : timeSpan);
							flag = true;
						}
					}
					if (flag)
					{
						return timeSpan;
					}
					return this._nullValue;
				}
				case AggregateType.Max:
				{
					TimeSpan timeSpan2 = TimeSpan.MinValue;
					foreach (int num6 in records)
					{
						if (!this.IsNull(num6))
						{
							timeSpan2 = ((TimeSpan.Compare(this._values[num6], timeSpan2) >= 0) ? this._values[num6] : timeSpan2);
							flag = true;
						}
					}
					if (flag)
					{
						return timeSpan2;
					}
					return this._nullValue;
				}
				case AggregateType.First:
					if (records.Length != 0)
					{
						return this._values[records[0]];
					}
					return null;
				case AggregateType.Count:
					return base.Aggregate(records, kind);
				case AggregateType.StDev:
				{
					int num7 = 0;
					decimal d2 = 0m;
					foreach (int num8 in records)
					{
						if (!this.IsNull(num8))
						{
							d2 += this._values[num8].Ticks;
							num7++;
						}
					}
					if (num7 > 1)
					{
						double num9 = 0.0;
						decimal d3 = d2 / num7;
						foreach (int num10 in records)
						{
							if (!this.IsNull(num10))
							{
								double num11 = (double)(this._values[num10].Ticks - d3);
								num9 += num11 * num11;
							}
						}
						ulong num12 = (ulong)Math.Round(Math.Sqrt(num9 / (double)(num7 - 1)));
						if (num12 > 9223372036854775807UL)
						{
							num12 = 9223372036854775807UL;
						}
						return TimeSpan.FromTicks((long)num12);
					}
					return null;
				}
				}
			}
			catch (OverflowException)
			{
				throw ExprException.Overflow(typeof(TimeSpan));
			}
			throw ExceptionBuilder.AggregateException(kind, this._dataType);
		}

		public override int Compare(int recordNo1, int recordNo2)
		{
			TimeSpan t = this._values[recordNo1];
			TimeSpan timeSpan = this._values[recordNo2];
			if (t == TimeSpanStorage.s_defaultValue || timeSpan == TimeSpanStorage.s_defaultValue)
			{
				int num = base.CompareBits(recordNo1, recordNo2);
				if (num != 0)
				{
					return num;
				}
			}
			return TimeSpan.Compare(t, timeSpan);
		}

		public override int CompareValueTo(int recordNo, object value)
		{
			if (this._nullValue == value)
			{
				if (this.IsNull(recordNo))
				{
					return 0;
				}
				return 1;
			}
			else
			{
				TimeSpan t = this._values[recordNo];
				if (TimeSpanStorage.s_defaultValue == t && this.IsNull(recordNo))
				{
					return -1;
				}
				return t.CompareTo((TimeSpan)value);
			}
		}

		private static TimeSpan ConvertToTimeSpan(object value)
		{
			Type type = value.GetType();
			if (type == typeof(string))
			{
				return TimeSpan.Parse((string)value);
			}
			if (type == typeof(int))
			{
				return new TimeSpan((long)((int)value));
			}
			if (type == typeof(long))
			{
				return new TimeSpan((long)value);
			}
			return (TimeSpan)value;
		}

		public override object ConvertValue(object value)
		{
			if (this._nullValue != value)
			{
				if (value != null)
				{
					value = TimeSpanStorage.ConvertToTimeSpan(value);
				}
				else
				{
					value = this._nullValue;
				}
			}
			return value;
		}

		public override void Copy(int recordNo1, int recordNo2)
		{
			base.CopyBits(recordNo1, recordNo2);
			this._values[recordNo2] = this._values[recordNo1];
		}

		public override object Get(int record)
		{
			TimeSpan timeSpan = this._values[record];
			if (timeSpan != TimeSpanStorage.s_defaultValue)
			{
				return timeSpan;
			}
			return base.GetBits(record);
		}

		public override void Set(int record, object value)
		{
			if (this._nullValue == value)
			{
				this._values[record] = TimeSpanStorage.s_defaultValue;
				base.SetNullBit(record, true);
				return;
			}
			this._values[record] = TimeSpanStorage.ConvertToTimeSpan(value);
			base.SetNullBit(record, false);
		}

		public override void SetCapacity(int capacity)
		{
			TimeSpan[] array = new TimeSpan[capacity];
			if (this._values != null)
			{
				Array.Copy(this._values, 0, array, 0, Math.Min(capacity, this._values.Length));
			}
			this._values = array;
			base.SetCapacity(capacity);
		}

		public override object ConvertXmlToObject(string s)
		{
			return XmlConvert.ToTimeSpan(s);
		}

		public override string ConvertObjectToXml(object value)
		{
			return XmlConvert.ToString((TimeSpan)value);
		}

		protected override object GetEmptyStorage(int recordCount)
		{
			return new TimeSpan[recordCount];
		}

		protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
		{
			((TimeSpan[])store)[storeIndex] = this._values[record];
			nullbits.Set(storeIndex, this.IsNull(record));
		}

		protected override void SetStorage(object store, BitArray nullbits)
		{
			this._values = (TimeSpan[])store;
			base.SetNullStorage(nullbits);
		}

		private static readonly TimeSpan s_defaultValue = TimeSpan.Zero;

		private TimeSpan[] _values;
	}
}
