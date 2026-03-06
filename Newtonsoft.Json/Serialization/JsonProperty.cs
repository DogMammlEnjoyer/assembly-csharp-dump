using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	[NullableContext(2)]
	[Nullable(0)]
	public class JsonProperty
	{
		internal JsonContract PropertyContract { get; set; }

		public string PropertyName
		{
			get
			{
				return this._propertyName;
			}
			set
			{
				this._propertyName = value;
				this._skipPropertyNameEscape = !JavaScriptUtils.ShouldEscapeJavaScriptString(this._propertyName, JavaScriptUtils.HtmlCharEscapeFlags);
			}
		}

		public Type DeclaringType { get; set; }

		public int? Order { get; set; }

		public string UnderlyingName { get; set; }

		public IValueProvider ValueProvider { get; set; }

		public IAttributeProvider AttributeProvider { get; set; }

		public Type PropertyType
		{
			get
			{
				return this._propertyType;
			}
			set
			{
				if (this._propertyType != value)
				{
					this._propertyType = value;
					this._hasGeneratedDefaultValue = false;
				}
			}
		}

		public JsonConverter Converter { get; set; }

		[Obsolete("MemberConverter is obsolete. Use Converter instead.")]
		public JsonConverter MemberConverter
		{
			get
			{
				return this.Converter;
			}
			set
			{
				this.Converter = value;
			}
		}

		public bool Ignored { get; set; }

		public bool Readable { get; set; }

		public bool Writable { get; set; }

		public bool HasMemberAttribute { get; set; }

		public object DefaultValue
		{
			get
			{
				if (!this._hasExplicitDefaultValue)
				{
					return null;
				}
				return this._defaultValue;
			}
			set
			{
				this._hasExplicitDefaultValue = true;
				this._defaultValue = value;
			}
		}

		internal object GetResolvedDefaultValue()
		{
			if (this._propertyType == null)
			{
				return null;
			}
			if (!this._hasExplicitDefaultValue && !this._hasGeneratedDefaultValue)
			{
				this._defaultValue = ReflectionUtils.GetDefaultValue(this._propertyType);
				this._hasGeneratedDefaultValue = true;
			}
			return this._defaultValue;
		}

		public Required Required
		{
			get
			{
				return this._required.GetValueOrDefault();
			}
			set
			{
				this._required = new Required?(value);
			}
		}

		public bool IsRequiredSpecified
		{
			get
			{
				return this._required != null;
			}
		}

		public bool? IsReference { get; set; }

		public NullValueHandling? NullValueHandling { get; set; }

		public DefaultValueHandling? DefaultValueHandling { get; set; }

		public ReferenceLoopHandling? ReferenceLoopHandling { get; set; }

		public ObjectCreationHandling? ObjectCreationHandling { get; set; }

		public TypeNameHandling? TypeNameHandling { get; set; }

		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Predicate<object> ShouldSerialize { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			1
		})] set; }

		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Predicate<object> ShouldDeserialize { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			1
		})] set; }

		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Predicate<object> GetIsSpecified { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			1
		})] set; }

		[Nullable(new byte[]
		{
			2,
			1,
			2
		})]
		public Action<object, object> SetIsSpecified { [return: Nullable(new byte[]
		{
			2,
			1,
			2
		})] get; [param: Nullable(new byte[]
		{
			2,
			1,
			2
		})] set; }

		[NullableContext(1)]
		public override string ToString()
		{
			return this.PropertyName ?? string.Empty;
		}

		public JsonConverter ItemConverter { get; set; }

		public bool? ItemIsReference { get; set; }

		public TypeNameHandling? ItemTypeNameHandling { get; set; }

		public ReferenceLoopHandling? ItemReferenceLoopHandling { get; set; }

		[NullableContext(1)]
		internal void WritePropertyName(JsonWriter writer)
		{
			string propertyName = this.PropertyName;
			if (this._skipPropertyNameEscape)
			{
				writer.WritePropertyName(propertyName, false);
				return;
			}
			writer.WritePropertyName(propertyName);
		}

		internal Required? _required;

		internal bool _hasExplicitDefaultValue;

		private object _defaultValue;

		private bool _hasGeneratedDefaultValue;

		private string _propertyName;

		internal bool _skipPropertyNameEscape;

		private Type _propertyType;
	}
}
