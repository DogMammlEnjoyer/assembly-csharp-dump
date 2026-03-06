using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	/// <summary>Determines how a class or field is displayed in the debugger variable windows.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Delegate, AllowMultiple = true)]
	[ComVisible(true)]
	public sealed class DebuggerDisplayAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DebuggerDisplayAttribute" /> class.</summary>
		/// <param name="value">The string to be displayed in the value column for instances of the type; an empty string ("") causes the value column to be hidden.</param>
		public DebuggerDisplayAttribute(string value)
		{
			if (value == null)
			{
				this.value = "";
			}
			else
			{
				this.value = value;
			}
			this.name = "";
			this.type = "";
		}

		/// <summary>Gets the string to display in the value column of the debugger variable windows.</summary>
		/// <returns>The string to display in the value column of the debugger variable.</returns>
		public string Value
		{
			get
			{
				return this.value;
			}
		}

		/// <summary>Gets or sets the name to display in the debugger variable windows.</summary>
		/// <returns>The name to display in the debugger variable windows.</returns>
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

		/// <summary>Gets or sets the string to display in the type column of the debugger variable windows.</summary>
		/// <returns>The string to display in the type column of the debugger variable windows.</returns>
		public string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		/// <summary>Gets or sets the type of the attribute's target.</summary>
		/// <returns>The attribute's target type.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <see cref="P:System.Diagnostics.DebuggerDisplayAttribute.Target" /> is set to <see langword="null" />.</exception>
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

		/// <summary>Gets or sets the type name of the attribute's target.</summary>
		/// <returns>The name of the attribute's target type.</returns>
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

		private string name;

		private string value;

		private string type;

		private string targetName;

		private Type target;
	}
}
