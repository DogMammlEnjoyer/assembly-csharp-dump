using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Internal;

namespace Unity.Hierarchy
{
	public readonly struct HierarchyPropertyUnmanaged<[IsUnmanaged] T> : IEquatable<HierarchyPropertyUnmanaged<T>>, IHierarchyProperty<T> where T : struct, ValueType
	{
		public bool IsCreated
		{
			get
			{
				bool result;
				if (this.m_Property != HierarchyPropertyId.Null)
				{
					Hierarchy hierarchy = this.m_Hierarchy;
					result = (hierarchy != null && hierarchy.IsCreated);
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		internal HierarchyPropertyUnmanaged(Hierarchy hierarchy, in HierarchyPropertyId property)
		{
			bool flag = hierarchy == null;
			if (flag)
			{
				throw new ArgumentNullException("hierarchy");
			}
			bool flag2 = property == HierarchyPropertyId.Null;
			if (flag2)
			{
				throw new ArgumentException("property");
			}
			this.m_Hierarchy = hierarchy;
			this.m_Property = property;
		}

		public unsafe void SetValue(in HierarchyNode node, T value)
		{
			bool flag = this.m_Hierarchy == null;
			if (flag)
			{
				throw new NullReferenceException("Hierarchy reference has not been set.");
			}
			bool flag2 = !this.m_Hierarchy.IsCreated;
			if (flag2)
			{
				throw new InvalidOperationException("Hierarchy has been disposed.");
			}
			this.m_Hierarchy.SetPropertyRaw(this.m_Property, node, (void*)(&value), sizeof(T));
		}

		public unsafe T GetValue(in HierarchyNode node)
		{
			bool flag = this.m_Hierarchy == null;
			if (flag)
			{
				throw new NullReferenceException("Hierarchy reference has not been set.");
			}
			bool flag2 = !this.m_Hierarchy.IsCreated;
			if (flag2)
			{
				throw new InvalidOperationException("Hierarchy has been disposed.");
			}
			int num;
			void* propertyRaw = this.m_Hierarchy.GetPropertyRaw(this.m_Property, node, out num);
			bool flag3 = propertyRaw == null || num != sizeof(T);
			T result;
			if (flag3)
			{
				result = default(T);
			}
			else
			{
				result = *UnsafeUtility.AsRef<T>(propertyRaw);
			}
			return result;
		}

		public void ClearValue(in HierarchyNode node)
		{
			bool flag = this.m_Hierarchy == null;
			if (flag)
			{
				throw new NullReferenceException("Hierarchy reference has not been set.");
			}
			bool flag2 = !this.m_Hierarchy.IsCreated;
			if (flag2)
			{
				throw new InvalidOperationException("Hierarchy has been disposed.");
			}
			this.m_Hierarchy.ClearProperty(this.m_Property, node);
		}

		[ExcludeFromDocs]
		public static bool operator ==(in HierarchyPropertyUnmanaged<T> lhs, in HierarchyPropertyUnmanaged<T> rhs)
		{
			return lhs.m_Hierarchy == rhs.m_Hierarchy && lhs.m_Property == rhs.m_Property;
		}

		[ExcludeFromDocs]
		public static bool operator !=(in HierarchyPropertyUnmanaged<T> lhs, in HierarchyPropertyUnmanaged<T> rhs)
		{
			return !(lhs == rhs);
		}

		[ExcludeFromDocs]
		public bool Equals(HierarchyPropertyUnmanaged<T> other)
		{
			return this.m_Hierarchy == other.m_Hierarchy && this.m_Property == other.m_Property;
		}

		[ExcludeFromDocs]
		public override string ToString()
		{
			return this.m_Property.ToString();
		}

		[ExcludeFromDocs]
		public override bool Equals(object obj)
		{
			bool result;
			if (obj is HierarchyPropertyUnmanaged<T>)
			{
				HierarchyPropertyUnmanaged<T> other = (HierarchyPropertyUnmanaged<T>)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		[ExcludeFromDocs]
		public override int GetHashCode()
		{
			return this.m_Property.GetHashCode();
		}

		T IHierarchyProperty<!0>.GetValue(in HierarchyNode node)
		{
			return this.GetValue(node);
		}

		void IHierarchyProperty<!0>.SetValue(in HierarchyNode node, T value)
		{
			this.SetValue(node, value);
		}

		void IHierarchyProperty<!0>.ClearValue(in HierarchyNode node)
		{
			this.ClearValue(node);
		}

		private readonly Hierarchy m_Hierarchy;

		internal readonly HierarchyPropertyId m_Property;
	}
}
