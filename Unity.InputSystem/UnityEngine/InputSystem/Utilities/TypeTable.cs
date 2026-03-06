using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.InputSystem.Utilities
{
	internal struct TypeTable
	{
		public IEnumerable<string> names
		{
			get
			{
				return from x in this.table.Keys
				select x.ToString();
			}
		}

		public IEnumerable<InternedString> internedNames
		{
			get
			{
				return this.table.Keys;
			}
		}

		public void Initialize()
		{
			this.table = new Dictionary<InternedString, Type>();
		}

		public InternedString FindNameForType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			foreach (KeyValuePair<InternedString, Type> keyValuePair in this.table)
			{
				if (keyValuePair.Value == type)
				{
					return keyValuePair.Key;
				}
			}
			return default(InternedString);
		}

		public void AddTypeRegistration(string name, Type type)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Name cannot be null or empty", "name");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			InternedString key = new InternedString(name);
			this.table[key] = type;
		}

		public Type LookupTypeRegistration(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			if (this.table == null)
			{
				throw new InvalidOperationException("Input System not yet initialized");
			}
			InternedString key = new InternedString(name);
			Type result;
			if (this.table.TryGetValue(key, out result))
			{
				return result;
			}
			return null;
		}

		public Dictionary<InternedString, Type> table;
	}
}
