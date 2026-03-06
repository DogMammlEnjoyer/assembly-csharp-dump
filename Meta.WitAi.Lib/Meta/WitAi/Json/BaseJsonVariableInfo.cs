using System;
using System.Collections.Generic;
using System.Reflection;

namespace Meta.WitAi.Json
{
	internal abstract class BaseJsonVariableInfo<T> : IJsonVariableInfo where T : MemberInfo
	{
		protected BaseJsonVariableInfo(T info)
		{
			this._info = info;
		}

		protected virtual string GetName()
		{
			return this._info.Name;
		}

		protected virtual bool IsDefined<TAttribute>() where TAttribute : Attribute
		{
			return this._info.IsDefined(typeof(TAttribute), false);
		}

		protected virtual IEnumerable<TAttribute> GetCustomAttributes<TAttribute>() where TAttribute : Attribute
		{
			return this._info.GetCustomAttributes(false);
		}

		public virtual string[] GetSerializeNames()
		{
			if (!this.IsDefined<JsonPropertyAttribute>())
			{
				return new string[]
				{
					this.GetName()
				};
			}
			List<string> list = new List<string>();
			foreach (JsonPropertyAttribute jsonPropertyAttribute in this.GetCustomAttributes<JsonPropertyAttribute>())
			{
				string text = jsonPropertyAttribute.PropertyName;
				if (string.IsNullOrEmpty(text))
				{
					text = this.GetName();
				}
				if (!list.Contains(text))
				{
					list.Add(text);
				}
			}
			return list.ToArray();
		}

		public virtual bool GetShouldSerialize()
		{
			return !this.IsDefined<JsonIgnoreAttribute>() && !this.IsDefined<NonSerializedAttribute>() && this.HasGet() && (this.IsGetPublic() || this.IsDefined<JsonPropertyAttribute>());
		}

		protected abstract bool HasGet();

		protected abstract bool IsGetPublic();

		public virtual bool GetShouldDeserialize()
		{
			return !this.IsDefined<JsonIgnoreAttribute>() && !this.IsDefined<NonSerializedAttribute>() && this.HasSet() && (this.IsSetPublic() || this.IsDefined<JsonPropertyAttribute>());
		}

		protected abstract bool HasSet();

		protected abstract bool IsSetPublic();

		public abstract Type GetVariableType();

		public abstract object GetValue(object obj);

		public abstract void SetValue(object obj, object newValue);

		protected T _info;
	}
}
