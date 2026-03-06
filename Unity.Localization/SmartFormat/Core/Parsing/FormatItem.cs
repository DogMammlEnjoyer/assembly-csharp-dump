using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization.SmartFormat.Core.Settings;

namespace UnityEngine.Localization.SmartFormat.Core.Parsing
{
	public abstract class FormatItem
	{
		public FormatItem Parent { get; private set; }

		public void Init(SmartSettings smartSettings, FormatItem parent, int startIndex)
		{
			this.Init(smartSettings, parent, parent.baseString, startIndex, parent.baseString.Length);
		}

		public void Init(SmartSettings smartSettings, FormatItem parent, int startIndex, int endIndex)
		{
			this.Init(smartSettings, parent, parent.baseString, startIndex, endIndex);
		}

		public void Init(SmartSettings smartSettings, FormatItem parent, string baseString, int startIndex, int endIndex)
		{
			this.Parent = parent;
			this.SmartSettings = smartSettings;
			this.baseString = baseString;
			this.startIndex = startIndex;
			this.endIndex = endIndex;
		}

		public virtual void Clear()
		{
			this.baseString = null;
			this.endIndex = 0;
			this.startIndex = 0;
			this.SmartSettings = null;
			this.m_RawText = null;
			this.Parent = null;
		}

		public string RawText
		{
			get
			{
				if (this.m_RawText == null)
				{
					this.m_RawText = this.baseString.Substring(this.startIndex, this.endIndex - this.startIndex);
				}
				return this.m_RawText;
			}
		}

		public IEnumerable<char> ToEnumerable()
		{
			return new FormatItem.PartialCharEnumerator(this.baseString, this.startIndex, this.endIndex);
		}

		public override string ToString()
		{
			string result;
			if (this.endIndex > this.startIndex)
			{
				if ((result = this.RawText) == null)
				{
					return "";
				}
			}
			else
			{
				result = "Empty (" + this.baseString.Substring(this.startIndex) + ")";
			}
			return result;
		}

		public string baseString;

		public int endIndex;

		protected SmartSettings SmartSettings;

		public int startIndex;

		protected string m_RawText;

		private struct PartialCharEnumerator : IEnumerable<char>, IEnumerable
		{
			public PartialCharEnumerator(string s, int from, int to)
			{
				this.m_BaseString = s;
				this.m_From = from;
				this.m_To = to;
			}

			public IEnumerator<char> GetEnumerator()
			{
				int num;
				for (int i = this.m_From; i < this.m_To; i = num)
				{
					yield return this.m_BaseString[i];
					num = i + 1;
				}
				yield break;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private string m_BaseString;

			private int m_From;

			private int m_To;
		}
	}
}
