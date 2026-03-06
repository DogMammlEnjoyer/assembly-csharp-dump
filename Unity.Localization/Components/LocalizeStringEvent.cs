using System;
using System.Collections.Generic;
using UnityEngine.Localization.Events;

namespace UnityEngine.Localization.Components
{
	[AddComponentMenu("Localization/Localize String Event")]
	public class LocalizeStringEvent : LocalizedMonoBehaviour
	{
		public LocalizedString StringReference
		{
			get
			{
				return this.m_StringReference;
			}
			set
			{
				this.ClearChangeHandler();
				this.m_StringReference = value;
				if (base.isActiveAndEnabled)
				{
					this.RegisterChangeHandler();
				}
			}
		}

		public UnityEventString OnUpdateString
		{
			get
			{
				return this.m_UpdateString;
			}
			set
			{
				this.m_UpdateString = value;
			}
		}

		public void RefreshString()
		{
			LocalizedString stringReference = this.StringReference;
			if (stringReference == null)
			{
				return;
			}
			stringReference.RefreshString();
		}

		public void SetTable(string tableReference)
		{
			if (this.StringReference == null)
			{
				this.StringReference = new LocalizedString();
			}
			this.StringReference.TableReference = tableReference;
		}

		public void SetEntry(string entryName)
		{
			if (this.StringReference == null)
			{
				this.StringReference = new LocalizedString();
			}
			this.StringReference.TableEntryReference = entryName;
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

		protected virtual void UpdateString(string value)
		{
			this.OnUpdateString.Invoke(value);
		}

		private void OnValidate()
		{
			this.RefreshString();
		}

		internal virtual void RegisterChangeHandler()
		{
			if (this.StringReference == null)
			{
				return;
			}
			if (this.m_FormatArguments.Count > 0)
			{
				this.StringReference.Arguments = this.m_FormatArguments.ToArray();
				if (Application.isPlaying)
				{
					Debug.LogWarningFormat("LocalizeStringEvent({0}) is using the deprecated Format Arguments field which will be removed in the future. Consider upgrading to use String Reference Local Variables instead.", new object[]
					{
						base.name,
						this
					});
				}
			}
			if (this.m_ChangeHandler == null)
			{
				this.m_ChangeHandler = new LocalizedString.ChangeHandler(this.UpdateString);
			}
			this.StringReference.StringChanged += this.m_ChangeHandler;
		}

		internal virtual void ClearChangeHandler()
		{
			if (this.StringReference != null)
			{
				this.StringReference.StringChanged -= this.m_ChangeHandler;
			}
		}

		[SerializeField]
		private LocalizedString m_StringReference = new LocalizedString();

		[SerializeField]
		private List<Object> m_FormatArguments = new List<Object>();

		[SerializeField]
		private UnityEventString m_UpdateString = new UnityEventString();

		private LocalizedString.ChangeHandler m_ChangeHandler;
	}
}
