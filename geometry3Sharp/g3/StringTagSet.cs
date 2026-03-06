using System;
using System.Collections.Generic;

namespace g3
{
	public class StringTagSet<T>
	{
		private void create()
		{
			if (this.tags == null)
			{
				this.tags = new Dictionary<T, string>();
			}
		}

		public void Add(T reference, string tag)
		{
			this.create();
			this.tags.Add(reference, tag);
		}

		public bool Has(T reference)
		{
			string text = "";
			return this.tags != null && this.tags.TryGetValue(reference, out text);
		}

		public string Get(T reference)
		{
			string result = "";
			if (this.tags != null && this.tags.TryGetValue(reference, out result))
			{
				return result;
			}
			return "";
		}

		public const string InvalidTag = "";

		private Dictionary<T, string> tags;
	}
}
