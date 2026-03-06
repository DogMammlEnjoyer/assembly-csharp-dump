using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	[Obsolete("BaseUxmlFactory<TCreatedType, TTraits> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public abstract class BaseUxmlFactory<TCreatedType, TTraits> where TCreatedType : new() where TTraits : BaseUxmlTraits, new()
	{
		protected BaseUxmlFactory()
		{
			this.m_Traits = Activator.CreateInstance<TTraits>();
		}

		public virtual string uxmlName
		{
			get
			{
				return typeof(TCreatedType).Name;
			}
		}

		public virtual string uxmlNamespace
		{
			get
			{
				return typeof(TCreatedType).Namespace ?? string.Empty;
			}
		}

		public virtual string uxmlQualifiedName
		{
			get
			{
				return typeof(TCreatedType).FullName;
			}
		}

		public virtual Type uxmlType
		{
			get
			{
				return typeof(TCreatedType);
			}
		}

		public bool canHaveAnyAttribute
		{
			get
			{
				return this.m_Traits.canHaveAnyAttribute;
			}
		}

		public virtual IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
		{
			get
			{
				return this.m_Traits.uxmlAttributesDescription;
			}
		}

		public virtual IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				return this.m_Traits.uxmlChildElementsDescription;
			}
		}

		public virtual string substituteForTypeName
		{
			get
			{
				bool flag = typeof(TCreatedType) == typeof(VisualElement);
				string result;
				if (flag)
				{
					result = string.Empty;
				}
				else
				{
					result = typeof(VisualElement).Name;
				}
				return result;
			}
		}

		public virtual string substituteForTypeNamespace
		{
			get
			{
				bool flag = typeof(TCreatedType) == typeof(VisualElement);
				string result;
				if (flag)
				{
					result = string.Empty;
				}
				else
				{
					result = (typeof(VisualElement).Namespace ?? string.Empty);
				}
				return result;
			}
		}

		public virtual string substituteForTypeQualifiedName
		{
			get
			{
				bool flag = typeof(TCreatedType) == typeof(VisualElement);
				string result;
				if (flag)
				{
					result = string.Empty;
				}
				else
				{
					result = typeof(VisualElement).FullName;
				}
				return result;
			}
		}

		public virtual bool AcceptsAttributeBag(IUxmlAttributes bag, CreationContext cc)
		{
			return true;
		}

		internal TTraits m_Traits;
	}
}
