using System;

namespace Photon.Voice
{
	public struct DeviceInfo
	{
		private DeviceInfo(bool isDefault, int idInt, string idString, string name)
		{
			this.IsDefault = isDefault;
			this.IDInt = idInt;
			this.IDString = idString;
			this.Name = name;
			this.useStringID = false;
		}

		public DeviceInfo(int id, string name)
		{
			this.IsDefault = false;
			this.IDInt = id;
			this.IDString = "";
			this.Name = name;
			this.useStringID = false;
		}

		public DeviceInfo(string id, string name)
		{
			this.IsDefault = false;
			this.IDInt = 0;
			this.IDString = id;
			this.Name = name;
			this.useStringID = true;
		}

		public DeviceInfo(string name)
		{
			this.IsDefault = false;
			this.IDInt = 0;
			this.IDString = name;
			this.Name = name;
			this.useStringID = true;
		}

		public bool IsDefault { readonly get; private set; }

		public int IDInt { readonly get; private set; }

		public string IDString { readonly get; private set; }

		public string Name { readonly get; private set; }

		public static bool operator ==(DeviceInfo d1, DeviceInfo d2)
		{
			return d1.Equals(d2);
		}

		public static bool operator !=(DeviceInfo d1, DeviceInfo d2)
		{
			return !d1.Equals(d2);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			if (this.useStringID)
			{
				return ((this.Name == null) ? "" : this.Name) + ((this.IDString == null || this.IDString == this.Name) ? "" : (" (" + this.IDString.Substring(0, Math.Min(10, this.IDString.Length)) + ")"));
			}
			return string.Format("{0} ({1})", this.Name, this.IDInt);
		}

		private bool useStringID;

		public static readonly DeviceInfo Default = new DeviceInfo(true, -128, "", "[Default]");
	}
}
