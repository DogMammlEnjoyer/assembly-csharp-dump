using System;
using System.Collections;
using System.Reflection;

namespace System.Diagnostics
{
	/// <summary>Identifies a switch used in an assembly, class, or member.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
	public sealed class SwitchAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.SwitchAttribute" /> class, specifying the name and the type of the switch.</summary>
		/// <param name="switchName">The display name of the switch.</param>
		/// <param name="switchType">The type of the switch.</param>
		public SwitchAttribute(string switchName, Type switchType)
		{
			this.SwitchName = switchName;
			this.SwitchType = switchType;
		}

		/// <summary>Gets or sets the display name of the switch.</summary>
		/// <returns>The display name of the switch.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <see cref="P:System.Diagnostics.SwitchAttribute.SwitchName" /> is set to <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <see cref="P:System.Diagnostics.SwitchAttribute.SwitchName" /> is set to an empty string.</exception>
		public string SwitchName
		{
			get
			{
				return this.name;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException(SR.GetString("Argument {0} cannot be null or zero-length.", new object[]
					{
						"value"
					}), "value");
				}
				this.name = value;
			}
		}

		/// <summary>Gets or sets the type of the switch.</summary>
		/// <returns>The type of the switch.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <see cref="P:System.Diagnostics.SwitchAttribute.SwitchType" /> is set to <see langword="null" />.</exception>
		public Type SwitchType
		{
			get
			{
				return this.type;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.type = value;
			}
		}

		/// <summary>Gets or sets the description of the switch.</summary>
		/// <returns>The description of the switch.</returns>
		public string SwitchDescription
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		/// <summary>Returns all switch attributes for the specified assembly.</summary>
		/// <param name="assembly">The assembly to check for switch attributes.</param>
		/// <returns>An array that contains all the switch attributes for the assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assembly" /> is <see langword="null" />.</exception>
		public static SwitchAttribute[] GetAll(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			ArrayList arrayList = new ArrayList();
			object[] customAttributes = assembly.GetCustomAttributes(typeof(SwitchAttribute), false);
			arrayList.AddRange(customAttributes);
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				SwitchAttribute.GetAllRecursive(types[i], arrayList);
			}
			SwitchAttribute[] array = new SwitchAttribute[arrayList.Count];
			arrayList.CopyTo(array, 0);
			return array;
		}

		private static void GetAllRecursive(Type type, ArrayList switchAttribs)
		{
			SwitchAttribute.GetAllRecursive(type, switchAttribs);
			MemberInfo[] members = type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < members.Length; i++)
			{
				if (!(members[i] is Type))
				{
					SwitchAttribute.GetAllRecursive(members[i], switchAttribs);
				}
			}
		}

		private static void GetAllRecursive(MemberInfo member, ArrayList switchAttribs)
		{
			object[] customAttributes = member.GetCustomAttributes(typeof(SwitchAttribute), false);
			switchAttribs.AddRange(customAttributes);
		}

		private Type type;

		private string name;

		private string description;
	}
}
