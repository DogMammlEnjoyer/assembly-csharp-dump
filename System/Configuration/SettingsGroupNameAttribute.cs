using System;

namespace System.Configuration
{
	/// <summary>Specifies a name for application settings property group. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SettingsGroupNameAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsGroupNameAttribute" /> class.</summary>
		/// <param name="groupName">A <see cref="T:System.String" /> containing the name of the application settings property group.</param>
		public SettingsGroupNameAttribute(string groupName)
		{
			this.group_name = groupName;
		}

		/// <summary>Gets the name of the application settings property group.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the name of the application settings property group.</returns>
		public string GroupName
		{
			get
			{
				return this.group_name;
			}
		}

		private string group_name;
	}
}
