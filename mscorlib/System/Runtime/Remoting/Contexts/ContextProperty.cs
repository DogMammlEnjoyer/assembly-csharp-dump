using System;
using System.Runtime.InteropServices;
using Unity;

namespace System.Runtime.Remoting.Contexts
{
	/// <summary>Holds the name/value pair of the property name and the object representing the property of a context.</summary>
	[ComVisible(true)]
	public class ContextProperty
	{
		private ContextProperty(string name, object prop)
		{
			this.name = name;
			this.prop = prop;
		}

		/// <summary>Gets the name of the T:System.Runtime.Remoting.Contexts.ContextProperty class.</summary>
		/// <returns>The name of the <see cref="T:System.Runtime.Remoting.Contexts.ContextProperty" /> class.</returns>
		public virtual string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>Gets the object representing the property of a context.</summary>
		/// <returns>The object representing the property of a context.</returns>
		public virtual object Property
		{
			get
			{
				return this.prop;
			}
		}

		internal ContextProperty()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private string name;

		private object prop;
	}
}
