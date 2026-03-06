using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using Mono.Security.Cryptography;
using Mono.Xml;

namespace System.Security.Permissions
{
	/// <summary>Allows security actions for a <see cref="T:System.Security.PermissionSet" /> to be applied to code using declarative security. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class PermissionSetAttribute : CodeAccessSecurityAttribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Permissions.PermissionSetAttribute" /> class with the specified security action.</summary>
		/// <param name="action">One of the enumeration values that specifies a security action.</param>
		public PermissionSetAttribute(SecurityAction action) : base(action)
		{
		}

		/// <summary>Gets or sets a file containing the XML representation of a custom permission set to be declared.</summary>
		/// <returns>The physical path to the file containing the XML representation of the permission set.</returns>
		public string File
		{
			get
			{
				return this.file;
			}
			set
			{
				this.file = value;
			}
		}

		/// <summary>Gets or sets the hexadecimal representation of the XML encoded permission set.</summary>
		/// <returns>The hexadecimal representation of the XML encoded permission set.</returns>
		public string Hex
		{
			get
			{
				return this.hex;
			}
			set
			{
				this.hex = value;
			}
		}

		/// <summary>Gets or sets the name of the permission set.</summary>
		/// <returns>The name of an immutable <see cref="T:System.Security.NamedPermissionSet" /> (one of several permission sets that are contained in the default policy and cannot be altered).</returns>
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether the file specified by <see cref="P:System.Security.Permissions.PermissionSetAttribute.File" /> is Unicode or ASCII encoded.</summary>
		/// <returns>
		///   <see langword="true" /> if the file is Unicode encoded; otherwise, <see langword="false" />.</returns>
		public bool UnicodeEncoded
		{
			get
			{
				return this.isUnicodeEncoded;
			}
			set
			{
				this.isUnicodeEncoded = value;
			}
		}

		/// <summary>Gets or sets the XML representation of a permission set.</summary>
		/// <returns>The XML representation of a permission set.</returns>
		public string XML
		{
			get
			{
				return this.xml;
			}
			set
			{
				this.xml = value;
			}
		}

		/// <summary>This method is not used.</summary>
		/// <returns>A null reference (<see langword="nothing" /> in Visual Basic) in all cases.</returns>
		public override IPermission CreatePermission()
		{
			return null;
		}

		private PermissionSet CreateFromXml(string xml)
		{
			SecurityParser securityParser = new SecurityParser();
			try
			{
				securityParser.LoadXml(xml);
			}
			catch (SmallXmlParserException ex)
			{
				throw new XmlSyntaxException(ex.Line, ex.ToString());
			}
			SecurityElement securityElement = securityParser.ToXml();
			string text = securityElement.Attribute("class");
			if (text == null)
			{
				return null;
			}
			PermissionState state = PermissionState.None;
			if (CodeAccessPermission.IsUnrestricted(securityElement))
			{
				state = PermissionState.Unrestricted;
			}
			if (text.EndsWith("NamedPermissionSet"))
			{
				NamedPermissionSet namedPermissionSet = new NamedPermissionSet(securityElement.Attribute("Name"), state);
				namedPermissionSet.FromXml(securityElement);
				return namedPermissionSet;
			}
			if (text.EndsWith("PermissionSet"))
			{
				PermissionSet permissionSet = new PermissionSet(state);
				permissionSet.FromXml(securityElement);
				return permissionSet;
			}
			return null;
		}

		/// <summary>Creates and returns a new permission set based on this permission set attribute object.</summary>
		/// <returns>A new permission set.</returns>
		public PermissionSet CreatePermissionSet()
		{
			PermissionSet result = null;
			if (base.Unrestricted)
			{
				result = new PermissionSet(PermissionState.Unrestricted);
			}
			else
			{
				result = new PermissionSet(PermissionState.None);
				if (this.name != null)
				{
					return PolicyLevel.CreateAppDomainLevel().GetNamedPermissionSet(this.name);
				}
				if (this.file != null)
				{
					Encoding encoding = this.isUnicodeEncoded ? Encoding.Unicode : Encoding.ASCII;
					using (StreamReader streamReader = new StreamReader(this.file, encoding))
					{
						return this.CreateFromXml(streamReader.ReadToEnd());
					}
				}
				if (this.xml != null)
				{
					result = this.CreateFromXml(this.xml);
				}
				else if (this.hex != null)
				{
					Encoding ascii = Encoding.ASCII;
					byte[] array = CryptoConvert.FromHex(this.hex);
					result = this.CreateFromXml(ascii.GetString(array, 0, array.Length));
				}
			}
			return result;
		}

		private string file;

		private string name;

		private bool isUnicodeEncoded;

		private string xml;

		private string hex;
	}
}
