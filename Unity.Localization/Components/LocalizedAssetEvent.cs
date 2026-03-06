using System;
using UnityEngine.Events;

namespace UnityEngine.Localization.Components
{
	public class LocalizedAssetEvent<TObject, TReference, TEvent> : LocalizedAssetBehaviour<TObject, TReference> where TObject : Object where TReference : LocalizedAsset<TObject>, new() where TEvent : UnityEvent<TObject>, new()
	{
		public TEvent OnUpdateAsset
		{
			get
			{
				return this.m_UpdateAsset;
			}
			set
			{
				this.m_UpdateAsset = value;
			}
		}

		protected override void UpdateAsset(TObject localizedAsset)
		{
			this.OnUpdateAsset.Invoke(localizedAsset);
		}

		[SerializeField]
		private TEvent m_UpdateAsset = Activator.CreateInstance<TEvent>();
	}
}
