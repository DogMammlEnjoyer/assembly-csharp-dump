using System;
using System.Collections.Generic;
using System.Text;

namespace UnityEngine.Localization.SmartFormat.Core.Parsing
{
	public class Placeholder : FormatItem
	{
		public void ReleaseToPool()
		{
			this.Clear();
			if (this.Format != null)
			{
				FormatItemPool.ReleaseFormat(this.Format);
			}
			this.Format = null;
			this.NestedDepth = 0;
			this.Alignment = 0;
			foreach (Selector selector in this.Selectors)
			{
				FormatItemPool.ReleaseSelector(selector);
			}
			this.Selectors.Clear();
		}

		public int NestedDepth { get; set; }

		public List<Selector> Selectors { get; } = new List<Selector>();

		public int Alignment { get; set; }

		public string FormatterName { get; set; }

		public string FormatterOptions { get; set; }

		public Format Format { get; set; }

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
				stringBuilder.Append('{');
				foreach (Selector selector in this.Selectors)
				{
					stringBuilder.Append(selector.baseString, selector.operatorStart, selector.endIndex - selector.operatorStart);
				}
				if (this.Alignment != 0)
				{
					stringBuilder.Append(',');
					stringBuilder.Append(this.Alignment);
				}
				if (this.FormatterName != "")
				{
					stringBuilder.Append(':');
					stringBuilder.Append(this.FormatterName);
					if (this.FormatterOptions != "")
					{
						stringBuilder.Append('(');
						stringBuilder.Append(this.FormatterOptions);
						stringBuilder.Append(')');
					}
				}
				if (this.Format != null)
				{
					stringBuilder.Append(':');
					stringBuilder.Append(this.Format);
				}
				stringBuilder.Append('}');
				result = stringBuilder.ToString();
			}
			return result;
		}
	}
}
