using System;
using System.Reflection;

namespace Meta.WitAi.Json
{
	internal class JsonFieldInfo : BaseJsonVariableInfo<FieldInfo>
	{
		public JsonFieldInfo(FieldInfo info) : base(info)
		{
		}

		public override Type GetVariableType()
		{
			return this._info.FieldType;
		}

		protected override bool HasGet()
		{
			return true;
		}

		protected override bool IsGetPublic()
		{
			return this._info.IsPublic;
		}

		public override object GetValue(object obj)
		{
			return this._info.GetValue(obj);
		}

		protected override bool HasSet()
		{
			return true;
		}

		protected override bool IsSetPublic()
		{
			return this._info.IsPublic;
		}

		public override void SetValue(object obj, object value)
		{
			this._info.SetValue(obj, value);
		}
	}
}
