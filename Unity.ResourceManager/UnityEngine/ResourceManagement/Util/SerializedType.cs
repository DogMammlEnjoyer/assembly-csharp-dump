using System;
using System.IO;
using System.Reflection;
using UnityEngine.Serialization;

namespace UnityEngine.ResourceManagement.Util
{
	[Serializable]
	public struct SerializedType
	{
		public string AssemblyName
		{
			get
			{
				return this.m_AssemblyName;
			}
		}

		public string ClassName
		{
			get
			{
				return this.m_ClassName;
			}
		}

		public override string ToString()
		{
			if (!(this.Value == null))
			{
				return this.Value.Name;
			}
			return "<none>";
		}

		public Type Value
		{
			get
			{
				Type result;
				try
				{
					if (string.IsNullOrEmpty(this.m_AssemblyName) || string.IsNullOrEmpty(this.m_ClassName))
					{
						result = null;
					}
					else
					{
						if (this.m_CachedType == null)
						{
							Assembly assembly = Assembly.Load(this.m_AssemblyName);
							if (assembly != null)
							{
								this.m_CachedType = assembly.GetType(this.m_ClassName);
							}
						}
						result = this.m_CachedType;
					}
				}
				catch (Exception ex)
				{
					if (ex.GetType() != typeof(FileNotFoundException))
					{
						Debug.LogException(ex);
					}
					result = null;
				}
				return result;
			}
			set
			{
				if (value != null)
				{
					this.m_AssemblyName = value.Assembly.FullName;
					this.m_ClassName = value.FullName;
					return;
				}
				this.m_AssemblyName = (this.m_ClassName = null);
			}
		}

		public bool ValueChanged { readonly get; set; }

		[FormerlySerializedAs("m_assemblyName")]
		[SerializeField]
		private string m_AssemblyName;

		[FormerlySerializedAs("m_className")]
		[SerializeField]
		private string m_ClassName;

		private Type m_CachedType;
	}
}
