using System;
using UnityEngine.Internal;

namespace Unity.Hierarchy
{
	public readonly struct HierarchyPropertyString : IEquatable<HierarchyPropertyString>, IHierarchyProperty<string>
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

		internal HierarchyPropertyString(Hierarchy hierarchy, in HierarchyPropertyId property)
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

		public string GetValue(in HierarchyNode node)
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
			return this.m_Hierarchy.GetPropertyString(this.m_Property, node);
		}

		public void SetValue(in HierarchyNode node, string value)
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
			this.m_Hierarchy.SetPropertyString(this.m_Property, node, value);
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
		public static bool operator ==(in HierarchyPropertyString lhs, in HierarchyPropertyString rhs)
		{
			return lhs.m_Hierarchy == rhs.m_Hierarchy && lhs.m_Property == rhs.m_Property;
		}

		[ExcludeFromDocs]
		public static bool operator !=(in HierarchyPropertyString lhs, in HierarchyPropertyString rhs)
		{
			return !(lhs == rhs);
		}

		[ExcludeFromDocs]
		public bool Equals(HierarchyPropertyString other)
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
			if (obj is HierarchyPropertyString)
			{
				HierarchyPropertyString other = (HierarchyPropertyString)obj;
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

		string IHierarchyProperty<string>.GetValue(in HierarchyNode node)
		{
			return this.GetValue(node);
		}

		void IHierarchyProperty<string>.SetValue(in HierarchyNode node, string value)
		{
			this.SetValue(node, value);
		}

		void IHierarchyProperty<string>.ClearValue(in HierarchyNode node)
		{
			this.ClearValue(node);
		}

		private readonly Hierarchy m_Hierarchy;

		internal readonly HierarchyPropertyId m_Property;
	}
}
