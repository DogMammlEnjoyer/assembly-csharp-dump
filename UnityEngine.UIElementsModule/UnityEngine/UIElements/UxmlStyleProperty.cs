using System;
using Unity.Collections;

namespace UnityEngine.UIElements
{
	internal struct UxmlStyleProperty : IDisposable, IEquatable<UxmlStyleProperty>
	{
		public bool isInlined
		{
			get
			{
				return this.values.Length > 0;
			}
		}

		public UxmlStyleProperty(StyleValueHandle[] values, bool requireVariableResolve)
		{
			this.values = new NativeArray<StyleValueHandle>(values, Allocator.Persistent);
			this.requireVariableResolve = requireVariableResolve;
		}

		public bool Equals(UxmlStyleProperty other)
		{
			bool flag = this.requireVariableResolve != other.requireVariableResolve;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.values.IsCreated != other.values.IsCreated;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !this.values.IsCreated;
					if (flag3)
					{
						result = true;
					}
					else
					{
						bool flag4 = this.values.Length != other.values.Length;
						if (flag4)
						{
							result = false;
						}
						else
						{
							for (int i = 0; i < this.values.Length; i++)
							{
								bool flag5 = this.values[i] != other.values[i];
								if (flag5)
								{
									return false;
								}
							}
							result = true;
						}
					}
				}
			}
			return result;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is UxmlStyleProperty)
			{
				UxmlStyleProperty other = (UxmlStyleProperty)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<NativeArray<StyleValueHandle>, bool>(this.values, this.requireVariableResolve);
		}

		public void Dispose()
		{
			this.values.Dispose();
		}

		public NativeArray<StyleValueHandle> values;

		public bool requireVariableResolve;
	}
}
