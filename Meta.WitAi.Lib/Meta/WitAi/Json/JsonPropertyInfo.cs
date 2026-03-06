using System;
using System.Reflection;

namespace Meta.WitAi.Json
{
	internal class JsonPropertyInfo : BaseJsonVariableInfo<PropertyInfo>
	{
		public JsonPropertyInfo(PropertyInfo info) : base(info)
		{
		}

		public override Type GetVariableType()
		{
			return this._info.PropertyType;
		}

		protected override bool HasGet()
		{
			return this._info.GetMethod != null;
		}

		protected override bool IsGetPublic()
		{
			return this._info.GetMethod.IsPublic;
		}

		public override object GetValue(object obj)
		{
			return this._info.GetValue(obj);
		}

		protected override bool HasSet()
		{
			return this._info.SetMethod != null;
		}

		protected override bool IsSetPublic()
		{
			return this._info.SetMethod.IsPublic;
		}

		public override void SetValue(object obj, object value)
		{
			this._info.SetValue(obj, value);
		}
	}
}
