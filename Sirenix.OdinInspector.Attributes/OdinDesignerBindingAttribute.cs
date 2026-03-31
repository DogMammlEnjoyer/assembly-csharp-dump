using System;
using System.Reflection;

namespace Sirenix.OdinInspector
{
	public class OdinDesignerBindingAttribute : Attribute
	{
		public OdinDesignerBindingAttribute(params string[] memberNames)
		{
			this.MemberNames = memberNames;
		}

		public MemberInfo GetBindingMemberInfo(Type type, int index)
		{
			FieldInfo field = type.GetField(this.MemberNames[index], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				return field;
			}
			return type.GetProperty(this.MemberNames[index], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public string[] MemberNames;
	}
}
