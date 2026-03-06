using System;

namespace UnityEngine.Lumin
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	[Obsolete("Lumin is no longer supported in Unity 2022.2")]
	public sealed class UsesLuminPrivilegeAttribute : Attribute
	{
		public UsesLuminPrivilegeAttribute(string privilege)
		{
			this.m_Privilege = privilege;
		}

		public string privilege
		{
			get
			{
				return this.m_Privilege;
			}
		}

		private readonly string m_Privilege;
	}
}
