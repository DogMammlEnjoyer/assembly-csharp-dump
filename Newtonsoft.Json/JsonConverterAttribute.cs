using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	[NullableContext(1)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class JsonConverterAttribute : Attribute
	{
		public Type ConverterType
		{
			get
			{
				return this._converterType;
			}
		}

		[Nullable(new byte[]
		{
			2,
			1
		})]
		public object[] ConverterParameters { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; }

		public JsonConverterAttribute(Type converterType)
		{
			if (converterType == null)
			{
				throw new ArgumentNullException("converterType");
			}
			this._converterType = converterType;
		}

		public JsonConverterAttribute(Type converterType, params object[] converterParameters) : this(converterType)
		{
			this.ConverterParameters = converterParameters;
		}

		private readonly Type _converterType;
	}
}
