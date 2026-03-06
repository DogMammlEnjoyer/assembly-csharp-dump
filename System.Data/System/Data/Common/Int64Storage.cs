using System;
using System.Collections;
using System.Xml;

namespace System.Data.Common
{
	internal sealed class Int64Storage : DataStorage
	{
		internal Int64Storage(DataColumn column) : base(column, typeof(long), 0L, StorageType.Int64)
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
					long num = 0L;
					checked
					{
						foreach (int num2 in records)
						{
							if (base.HasValue(num2))
							{
								num += this._values[num2];
								flag = true;
							}
						}
						if (flag)
						{
							return num;
						}
						return this._nullValue;
					}
				}
				case AggregateType.Mean:
				{
					decimal d = 0m;
					int num3 = 0;
					foreach (int num4 in records)
					{
						if (base.HasValue(num4))
						{
							d += this._values[num4];
							num3++;
							flag = true;
						}
					}
					if (flag)
					{
						return (long)(d / num3);
					}
					return this._nullValue;
				}
				case AggregateType.Min:
				{
					long num5 = long.MaxValue;
					foreach (int num6 in records)
					{
						if (base.HasValue(num6))
						{
							num5 = Math.Min(this._values[num6], num5);
							flag = true;
						}
					}
					if (flag)
					{
						return num5;
					}
					return this._nullValue;
				}
				case AggregateType.Max:
				{
					long num7 = long.MinValue;
					foreach (int num8 in records)
					{
						if (base.HasValue(num8))
						{
							num7 = Math.Max(this._values[num8], num7);
							flag = true;
						}
					}
					if (flag)
					{
						return num7;
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
				case AggregateType.Var:
				case AggregateType.StDev:
				{
					int num9 = 0;
					double num10 = 0.0;
					double num11 = 0.0;
					foreach (int num12 in records)
					{
						if (base.HasValue(num12))
						{
							num10 += (double)this._values[num12];
							num11 += (double)this._values[num12] * (double)this._values[num12];
							num9++;
						}
					}
					if (num9 <= 1)
					{
						return this._nullValue;
					}
					double num13 = (double)num9 * num11 - num10 * num10;
					if (num13 / (num10 * num10) < 1E-15 || num13 < 0.0)
					{
						num13 = 0.0;
					}
					else
					{
						num13 /= (double)(num9 * (num9 - 1));
					}
					if (kind == AggregateType.StDev)
					{
						return Math.Sqrt(num13);
					}
					return num13;
				}
				}
			}
			catch (OverflowException)
			{
				throw ExprException.Overflow(typeof(long));
			}
			throw ExceptionBuilder.AggregateException(kind, this._dataType);
		}

		public override int Compare(int recordNo1, int recordNo2)
		{
			long num = this._values[recordNo1];
			long num2 = this._values[recordNo2];
			if (num == 0L || num2 == 0L)
			{
				int num3 = base.CompareBits(recordNo1, recordNo2);
				if (num3 != 0)
				{
					return num3;
				}
			}
			if (num < num2)
			{
				return -1;
			}
			if (num <= num2)
			{
				return 0;
			}
			return 1;
		}

		public override int CompareValueTo(int recordNo, object value)
		{
			if (this._nullValue == value)
			{
				if (!base.HasValue(recordNo))
				{
					return 0;
				}
				return 1;
			}
			else
			{
				long num = this._values[recordNo];
				if (num == 0L && !base.HasValue(recordNo))
				{
					return -1;
				}
				return num.CompareTo((long)value);
			}
		}

		public override object ConvertValue(object value)
		{
			if (this._nullValue != value)
			{
				if (value != null)
				{
					value = ((IConvertible)value).ToInt64(base.FormatProvider);
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
			long num = this._values[record];
			if (num != 0L)
			{
				return num;
			}
			return base.GetBits(record);
		}

		public override void Set(int record, object value)
		{
			if (this._nullValue == value)
			{
				this._values[record] = 0L;
				base.SetNullBit(record, true);
				return;
			}
			this._values[record] = ((IConvertible)value).ToInt64(base.FormatProvider);
			base.SetNullBit(record, false);
		}

		public override void SetCapacity(int capacity)
		{
			long[] array = new long[capacity];
			if (this._values != null)
			{
				Array.Copy(this._values, 0, array, 0, Math.Min(capacity, this._values.Length));
			}
			this._values = array;
			base.SetCapacity(capacity);
		}

		public override object ConvertXmlToObject(string s)
		{
			return XmlConvert.ToInt64(s);
		}

		public override string ConvertObjectToXml(object value)
		{
			return XmlConvert.ToString((long)value);
		}

		protected override object GetEmptyStorage(int recordCount)
		{
			return new long[recordCount];
		}

		protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
		{
			((long[])store)[storeIndex] = this._values[record];
			nullbits.Set(storeIndex, !base.HasValue(record));
		}

		protected override void SetStorage(object store, BitArray nullbits)
		{
			this._values = (long[])store;
			base.SetNullStorage(nullbits);
		}

		private const long defaultValue = 0L;

		private long[] _values;
	}
}
