using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meta.WitAi.Composer.Data
{
	public abstract class ReservedContextPathDictionary : BaseReservedContextPath
	{
		public Dictionary<string, string> GetDictionary()
		{
			return this._runtimeDynamicContext;
		}

		public string this[string key]
		{
			get
			{
				if (!this._runtimeDynamicContext.ContainsKey(key))
				{
					return string.Empty;
				}
				return this._runtimeDynamicContext[key];
			}
			set
			{
				this.Set(key, value);
			}
		}

		public bool Add(string key, string context = null)
		{
			if (this._runtimeDynamicContext.ContainsKey(key))
			{
				return false;
			}
			this.Set(key, context);
			return true;
		}

		public void Set(string key, string context = null)
		{
			if (context == null)
			{
				context = key;
			}
			this._runtimeDynamicContext[key] = context;
			this.UpdateContextMap();
		}

		public void Remove(string key)
		{
			this._runtimeDynamicContext.Remove(key);
			string.Join(this.Separator, this._runtimeDynamicContext.Values);
		}

		protected internal override void UpdateContextMap()
		{
			string newValue = string.Join(this.Separator, this._runtimeDynamicContext.Values);
			base.Map.SetData<string>(this.ReservedPath, newValue);
			string.Join(this.Separator, this._runtimeDynamicContext.Values);
		}

		public override void Clear()
		{
			this._runtimeDynamicContext.Clear();
			base.Clear();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Reserved Path: " + this.ReservedPath);
			for (int i = 0; i < this._runtimeDynamicContext.Keys.Count; i++)
			{
				string text = this._runtimeDynamicContext.Keys.ElementAt(i);
				stringBuilder.Append(string.Format("\n\t[{0}] {1} : {2}", i, text, this._runtimeDynamicContext[text]));
			}
			return stringBuilder.ToString();
		}

		private readonly Dictionary<string, string> _runtimeDynamicContext = new Dictionary<string, string>();

		private ComposerService _composer;

		protected readonly string Separator = "\n";
	}
}
