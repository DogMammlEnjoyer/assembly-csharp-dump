using System;

namespace UnityEngine.Localization.Components
{
	[ExecuteAlways]
	public abstract class LocalizedAssetBehaviour<TObject, TReference> : LocalizedMonoBehaviour where TObject : Object where TReference : LocalizedAsset<TObject>, new()
	{
		public TReference AssetReference
		{
			get
			{
				return this.m_LocalizedAssetReference;
			}
			set
			{
				this.ClearChangeHandler();
				this.m_LocalizedAssetReference = value;
				if (base.isActiveAndEnabled)
				{
					this.RegisterChangeHandler();
				}
			}
		}

		protected virtual void OnEnable()
		{
			this.RegisterChangeHandler();
		}

		protected virtual void OnDisable()
		{
			this.ClearChangeHandler();
		}

		private void OnDestroy()
		{
			this.ClearChangeHandler();
		}

		private void OnValidate()
		{
			TReference treference = this.AssetReference;
			if (treference == null)
			{
				return;
			}
			treference.ForceUpdate();
		}

		internal virtual void RegisterChangeHandler()
		{
			if (this.AssetReference == null)
			{
				return;
			}
			if (this.m_ChangeHandler == null)
			{
				this.m_ChangeHandler = new LocalizedAsset<TObject>.ChangeHandler(this.UpdateAsset);
			}
			this.AssetReference.AssetChanged += this.m_ChangeHandler;
		}

		internal virtual void ClearChangeHandler()
		{
			if (this.AssetReference != null)
			{
				this.AssetReference.AssetChanged -= this.m_ChangeHandler;
			}
		}

		protected abstract void UpdateAsset(TObject localizedAsset);

		[SerializeField]
		private TReference m_LocalizedAssetReference = Activator.CreateInstance<TReference>();

		private LocalizedAsset<TObject>.ChangeHandler m_ChangeHandler;
	}
}
