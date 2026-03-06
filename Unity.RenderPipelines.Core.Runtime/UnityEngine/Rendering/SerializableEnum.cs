using System;

namespace UnityEngine.Rendering
{
	[Serializable]
	public class SerializableEnum
	{
		public Enum value
		{
			get
			{
				object obj;
				if (string.IsNullOrEmpty(this.m_EnumTypeAsString) || !Enum.TryParse(Type.GetType(this.m_EnumTypeAsString), this.m_EnumValueAsString, out obj))
				{
					return null;
				}
				return (Enum)obj;
			}
			set
			{
				this.m_EnumValueAsString = value.ToString();
			}
		}

		public SerializableEnum(Type enumType)
		{
			this.m_EnumTypeAsString = enumType.AssemblyQualifiedName;
			this.m_EnumValueAsString = Enum.GetNames(enumType)[0];
		}

		[SerializeField]
		private string m_EnumValueAsString;

		[SerializeField]
		private string m_EnumTypeAsString;
	}
}
