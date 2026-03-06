using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using Valve.Newtonsoft.Json.Linq;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Serialization
{
	public class JsonObjectContract : JsonContainerContract
	{
		public MemberSerialization MemberSerialization { get; set; }

		public Required? ItemRequired { get; set; }

		public JsonPropertyCollection Properties { get; private set; }

		[Obsolete("ConstructorParameters is obsolete. Use CreatorParameters instead.")]
		public JsonPropertyCollection ConstructorParameters
		{
			get
			{
				return this.CreatorParameters;
			}
		}

		public JsonPropertyCollection CreatorParameters
		{
			get
			{
				if (this._creatorParameters == null)
				{
					this._creatorParameters = new JsonPropertyCollection(base.UnderlyingType);
				}
				return this._creatorParameters;
			}
		}

		[Obsolete("OverrideConstructor is obsolete. Use OverrideCreator instead.")]
		public ConstructorInfo OverrideConstructor
		{
			get
			{
				return this._overrideConstructor;
			}
			set
			{
				this._overrideConstructor = value;
				this._overrideCreator = ((value != null) ? JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(value) : null);
			}
		}

		[Obsolete("ParametrizedConstructor is obsolete. Use OverrideCreator instead.")]
		public ConstructorInfo ParametrizedConstructor
		{
			get
			{
				return this._parametrizedConstructor;
			}
			set
			{
				this._parametrizedConstructor = value;
				this._parameterizedCreator = ((value != null) ? JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(value) : null);
			}
		}

		public ObjectConstructor<object> OverrideCreator
		{
			get
			{
				return this._overrideCreator;
			}
			set
			{
				this._overrideCreator = value;
				this._overrideConstructor = null;
			}
		}

		internal ObjectConstructor<object> ParameterizedCreator
		{
			get
			{
				return this._parameterizedCreator;
			}
		}

		public ExtensionDataSetter ExtensionDataSetter { get; set; }

		public ExtensionDataGetter ExtensionDataGetter { get; set; }

		public Type ExtensionDataValueType
		{
			get
			{
				return this._extensionDataValueType;
			}
			set
			{
				this._extensionDataValueType = value;
				this.ExtensionDataIsJToken = (value != null && typeof(JToken).IsAssignableFrom(value));
			}
		}

		internal bool HasRequiredOrDefaultValueProperties
		{
			get
			{
				if (this._hasRequiredOrDefaultValueProperties == null)
				{
					this._hasRequiredOrDefaultValueProperties = new bool?(false);
					if (this.ItemRequired.GetValueOrDefault(Required.Default) != Required.Default)
					{
						this._hasRequiredOrDefaultValueProperties = new bool?(true);
					}
					else
					{
						foreach (JsonProperty jsonProperty in this.Properties)
						{
							if (jsonProperty.Required != Required.Default || (jsonProperty.DefaultValueHandling & DefaultValueHandling.Populate) == DefaultValueHandling.Populate)
							{
								this._hasRequiredOrDefaultValueProperties = new bool?(true);
								break;
							}
						}
					}
				}
				return this._hasRequiredOrDefaultValueProperties.GetValueOrDefault();
			}
		}

		public JsonObjectContract(Type underlyingType) : base(underlyingType)
		{
			this.ContractType = JsonContractType.Object;
			this.Properties = new JsonPropertyCollection(base.UnderlyingType);
		}

		internal object GetUninitializedObject()
		{
			if (!JsonTypeReflector.FullyTrusted)
			{
				throw new JsonException("Insufficient permissions. Creating an uninitialized '{0}' type requires full trust.".FormatWith(CultureInfo.InvariantCulture, this.NonNullableUnderlyingType));
			}
			return FormatterServices.GetUninitializedObject(this.NonNullableUnderlyingType);
		}

		internal bool ExtensionDataIsJToken;

		private bool? _hasRequiredOrDefaultValueProperties;

		private ConstructorInfo _parametrizedConstructor;

		private ConstructorInfo _overrideConstructor;

		private ObjectConstructor<object> _overrideCreator;

		private ObjectConstructor<object> _parameterizedCreator;

		private JsonPropertyCollection _creatorParameters;

		private Type _extensionDataValueType;
	}
}
