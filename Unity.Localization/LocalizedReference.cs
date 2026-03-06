using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

namespace UnityEngine.Localization
{
	[UxmlObject]
	[Serializable]
	public abstract class LocalizedReference : CustomBinding, ISerializationCallbackReceiver
	{
		public TableReference TableReference
		{
			get
			{
				return this.m_TableReference;
			}
			set
			{
				if (value.Equals(this.m_TableReference))
				{
					return;
				}
				this.m_TableReference = value;
				this.ForceUpdate();
			}
		}

		public TableEntryReference TableEntryReference
		{
			get
			{
				return this.m_TableEntryReference;
			}
			set
			{
				if (value.Equals(this.m_TableEntryReference))
				{
					return;
				}
				this.m_TableEntryReference = value;
				this.ForceUpdate();
			}
		}

		public FallbackBehavior FallbackState
		{
			get
			{
				return this.m_FallbackState;
			}
			set
			{
				this.m_FallbackState = value;
			}
		}

		public Locale LocaleOverride
		{
			get
			{
				return this.m_LocaleOverride;
			}
			set
			{
				if (this.m_LocaleOverride == value)
				{
					return;
				}
				this.m_LocaleOverride = value;
				this.ForceUpdate();
			}
		}

		public virtual bool WaitForCompletion
		{
			get
			{
				return this.m_WaitForCompletion;
			}
			set
			{
				this.m_WaitForCompletion = value;
			}
		}

		internal abstract bool ForceSynchronous { get; }

		public bool IsEmpty
		{
			get
			{
				return this.TableReference.ReferenceType == TableReference.Type.Empty || this.TableEntryReference.ReferenceType == TableEntryReference.Type.Empty;
			}
		}

		public void SetReference(TableReference table, TableEntryReference entry)
		{
			bool flag = false;
			if (!this.m_TableReference.Equals(table))
			{
				this.m_TableReference = table;
				flag = true;
			}
			if (!this.m_TableEntryReference.Equals(entry))
			{
				this.m_TableEntryReference = entry;
				flag = true;
			}
			if (flag)
			{
				this.ForceUpdate();
			}
		}

		public override string ToString()
		{
			return string.Format("{0}/{1}", this.TableReference, this.TableEntryReference.ToString(this.TableReference));
		}

		protected internal abstract void ForceUpdate();

		protected abstract void Reset();

		public virtual void OnBeforeSerialize()
		{
		}

		public virtual void OnAfterDeserialize()
		{
		}

		[UxmlAttribute("table")]
		internal TableReference TableReferenceUXML
		{
			get
			{
				return this.TableReference;
			}
			set
			{
				this.TableReference = value;
			}
		}

		[UxmlAttribute("entry")]
		internal TableEntryReference TableEntryReferenceUXML
		{
			get
			{
				return this.TableEntryReference;
			}
			set
			{
				this.TableEntryReference = value;
			}
		}

		[UxmlAttribute("fallback")]
		internal FallbackBehavior FallbackStateUXML
		{
			get
			{
				return this.FallbackState;
			}
			set
			{
				this.FallbackState = value;
			}
		}

		public LocalizedReference()
		{
			base.updateTrigger = BindingUpdateTrigger.WhenDirty;
		}

		protected override void OnActivated(in BindingActivationContext context)
		{
			base.OnActivated(context);
			this.m_ActivatedCount++;
			if (this.m_ActivatedCount == 1)
			{
				this.Initialize();
			}
		}

		protected override void OnDeactivated(in BindingActivationContext context)
		{
			base.OnDeactivated(context);
			this.m_ActivatedCount--;
			if (this.m_ActivatedCount == 0)
			{
				this.Cleanup();
			}
		}

		protected abstract void Initialize();

		protected abstract void Cleanup();

		internal BindingResult CreateErrorResult(in BindingContext context, VisitReturnCode errorCode, Type sourceType)
		{
			VisualElement targetElement = context.targetElement;
			string typeDisplayName = TypeUtility.GetTypeDisplayName(base.GetType());
			string text = string.Format("{0}.{1}", TypeUtility.GetTypeDisplayName(targetElement.GetType()), context.bindingId);
			BindingResult result;
			switch (errorCode)
			{
			case VisitReturnCode.InvalidPath:
				result = new BindingResult(BindingStatus.Failure, typeDisplayName + ": Binding id `" + text + "` is either invalid or contains a `null` value.");
				break;
			case VisitReturnCode.InvalidCast:
				result = new BindingResult(BindingStatus.Failure, string.Format("{0}: Invalid conversion from {1} for binding id `{2}`", typeDisplayName, sourceType, text));
				break;
			case VisitReturnCode.AccessViolation:
				result = new BindingResult(BindingStatus.Failure, typeDisplayName + ": Trying set value for binding id `" + text + "`, but it is read-only.");
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		[SerializeField]
		private TableReference m_TableReference;

		[SerializeField]
		private TableEntryReference m_TableEntryReference;

		[SerializeField]
		private FallbackBehavior m_FallbackState;

		[SerializeField]
		private bool m_WaitForCompletion;

		internal Locale m_LocaleOverride;

		private int m_ActivatedCount;

		[CompilerGenerated]
		[Serializable]
		public new abstract class UxmlSerializedData : CustomBinding.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(LocalizedReference.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("TableReferenceUXML", "table", null, Array.Empty<string>()),
					new UxmlAttributeNames("TableEntryReferenceUXML", "entry", null, Array.Empty<string>()),
					new UxmlAttributeNames("FallbackStateUXML", "fallback", null, Array.Empty<string>())
				});
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				LocalizedReference localizedReference = (LocalizedReference)obj;
				if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.TableReferenceUXML_UxmlAttributeFlags))
				{
					localizedReference.TableReferenceUXML = this.TableReferenceUXML;
				}
				if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.TableEntryReferenceUXML_UxmlAttributeFlags))
				{
					localizedReference.TableEntryReferenceUXML = this.TableEntryReferenceUXML;
				}
				if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.FallbackStateUXML_UxmlAttributeFlags))
				{
					localizedReference.FallbackStateUXML = this.FallbackStateUXML;
				}
			}

			[UxmlAttribute("table")]
			[SerializeField]
			private TableReference TableReferenceUXML;

			[UxmlAttribute("entry")]
			[SerializeField]
			private TableEntryReference TableEntryReferenceUXML;

			[UxmlAttribute("fallback")]
			[SerializeField]
			private FallbackBehavior FallbackStateUXML;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags TableReferenceUXML_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags TableEntryReferenceUXML_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags FallbackStateUXML_UxmlAttributeFlags;
		}
	}
}
