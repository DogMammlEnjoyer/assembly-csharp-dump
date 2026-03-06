using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	/// <summary>Specifies the display proxy for a type.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	[ComVisible(true)]
	public sealed class DebuggerTypeProxyAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DebuggerTypeProxyAttribute" /> class using the type of the proxy.</summary>
		/// <param name="type">The proxy type.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="type" /> is <see langword="null" />.</exception>
		public DebuggerTypeProxyAttribute(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.typeName = type.AssemblyQualifiedName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DebuggerTypeProxyAttribute" /> class using the type name of the proxy.</summary>
		/// <param name="typeName">The type name of the proxy type.</param>
		public DebuggerTypeProxyAttribute(string typeName)
		{
			this.typeName = typeName;
		}

		/// <summary>Gets the type name of the proxy type.</summary>
		/// <returns>The type name of the proxy type.</returns>
		public string ProxyTypeName
		{
			get
			{
				return this.typeName;
			}
		}

		/// <summary>Gets or sets the target type for the attribute.</summary>
		/// <returns>The target type for the attribute.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <see cref="P:System.Diagnostics.DebuggerTypeProxyAttribute.Target" /> is set to <see langword="null" />.</exception>
		public Type Target
		{
			get
			{
				return this.target;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.targetName = value.AssemblyQualifiedName;
				this.target = value;
			}
		}

		/// <summary>Gets or sets the name of the target type.</summary>
		/// <returns>The name of the target type.</returns>
		public string TargetTypeName
		{
			get
			{
				return this.targetName;
			}
			set
			{
				this.targetName = value;
			}
		}

		private string typeName;

		private string targetName;

		private Type target;
	}
}
