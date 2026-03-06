using System;

namespace Meta.WitAi.Composer.Data
{
	public abstract class ReservedContextPath : BaseReservedContextPath
	{
		public string GetValue()
		{
			return this._value;
		}

		public void Set(string value)
		{
			this._value = value;
			this.UpdateContextMap();
		}

		protected internal override void UpdateContextMap()
		{
			if (base.Map == null)
			{
				VLog.W(string.Format("Missing Composer map for {0}", this), null);
			}
			ComposerContextMap map = base.Map;
			if (map == null)
			{
				return;
			}
			map.SetData<string>(this.ReservedPath, this._value);
		}

		public override void Clear()
		{
			this._value = string.Empty;
			base.Clear();
		}

		public override string ToString()
		{
			return this.ReservedPath + " : " + this._value;
		}

		private string _value;
	}
}
