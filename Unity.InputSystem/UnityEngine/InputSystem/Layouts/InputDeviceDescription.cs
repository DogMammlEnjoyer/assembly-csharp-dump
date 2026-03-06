using System;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Layouts
{
	[Serializable]
	public struct InputDeviceDescription : IEquatable<InputDeviceDescription>
	{
		public string interfaceName
		{
			get
			{
				return this.m_InterfaceName;
			}
			set
			{
				this.m_InterfaceName = value;
			}
		}

		public string deviceClass
		{
			get
			{
				return this.m_DeviceClass;
			}
			set
			{
				this.m_DeviceClass = value;
			}
		}

		public string manufacturer
		{
			get
			{
				return this.m_Manufacturer;
			}
			set
			{
				this.m_Manufacturer = value;
			}
		}

		public string product
		{
			get
			{
				return this.m_Product;
			}
			set
			{
				this.m_Product = value;
			}
		}

		public string serial
		{
			get
			{
				return this.m_Serial;
			}
			set
			{
				this.m_Serial = value;
			}
		}

		public string version
		{
			get
			{
				return this.m_Version;
			}
			set
			{
				this.m_Version = value;
			}
		}

		public string capabilities
		{
			get
			{
				return this.m_Capabilities;
			}
			set
			{
				this.m_Capabilities = value;
			}
		}

		public bool empty
		{
			get
			{
				return string.IsNullOrEmpty(this.m_InterfaceName) && string.IsNullOrEmpty(this.m_DeviceClass) && string.IsNullOrEmpty(this.m_Manufacturer) && string.IsNullOrEmpty(this.m_Product) && string.IsNullOrEmpty(this.m_Serial) && string.IsNullOrEmpty(this.m_Version) && string.IsNullOrEmpty(this.m_Capabilities);
			}
		}

		public override string ToString()
		{
			bool flag = !string.IsNullOrEmpty(this.product);
			bool flag2 = !string.IsNullOrEmpty(this.manufacturer);
			bool flag3 = !string.IsNullOrEmpty(this.interfaceName);
			if (flag && flag2)
			{
				if (flag3)
				{
					return string.Concat(new string[]
					{
						this.manufacturer,
						" ",
						this.product,
						" (",
						this.interfaceName,
						")"
					});
				}
				return this.manufacturer + " " + this.product;
			}
			else if (flag)
			{
				if (flag3)
				{
					return this.product + " (" + this.interfaceName + ")";
				}
				return this.product;
			}
			else if (!string.IsNullOrEmpty(this.deviceClass))
			{
				if (flag3)
				{
					return this.deviceClass + " (" + this.interfaceName + ")";
				}
				return this.deviceClass;
			}
			else if (!string.IsNullOrEmpty(this.capabilities))
			{
				string text = this.capabilities;
				if (this.capabilities.Length > 40)
				{
					text = text.Substring(0, 40) + "...";
				}
				if (flag3)
				{
					return text + " (" + this.interfaceName + ")";
				}
				return text;
			}
			else
			{
				if (flag3)
				{
					return this.interfaceName;
				}
				return "<Empty Device Description>";
			}
		}

		public bool Equals(InputDeviceDescription other)
		{
			return this.m_InterfaceName.InvariantEqualsIgnoreCase(other.m_InterfaceName) && this.m_DeviceClass.InvariantEqualsIgnoreCase(other.m_DeviceClass) && this.m_Manufacturer.InvariantEqualsIgnoreCase(other.m_Manufacturer) && this.m_Product.InvariantEqualsIgnoreCase(other.m_Product) && this.m_Serial.InvariantEqualsIgnoreCase(other.m_Serial) && this.m_Version.InvariantEqualsIgnoreCase(other.m_Version) && this.m_Capabilities.InvariantEqualsIgnoreCase(other.m_Capabilities);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is InputDeviceDescription)
			{
				InputDeviceDescription other = (InputDeviceDescription)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (((((((this.m_InterfaceName != null) ? this.m_InterfaceName.GetHashCode() : 0) * 397 ^ ((this.m_DeviceClass != null) ? this.m_DeviceClass.GetHashCode() : 0)) * 397 ^ ((this.m_Manufacturer != null) ? this.m_Manufacturer.GetHashCode() : 0)) * 397 ^ ((this.m_Product != null) ? this.m_Product.GetHashCode() : 0)) * 397 ^ ((this.m_Serial != null) ? this.m_Serial.GetHashCode() : 0)) * 397 ^ ((this.m_Version != null) ? this.m_Version.GetHashCode() : 0)) * 397 ^ ((this.m_Capabilities != null) ? this.m_Capabilities.GetHashCode() : 0);
		}

		public static bool operator ==(InputDeviceDescription left, InputDeviceDescription right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(InputDeviceDescription left, InputDeviceDescription right)
		{
			return !left.Equals(right);
		}

		public string ToJson()
		{
			return JsonUtility.ToJson(new InputDeviceDescription.DeviceDescriptionJson
			{
				@interface = this.interfaceName,
				type = this.deviceClass,
				product = this.product,
				manufacturer = this.manufacturer,
				serial = this.serial,
				version = this.version,
				capabilities = this.capabilities
			}, true);
		}

		public static InputDeviceDescription FromJson(string json)
		{
			if (json == null)
			{
				throw new ArgumentNullException("json");
			}
			InputDeviceDescription.DeviceDescriptionJson deviceDescriptionJson = JsonUtility.FromJson<InputDeviceDescription.DeviceDescriptionJson>(json);
			return new InputDeviceDescription
			{
				interfaceName = deviceDescriptionJson.@interface,
				deviceClass = deviceDescriptionJson.type,
				product = deviceDescriptionJson.product,
				manufacturer = deviceDescriptionJson.manufacturer,
				serial = deviceDescriptionJson.serial,
				version = deviceDescriptionJson.version,
				capabilities = deviceDescriptionJson.capabilities
			};
		}

		internal static bool ComparePropertyToDeviceDescriptor(string propertyName, JsonParser.JsonString propertyValue, string deviceDescriptor)
		{
			JsonParser jsonParser = new JsonParser(deviceDescriptor);
			if (!jsonParser.NavigateToProperty(propertyName))
			{
				return propertyValue.text.isEmpty;
			}
			return jsonParser.CurrentPropertyHasValueEqualTo(propertyValue);
		}

		[SerializeField]
		private string m_InterfaceName;

		[SerializeField]
		private string m_DeviceClass;

		[SerializeField]
		private string m_Manufacturer;

		[SerializeField]
		private string m_Product;

		[SerializeField]
		private string m_Serial;

		[SerializeField]
		private string m_Version;

		[SerializeField]
		private string m_Capabilities;

		private struct DeviceDescriptionJson
		{
			public string @interface;

			public string type;

			public string product;

			public string serial;

			public string version;

			public string manufacturer;

			public string capabilities;
		}
	}
}
