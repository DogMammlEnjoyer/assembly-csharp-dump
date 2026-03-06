using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	/// <summary>Represents information about an operating system, such as the version and platform identifier. This class cannot be inherited.</summary>
	[Serializable]
	public sealed class OperatingSystem : ISerializable, ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.OperatingSystem" /> class, using the specified platform identifier value and version object.</summary>
		/// <param name="platform">One of the <see cref="T:System.PlatformID" /> values that indicates the operating system platform.</param>
		/// <param name="version">A <see cref="T:System.Version" /> object that indicates the version of the operating system.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="version" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="platform" /> is not a <see cref="T:System.PlatformID" /> enumeration value.</exception>
		public OperatingSystem(PlatformID platform, Version version) : this(platform, version, null)
		{
		}

		internal OperatingSystem(PlatformID platform, Version version, string servicePack)
		{
			if (platform < PlatformID.Win32S || platform > PlatformID.MacOSX)
			{
				throw new ArgumentOutOfRangeException("platform", platform, SR.Format("Illegal enum value: {0}.", platform));
			}
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			this._platform = platform;
			this._version = version;
			this._servicePack = servicePack;
		}

		/// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the data necessary to deserialize this instance.</summary>
		/// <param name="info">The object to populate with serialization information.</param>
		/// <param name="context">The place to store and retrieve serialized data. Reserved for future use.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="info" /> is <see langword="null" />.</exception>
		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new PlatformNotSupportedException();
		}

		/// <summary>Gets a <see cref="T:System.PlatformID" /> enumeration value that identifies the operating system platform.</summary>
		/// <returns>One of the <see cref="T:System.PlatformID" /> values.</returns>
		public PlatformID Platform
		{
			get
			{
				return this._platform;
			}
		}

		/// <summary>Gets the service pack version represented by this <see cref="T:System.OperatingSystem" /> object.</summary>
		/// <returns>The service pack version, if service packs are supported and at least one is installed; otherwise, an empty string ("").</returns>
		public string ServicePack
		{
			get
			{
				return this._servicePack ?? string.Empty;
			}
		}

		/// <summary>Gets a <see cref="T:System.Version" /> object that identifies the operating system.</summary>
		/// <returns>A <see cref="T:System.Version" /> object that describes the major version, minor version, build, and revision numbers for the operating system.</returns>
		public Version Version
		{
			get
			{
				return this._version;
			}
		}

		/// <summary>Creates an <see cref="T:System.OperatingSystem" /> object that is identical to this instance.</summary>
		/// <returns>An <see cref="T:System.OperatingSystem" /> object that is a copy of this instance.</returns>
		public object Clone()
		{
			return new OperatingSystem(this._platform, this._version, this._servicePack);
		}

		/// <summary>Converts the value of this <see cref="T:System.OperatingSystem" /> object to its equivalent string representation.</summary>
		/// <returns>The string representation of the values returned by the <see cref="P:System.OperatingSystem.Platform" />, <see cref="P:System.OperatingSystem.Version" />, and <see cref="P:System.OperatingSystem.ServicePack" /> properties.</returns>
		public override string ToString()
		{
			return this.VersionString;
		}

		/// <summary>Gets the concatenated string representation of the platform identifier, version, and service pack that are currently installed on the operating system.</summary>
		/// <returns>The string representation of the values returned by the <see cref="P:System.OperatingSystem.Platform" />, <see cref="P:System.OperatingSystem.Version" />, and <see cref="P:System.OperatingSystem.ServicePack" /> properties.</returns>
		public string VersionString
		{
			get
			{
				if (this._versionString == null)
				{
					string str;
					switch (this._platform)
					{
					case PlatformID.Win32S:
						str = "Microsoft Win32S ";
						break;
					case PlatformID.Win32Windows:
						str = ((this._version.Major > 4 || (this._version.Major == 4 && this._version.Minor > 0)) ? "Microsoft Windows 98 " : "Microsoft Windows 95 ");
						break;
					case PlatformID.Win32NT:
						str = "Microsoft Windows NT ";
						break;
					case PlatformID.WinCE:
						str = "Microsoft Windows CE ";
						break;
					case PlatformID.Unix:
						str = "Unix ";
						break;
					case PlatformID.Xbox:
						str = "Xbox ";
						break;
					case PlatformID.MacOSX:
						str = "Mac OS X ";
						break;
					default:
						str = "<unknown> ";
						break;
					}
					this._versionString = (string.IsNullOrEmpty(this._servicePack) ? (str + this._version.ToString()) : (str + this._version.ToString(3) + " " + this._servicePack));
				}
				return this._versionString;
			}
		}

		private readonly Version _version;

		private readonly PlatformID _platform;

		private readonly string _servicePack;

		private string _versionString;
	}
}
