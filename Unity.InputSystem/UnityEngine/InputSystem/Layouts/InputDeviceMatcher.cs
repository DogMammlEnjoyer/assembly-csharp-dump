using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Layouts
{
	public struct InputDeviceMatcher : IEquatable<InputDeviceMatcher>
	{
		public bool empty
		{
			get
			{
				return this.m_Patterns == null;
			}
		}

		public IEnumerable<KeyValuePair<string, object>> patterns
		{
			get
			{
				if (this.m_Patterns == null)
				{
					yield break;
				}
				int count = this.m_Patterns.Length;
				int num;
				for (int i = 0; i < count; i = num)
				{
					yield return new KeyValuePair<string, object>(this.m_Patterns[i].Key.ToString(), this.m_Patterns[i].Value);
					num = i + 1;
				}
				yield break;
			}
		}

		public InputDeviceMatcher WithInterface(string pattern, bool supportRegex = true)
		{
			return this.With(InputDeviceMatcher.kInterfaceKey, pattern, supportRegex);
		}

		public InputDeviceMatcher WithDeviceClass(string pattern, bool supportRegex = true)
		{
			return this.With(InputDeviceMatcher.kDeviceClassKey, pattern, supportRegex);
		}

		public InputDeviceMatcher WithManufacturer(string pattern, bool supportRegex = true)
		{
			return this.With(InputDeviceMatcher.kManufacturerKey, pattern, supportRegex);
		}

		public InputDeviceMatcher WithManufacturerContains(string noRegExPattern)
		{
			return this.With(InputDeviceMatcher.kManufacturerContainsKey, noRegExPattern, false);
		}

		public InputDeviceMatcher WithProduct(string pattern, bool supportRegex = true)
		{
			return this.With(InputDeviceMatcher.kProductKey, pattern, supportRegex);
		}

		public InputDeviceMatcher WithVersion(string pattern, bool supportRegex = true)
		{
			return this.With(InputDeviceMatcher.kVersionKey, pattern, supportRegex);
		}

		public InputDeviceMatcher WithCapability<TValue>(string path, TValue value)
		{
			return this.With(new InternedString(path), value, true);
		}

		private InputDeviceMatcher With(InternedString key, object value, bool supportRegex = true)
		{
			if (supportRegex)
			{
				string text = value as string;
				if (text != null)
				{
					double num;
					if (!text.All((char ch) => char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch)) && !double.TryParse(text, out num))
					{
						value = new Regex(text, RegexOptions.IgnoreCase);
					}
				}
			}
			InputDeviceMatcher result = this;
			ArrayHelpers.Append<KeyValuePair<InternedString, object>>(ref result.m_Patterns, new KeyValuePair<InternedString, object>(key, value));
			return result;
		}

		public float MatchPercentage(InputDeviceDescription deviceDescription)
		{
			if (this.empty)
			{
				return 0f;
			}
			int num = this.m_Patterns.Length;
			for (int i = 0; i < num; i++)
			{
				InternedString key = this.m_Patterns[i].Key;
				object value = this.m_Patterns[i].Value;
				if (key == InputDeviceMatcher.kInterfaceKey)
				{
					if (string.IsNullOrEmpty(deviceDescription.interfaceName) || !InputDeviceMatcher.MatchSingleProperty(value, deviceDescription.interfaceName))
					{
						return 0f;
					}
				}
				else if (key == InputDeviceMatcher.kDeviceClassKey)
				{
					if (string.IsNullOrEmpty(deviceDescription.deviceClass) || !InputDeviceMatcher.MatchSingleProperty(value, deviceDescription.deviceClass))
					{
						return 0f;
					}
				}
				else if (key == InputDeviceMatcher.kManufacturerKey)
				{
					if (string.IsNullOrEmpty(deviceDescription.manufacturer) || !InputDeviceMatcher.MatchSingleProperty(value, deviceDescription.manufacturer))
					{
						return 0f;
					}
				}
				else if (key == InputDeviceMatcher.kManufacturerContainsKey)
				{
					if (string.IsNullOrEmpty(deviceDescription.manufacturer) || !InputDeviceMatcher.MatchSinglePropertyContains(value, deviceDescription.manufacturer))
					{
						return 0f;
					}
				}
				else if (key == InputDeviceMatcher.kProductKey)
				{
					if (string.IsNullOrEmpty(deviceDescription.product) || !InputDeviceMatcher.MatchSingleProperty(value, deviceDescription.product))
					{
						return 0f;
					}
				}
				else if (key == InputDeviceMatcher.kVersionKey)
				{
					if (string.IsNullOrEmpty(deviceDescription.version) || !InputDeviceMatcher.MatchSingleProperty(value, deviceDescription.version))
					{
						return 0f;
					}
				}
				else
				{
					if (string.IsNullOrEmpty(deviceDescription.capabilities))
					{
						return 0f;
					}
					JsonParser jsonParser = new JsonParser(deviceDescription.capabilities);
					if (!jsonParser.NavigateToProperty(key.ToString()) || !jsonParser.CurrentPropertyHasValueEqualTo(new JsonParser.JsonValue
					{
						type = JsonParser.JsonValueType.Any,
						anyValue = value
					}))
					{
						return 0f;
					}
				}
			}
			int numPropertiesIn = InputDeviceMatcher.GetNumPropertiesIn(deviceDescription);
			float num2 = 1f / (float)numPropertiesIn;
			return (float)num * num2;
		}

		private static bool MatchSingleProperty(object pattern, string value)
		{
			string text = pattern as string;
			if (text != null)
			{
				return string.Compare(text, value, StringComparison.OrdinalIgnoreCase) == 0;
			}
			Regex regex = pattern as Regex;
			return regex != null && regex.IsMatch(value);
		}

		private static bool MatchSinglePropertyContains(object pattern, string value)
		{
			string text = pattern as string;
			return text != null && value.Contains(text, StringComparison.OrdinalIgnoreCase);
		}

		private static int GetNumPropertiesIn(InputDeviceDescription description)
		{
			int num = 0;
			if (!string.IsNullOrEmpty(description.interfaceName))
			{
				num++;
			}
			if (!string.IsNullOrEmpty(description.deviceClass))
			{
				num++;
			}
			if (!string.IsNullOrEmpty(description.manufacturer))
			{
				num++;
			}
			if (!string.IsNullOrEmpty(description.product))
			{
				num++;
			}
			if (!string.IsNullOrEmpty(description.version))
			{
				num++;
			}
			if (!string.IsNullOrEmpty(description.capabilities))
			{
				num++;
			}
			return num;
		}

		public static InputDeviceMatcher FromDeviceDescription(InputDeviceDescription deviceDescription)
		{
			InputDeviceMatcher result = default(InputDeviceMatcher);
			if (!string.IsNullOrEmpty(deviceDescription.interfaceName))
			{
				result = result.WithInterface(deviceDescription.interfaceName, false);
			}
			if (!string.IsNullOrEmpty(deviceDescription.deviceClass))
			{
				result = result.WithDeviceClass(deviceDescription.deviceClass, false);
			}
			if (!string.IsNullOrEmpty(deviceDescription.manufacturer))
			{
				result = result.WithManufacturer(deviceDescription.manufacturer, false);
			}
			if (!string.IsNullOrEmpty(deviceDescription.product))
			{
				result = result.WithProduct(deviceDescription.product, false);
			}
			if (!string.IsNullOrEmpty(deviceDescription.version))
			{
				result = result.WithVersion(deviceDescription.version, false);
			}
			return result;
		}

		public override string ToString()
		{
			if (this.empty)
			{
				return "<empty>";
			}
			string text = string.Empty;
			foreach (KeyValuePair<InternedString, object> keyValuePair in this.m_Patterns)
			{
				if (text.Length > 0)
				{
					text += string.Format(",{0}={1}", keyValuePair.Key, keyValuePair.Value);
				}
				else
				{
					text += string.Format("{0}={1}", keyValuePair.Key, keyValuePair.Value);
				}
			}
			return text;
		}

		public bool Equals(InputDeviceMatcher other)
		{
			if (this.m_Patterns == other.m_Patterns)
			{
				return true;
			}
			if (this.m_Patterns == null || other.m_Patterns == null)
			{
				return false;
			}
			if (this.m_Patterns.Length != other.m_Patterns.Length)
			{
				return false;
			}
			for (int i = 0; i < this.m_Patterns.Length; i++)
			{
				KeyValuePair<InternedString, object> keyValuePair = this.m_Patterns[i];
				bool flag = false;
				int j = 0;
				while (j < this.m_Patterns.Length)
				{
					KeyValuePair<InternedString, object> keyValuePair2 = other.m_Patterns[j];
					if (!(keyValuePair.Key != keyValuePair2.Key))
					{
						if (!keyValuePair.Value.Equals(keyValuePair2.Value))
						{
							return false;
						}
						flag = true;
						break;
					}
					else
					{
						j++;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is InputDeviceMatcher)
			{
				InputDeviceMatcher other = (InputDeviceMatcher)obj;
				return this.Equals(other);
			}
			return false;
		}

		public static bool operator ==(InputDeviceMatcher left, InputDeviceMatcher right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(InputDeviceMatcher left, InputDeviceMatcher right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			if (this.m_Patterns == null)
			{
				return 0;
			}
			return this.m_Patterns.GetHashCode();
		}

		private KeyValuePair<InternedString, object>[] m_Patterns;

		private static readonly InternedString kInterfaceKey = new InternedString("interface");

		private static readonly InternedString kDeviceClassKey = new InternedString("deviceClass");

		private static readonly InternedString kManufacturerKey = new InternedString("manufacturer");

		private static readonly InternedString kManufacturerContainsKey = new InternedString("manufacturerContains");

		private static readonly InternedString kProductKey = new InternedString("product");

		private static readonly InternedString kVersionKey = new InternedString("version");

		[Serializable]
		internal struct MatcherJson
		{
			public static InputDeviceMatcher.MatcherJson FromMatcher(InputDeviceMatcher matcher)
			{
				if (matcher.empty)
				{
					return default(InputDeviceMatcher.MatcherJson);
				}
				InputDeviceMatcher.MatcherJson matcherJson = default(InputDeviceMatcher.MatcherJson);
				foreach (KeyValuePair<InternedString, object> keyValuePair in matcher.m_Patterns)
				{
					InternedString key = keyValuePair.Key;
					string value = keyValuePair.Value.ToString();
					if (key == InputDeviceMatcher.kInterfaceKey)
					{
						if (matcherJson.@interface == null)
						{
							matcherJson.@interface = value;
						}
						else
						{
							ArrayHelpers.Append<string>(ref matcherJson.interfaces, value);
						}
					}
					else if (key == InputDeviceMatcher.kDeviceClassKey)
					{
						if (matcherJson.deviceClass == null)
						{
							matcherJson.deviceClass = value;
						}
						else
						{
							ArrayHelpers.Append<string>(ref matcherJson.deviceClasses, value);
						}
					}
					else if (key == InputDeviceMatcher.kManufacturerKey)
					{
						if (matcherJson.manufacturer == null)
						{
							matcherJson.manufacturer = value;
						}
						else
						{
							ArrayHelpers.Append<string>(ref matcherJson.manufacturers, value);
						}
					}
					else if (key == InputDeviceMatcher.kProductKey)
					{
						if (matcherJson.product == null)
						{
							matcherJson.product = value;
						}
						else
						{
							ArrayHelpers.Append<string>(ref matcherJson.products, value);
						}
					}
					else if (key == InputDeviceMatcher.kVersionKey)
					{
						if (matcherJson.version == null)
						{
							matcherJson.version = value;
						}
						else
						{
							ArrayHelpers.Append<string>(ref matcherJson.versions, value);
						}
					}
					else
					{
						ArrayHelpers.Append<InputDeviceMatcher.MatcherJson.Capability>(ref matcherJson.capabilities, new InputDeviceMatcher.MatcherJson.Capability
						{
							path = key,
							value = value
						});
					}
				}
				return matcherJson;
			}

			public InputDeviceMatcher ToMatcher()
			{
				InputDeviceMatcher result = default(InputDeviceMatcher);
				if (!string.IsNullOrEmpty(this.@interface))
				{
					result = result.WithInterface(this.@interface, true);
				}
				if (this.interfaces != null)
				{
					foreach (string pattern in this.interfaces)
					{
						result = result.WithInterface(pattern, true);
					}
				}
				if (!string.IsNullOrEmpty(this.deviceClass))
				{
					result = result.WithDeviceClass(this.deviceClass, true);
				}
				if (this.deviceClasses != null)
				{
					foreach (string pattern2 in this.deviceClasses)
					{
						result = result.WithDeviceClass(pattern2, true);
					}
				}
				if (!string.IsNullOrEmpty(this.manufacturer))
				{
					result = result.WithManufacturer(this.manufacturer, true);
				}
				if (this.manufacturers != null)
				{
					foreach (string pattern3 in this.manufacturers)
					{
						result = result.WithManufacturer(pattern3, true);
					}
				}
				if (!string.IsNullOrEmpty(this.manufacturerContains))
				{
					result = result.WithManufacturerContains(this.manufacturerContains);
				}
				if (!string.IsNullOrEmpty(this.product))
				{
					result = result.WithProduct(this.product, true);
				}
				if (this.products != null)
				{
					foreach (string pattern4 in this.products)
					{
						result = result.WithProduct(pattern4, true);
					}
				}
				if (!string.IsNullOrEmpty(this.version))
				{
					result = result.WithVersion(this.version, true);
				}
				if (this.versions != null)
				{
					foreach (string pattern5 in this.versions)
					{
						result = result.WithVersion(pattern5, true);
					}
				}
				if (this.capabilities != null)
				{
					foreach (InputDeviceMatcher.MatcherJson.Capability capability in this.capabilities)
					{
						result = result.WithCapability<string>(capability.path, capability.value);
					}
				}
				return result;
			}

			public string @interface;

			public string[] interfaces;

			public string deviceClass;

			public string[] deviceClasses;

			public string manufacturer;

			public string manufacturerContains;

			public string[] manufacturers;

			public string product;

			public string[] products;

			public string version;

			public string[] versions;

			public InputDeviceMatcher.MatcherJson.Capability[] capabilities;

			public struct Capability
			{
				public string path;

				public string value;
			}
		}
	}
}
