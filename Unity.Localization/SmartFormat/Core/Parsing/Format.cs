using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Pool;

namespace UnityEngine.Localization.SmartFormat.Core.Parsing
{
	public class Format : FormatItem
	{
		public void ReleaseToPool()
		{
			this.Clear();
			foreach (FormatItem formatItem in this.Items)
			{
				if (this == formatItem.Parent)
				{
					FormatItemPool.Release(formatItem);
				}
			}
			foreach (Format.SplitList toRelease in this.m_Splits)
			{
				SplitListPool.Release(toRelease);
			}
			this.parent = null;
			this.Items.Clear();
			this.HasNested = false;
			this.splitCache = null;
			this.m_Splits.Clear();
		}

		public List<FormatItem> Items { get; } = new List<FormatItem>();

		public bool HasNested { get; set; }

		public Format Substring(int startIndex)
		{
			return this.Substring(startIndex, this.endIndex - this.startIndex - startIndex);
		}

		public Format Substring(int startIndex, int length)
		{
			startIndex = this.startIndex + startIndex;
			int num = startIndex + length;
			if (startIndex < this.startIndex || startIndex > this.endIndex)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (num > this.endIndex)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			if (startIndex == this.startIndex && num == this.endIndex)
			{
				return this;
			}
			Format format = FormatItemPool.GetFormat(this.SmartSettings, this.baseString, startIndex, num);
			foreach (FormatItem formatItem in this.Items)
			{
				if (formatItem.endIndex > startIndex)
				{
					if (num <= formatItem.startIndex)
					{
						break;
					}
					FormatItem item = formatItem;
					if (formatItem is LiteralText)
					{
						if (startIndex > formatItem.startIndex || formatItem.endIndex > num)
						{
							item = FormatItemPool.GetLiteralText(this.SmartSettings, format, Math.Max(startIndex, formatItem.startIndex), Math.Min(num, formatItem.endIndex));
						}
					}
					else
					{
						format.HasNested = true;
					}
					format.Items.Add(item);
				}
			}
			return format;
		}

		public int IndexOf(char search)
		{
			return this.IndexOf(search, 0);
		}

		public int IndexOf(char search, int startIndex)
		{
			startIndex = this.startIndex + startIndex;
			foreach (FormatItem formatItem in this.Items)
			{
				if (formatItem.endIndex >= startIndex)
				{
					LiteralText literalText = formatItem as LiteralText;
					if (literalText != null)
					{
						if (startIndex < literalText.startIndex)
						{
							startIndex = literalText.startIndex;
						}
						int num = literalText.baseString.IndexOf(search, startIndex, literalText.endIndex - startIndex);
						if (num != -1)
						{
							return num - this.startIndex;
						}
					}
				}
			}
			return -1;
		}

		private List<int> FindAll(char search)
		{
			return this.FindAll(search, -1);
		}

		private List<int> FindAll(char search, int maxCount)
		{
			List<int> list = CollectionPool<List<int>, int>.Get();
			int num = 0;
			while (maxCount != 0)
			{
				num = this.IndexOf(search, num);
				if (num == -1)
				{
					break;
				}
				list.Add(num);
				num++;
				maxCount--;
			}
			return list;
		}

		public IList<Format> Split(char search)
		{
			if (this.splitCache == null || this.splitCacheChar != search)
			{
				this.splitCacheChar = search;
				this.splitCache = this.Split(search, -1);
			}
			return this.splitCache;
		}

		public IList<Format> Split(char search, int maxCount)
		{
			List<int> splits = this.FindAll(search, maxCount);
			Format.SplitList splitList = SplitListPool.Get(this, splits);
			this.m_Splits.Add(splitList);
			return splitList;
		}

		public string GetLiteralText()
		{
			StringBuilder stringBuilder;
			string result;
			using (StringBuilderPool.Get(out stringBuilder))
			{
				foreach (FormatItem formatItem in this.Items)
				{
					LiteralText literalText = formatItem as LiteralText;
					if (literalText != null)
					{
						stringBuilder.Append(literalText);
					}
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder;
			string result;
			using (StringBuilderPool.Get(out stringBuilder))
			{
				int num = this.endIndex - this.startIndex;
				if (stringBuilder.Capacity < num)
				{
					stringBuilder.Capacity = num;
				}
				foreach (FormatItem value in this.Items)
				{
					stringBuilder.Append(value);
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		public Placeholder parent;

		private List<Format.SplitList> m_Splits = new List<Format.SplitList>();

		private char splitCacheChar;

		private IList<Format> splitCache;

		internal class SplitList : IList<Format>, ICollection<Format>, IEnumerable<Format>, IEnumerable
		{
			public void Init(Format format, List<int> splits)
			{
				this.m_Format = format;
				this.m_Splits = splits;
				for (int i = 0; i < this.Count; i++)
				{
					this.m_FormatCache.Add(null);
				}
			}

			public Format this[int index]
			{
				get
				{
					if (index > this.m_Splits.Count)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					if (this.m_Splits.Count == 0)
					{
						return this.m_Format;
					}
					if (this.m_FormatCache[index] != null)
					{
						return this.m_FormatCache[index];
					}
					if (index == 0)
					{
						Format format = this.m_Format.Substring(0, this.m_Splits[0]);
						this.m_FormatCache[index] = format;
						return format;
					}
					if (index == this.m_Splits.Count)
					{
						Format format2 = this.m_Format.Substring(this.m_Splits[index - 1] + 1);
						this.m_FormatCache[index] = format2;
						return format2;
					}
					int num = this.m_Splits[index - 1] + 1;
					Format format3 = this.m_Format.Substring(num, this.m_Splits[index] - num);
					this.m_FormatCache[index] = format3;
					return format3;
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public void CopyTo(Format[] array, int arrayIndex)
			{
				int num = this.m_Splits.Count + 1;
				for (int i = 0; i < num; i++)
				{
					array[arrayIndex + i] = this[i];
				}
			}

			public int Count
			{
				get
				{
					return this.m_Splits.Count + 1;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public int IndexOf(Format item)
			{
				throw new NotSupportedException();
			}

			public void Insert(int index, Format item)
			{
				throw new NotSupportedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			public void Add(Format item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				this.m_Format = null;
				CollectionPool<List<int>, int>.Release(this.m_Splits);
				this.m_Splits = null;
				for (int i = 0; i < this.m_FormatCache.Count; i++)
				{
					if (this.m_FormatCache[i] != null)
					{
						FormatItemPool.ReleaseFormat(this.m_FormatCache[i]);
					}
				}
				this.m_FormatCache.Clear();
			}

			public bool Contains(Format item)
			{
				throw new NotSupportedException();
			}

			public bool Remove(Format item)
			{
				throw new NotSupportedException();
			}

			public IEnumerator<Format> GetEnumerator()
			{
				throw new NotSupportedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotSupportedException();
			}

			private Format m_Format;

			private List<int> m_Splits;

			private List<Format> m_FormatCache = new List<Format>();
		}
	}
}
