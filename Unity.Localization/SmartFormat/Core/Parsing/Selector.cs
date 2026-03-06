using System;

namespace UnityEngine.Localization.SmartFormat.Core.Parsing
{
	public class Selector : FormatItem
	{
		public override void Clear()
		{
			base.Clear();
			this.m_Operator = null;
		}

		public int SelectorIndex { get; internal set; }

		public string Operator
		{
			get
			{
				if (this.m_Operator == null)
				{
					this.m_Operator = this.baseString.Substring(this.operatorStart, this.startIndex - this.operatorStart);
				}
				return this.m_Operator;
			}
		}

		private string m_Operator;

		internal int operatorStart;
	}
}
