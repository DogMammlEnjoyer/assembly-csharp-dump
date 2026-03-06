using System;

namespace UnityEngine.UIElements
{
	[Obsolete("UxmlObjectFactory<TCreatedType, TTraits> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	internal class UxmlObjectFactory<TCreatedType, TTraits> : BaseUxmlFactory<TCreatedType, TTraits>, IUxmlObjectFactory<TCreatedType>, IBaseUxmlObjectFactory, IBaseUxmlFactory where TCreatedType : new() where TTraits : UxmlObjectTraits<TCreatedType>, new()
	{
		public virtual TCreatedType CreateObject(IUxmlAttributes bag, CreationContext cc)
		{
			TCreatedType result = Activator.CreateInstance<TCreatedType>();
			this.m_Traits.Init(ref result, bag, cc);
			return result;
		}
	}
}
