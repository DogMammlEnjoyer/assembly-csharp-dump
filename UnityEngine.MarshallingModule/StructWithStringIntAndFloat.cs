using System;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	internal struct StructWithStringIntAndFloat
	{
		public override bool Equals(object other)
		{
			bool flag = other == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = other is StructWithStringIntAndFloat;
				if (flag2)
				{
					StructWithStringIntAndFloat structWithStringIntAndFloat = (StructWithStringIntAndFloat)other;
					result = (this.a.Equals(structWithStringIntAndFloat.a) && this.b == structWithStringIntAndFloat.b && this.c == structWithStringIntAndFloat.c);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.a.GetHashCode();
		}

		public string a;

		public int b;

		public float c;
	}
}
