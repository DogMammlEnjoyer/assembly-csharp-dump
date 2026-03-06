using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Device;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Operations;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.Localization.Settings
{
	public class LocalizationSettings : ScriptableObject, IReset, IDisposable
	{
		internal bool IsChangingSelectedLocale { get; private set; }

		internal bool HasSelectedLocaleChangedSubscribers
		{
			get
			{
				return this.m_SelectedLocaleChanged.Length != 0;
			}
		}

		public event Action<Locale> OnSelectedLocaleChanged
		{
			add
			{
				this.m_SelectedLocaleChanged.Add(value, 5);
			}
			remove
			{
				this.m_SelectedLocaleChanged.RemoveByMovingTail(value);
			}
		}

		public static bool HasSettings
		{
			get
			{
				if (LocalizationSettings.s_Instance == null)
				{
					LocalizationSettings.s_Instance = LocalizationSettings.GetInstanceDontCreateDefault();
				}
				return LocalizationSettings.s_Instance != null;
			}
		}

		public static AsyncOperationHandle<LocalizationSettings> InitializationOperation
		{
			get
			{
				return LocalizationSettings.Instance.GetInitializationOperation();
			}
		}

		public static LocalizationSettings Instance
		{
			get
			{
				if (LocalizationSettings.s_Instance == null)
				{
					LocalizationSettings.s_Instance = LocalizationSettings.GetOrCreateSettings();
				}
				return LocalizationSettings.s_Instance;
			}
			set
			{
				LocalizationSettings.s_Instance = value;
			}
		}

		public static List<IStartupLocaleSelector> StartupLocaleSelectors
		{
			get
			{
				return LocalizationSettings.Instance.GetStartupLocaleSelectors();
			}
		}

		public static ILocalesProvider AvailableLocales
		{
			get
			{
				return LocalizationSettings.Instance.GetAvailableLocales();
			}
			set
			{
				LocalizationSettings.Instance.SetAvailableLocales(value);
			}
		}

		public static LocalizedAssetDatabase AssetDatabase
		{
			get
			{
				return LocalizationSettings.Instance.GetAssetDatabase();
			}
			set
			{
				LocalizationSettings.Instance.SetAssetDatabase(value);
			}
		}

		public static LocalizedStringDatabase StringDatabase
		{
			get
			{
				return LocalizationSettings.Instance.GetStringDatabase();
			}
			set
			{
				LocalizationSettings.Instance.SetStringDatabase(value);
			}
		}

		public static MetadataCollection Metadata
		{
			get
			{
				return LocalizationSettings.Instance.GetMetadata();
			}
		}

		public static Locale SelectedLocale
		{
			get
			{
				return LocalizationSettings.Instance.GetSelectedLocale();
			}
			set
			{
				LocalizationSettings.Instance.SetSelectedLocale(value);
			}
		}

		public static AsyncOperationHandle<Locale> SelectedLocaleAsync
		{
			get
			{
				return LocalizationSettings.Instance.GetSelectedLocaleAsync();
			}
		}

		public static event Action<Locale> SelectedLocaleChanged
		{
			add
			{
				LocalizationSettings.Instance.OnSelectedLocaleChanged += value;
			}
			remove
			{
				LocalizationSettings.Instance.OnSelectedLocaleChanged -= value;
			}
		}

		public static Locale ProjectLocale
		{
			get
			{
				if (LocalizationSettings.Instance.m_ProjectLocale == null || LocalizationSettings.Instance.m_ProjectLocale.Identifier != LocalizationSettings.Instance.m_ProjectLocaleIdentifier)
				{
					IPreloadRequired preloadRequired = LocalizationSettings.Instance.GetAvailableLocales() as IPreloadRequired;
					if (preloadRequired != null && !preloadRequired.PreloadOperation.IsDone)
					{
						preloadRequired.PreloadOperation.WaitForCompletion();
					}
					LocalizationSettings instance = LocalizationSettings.Instance;
					ILocalesProvider availableLocales = LocalizationSettings.AvailableLocales;
					instance.m_ProjectLocale = ((availableLocales != null) ? availableLocales.GetLocale(LocalizationSettings.Instance.m_ProjectLocaleIdentifier) : null);
				}
				return LocalizationSettings.Instance.m_ProjectLocale;
			}
			set
			{
				LocalizationSettings.Instance.m_ProjectLocale = value;
				LocalizationSettings.Instance.m_ProjectLocaleIdentifier = ((value != null) ? value.Identifier : default(LocaleIdentifier));
			}
		}

		public static bool InitializeSynchronously
		{
			get
			{
				return LocalizationSettings.Instance.m_InitializeSynchronously;
			}
			set
			{
				LocalizationSettings.Instance.m_InitializeSynchronously = value;
			}
		}

		public static PreloadBehavior PreloadBehavior
		{
			get
			{
				return LocalizationSettings.Instance.m_PreloadBehavior;
			}
			set
			{
				LocalizationSettings.Instance.m_PreloadBehavior = value;
			}
		}

		internal virtual void OnEnable()
		{
			if (LocalizationSettings.s_Instance == null)
			{
				LocalizationSettings.s_Instance = this;
			}
		}

		internal static void ValidateSettingsExist(string error = "")
		{
			if (!LocalizationSettings.HasSettings)
			{
				throw new Exception("There is no active LocalizationSettings.\n " + error);
			}
		}

		public virtual AsyncOperationHandle<LocalizationSettings> GetInitializationOperation()
		{
			if (!this.m_InitializingOperationHandle.IsValid())
			{
				InitializationOperation initializationOperation = UnityEngine.Localization.Operations.InitializationOperation.Pool.Get();
				initializationOperation.Init(this);
				initializationOperation.Dependency = AddressablesInterface.Instance.InitializeAddressablesAsync();
				this.m_InitializingOperationHandle = AddressablesInterface.ResourceManager.StartOperation<LocalizationSettings>(initializationOperation, initializationOperation.Dependency);
				if (!this.m_InitializingOperationHandle.IsDone && this.m_InitializeSynchronously && this.IsPlaying)
				{
					this.m_InitializingOperationHandle.WaitForCompletion();
				}
			}
			return this.m_InitializingOperationHandle;
		}

		internal bool IsChangingPlayMode
		{
			get
			{
				return this.IsPlayingOrWillChangePlaymode && !this.IsPlaying;
			}
		}

		internal bool IsPlayingOrWillChangePlaymode
		{
			get
			{
				return true;
			}
		}

		internal bool IsPlaying
		{
			get
			{
				return Application.isPlaying;
			}
		}

		internal virtual RuntimePlatform Platform
		{
			get
			{
				return Application.platform;
			}
		}

		public List<IStartupLocaleSelector> GetStartupLocaleSelectors()
		{
			return this.m_StartupSelectors;
		}

		public void SetAvailableLocales(ILocalesProvider available)
		{
			this.m_AvailableLocales = available;
		}

		public virtual ILocalesProvider GetAvailableLocales()
		{
			return this.m_AvailableLocales;
		}

		public void SetAssetDatabase(LocalizedAssetDatabase database)
		{
			this.m_AssetDatabase = database;
		}

		public virtual LocalizedAssetDatabase GetAssetDatabase()
		{
			return this.m_AssetDatabase;
		}

		public void SetStringDatabase(LocalizedStringDatabase database)
		{
			this.m_StringDatabase = database;
		}

		public virtual LocalizedStringDatabase GetStringDatabase()
		{
			return this.m_StringDatabase;
		}

		public MetadataCollection GetMetadata()
		{
			return this.m_Metadata;
		}

		public void ForceRefresh()
		{
			if (this.m_SelectedLocaleAsync.IsValid() && this.m_SelectedLocaleAsync.IsDone)
			{
				this.InvokeSelectedLocaleChanged(this.m_SelectedLocaleAsync.Result);
				return;
			}
			this.InvokeSelectedLocaleChanged(null);
		}

		internal void SendLocaleChangedEvents(Locale locale)
		{
			LocalizedStringDatabase stringDatabase = this.m_StringDatabase;
			if (stringDatabase != null)
			{
				stringDatabase.OnLocaleChanged(locale);
			}
			LocalizedAssetDatabase assetDatabase = this.m_AssetDatabase;
			if (assetDatabase != null)
			{
				assetDatabase.OnLocaleChanged(locale);
			}
			if (this.m_InitializingOperationHandle.IsValid())
			{
				AddressablesInterface.SafeRelease(this.m_InitializingOperationHandle);
				this.m_InitializingOperationHandle = default(AsyncOperationHandle<LocalizationSettings>);
			}
			if (this.GetInitializationOperation().Status == AsyncOperationStatus.Succeeded)
			{
				this.InvokeSelectedLocaleChanged(locale);
				return;
			}
			ComponentSingleton<LocalizationBehaviour>.Instance.StartCoroutine(this.InitializeAndCallSelectedLocaleChangedCoroutine(locale));
		}

		private IEnumerator InitializeAndCallSelectedLocaleChangedCoroutine(Locale locale)
		{
			yield return this.m_InitializingOperationHandle;
			this.InvokeSelectedLocaleChanged(locale);
			yield break;
		}

		private void InvokeSelectedLocaleChanged(Locale locale)
		{
			this.IsChangingSelectedLocale = true;
			try
			{
				this.m_SelectedLocaleChanged.LockForChanges();
				int length = this.m_SelectedLocaleChanged.Length;
				if (length == 1)
				{
					this.m_SelectedLocaleChanged.SingleDelegate(locale);
				}
				else if (length > 1)
				{
					Action<Locale>[] multiDelegates = this.m_SelectedLocaleChanged.MultiDelegates;
					for (int i = 0; i < length; i++)
					{
						multiDelegates[i](locale);
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			this.IsChangingSelectedLocale = false;
			this.m_SelectedLocaleChanged.UnlockForChanges();
		}

		private Locale SelectActiveLocale()
		{
			if (this.m_AvailableLocales == null)
			{
				Debug.LogError("AvailableLocales is null, can not pick a Locale.");
				return null;
			}
			if (this.m_AvailableLocales.Locales == null)
			{
				Debug.LogError("AvailableLocales.Locales is null, can not pick a Locale.");
				return null;
			}
			return this.SelectLocaleUsingStartupSelectors();
		}

		protected internal virtual Locale SelectLocaleUsingStartupSelectors()
		{
			foreach (IStartupLocaleSelector startupLocaleSelector in this.m_StartupSelectors)
			{
				Locale startupLocale = startupLocaleSelector.GetStartupLocale(this.GetAvailableLocales());
				if (startupLocale != null)
				{
					return startupLocale;
				}
			}
			StringBuilder stringBuilder;
			using (StringBuilderPool.Get(out stringBuilder))
			{
				stringBuilder.AppendLine("No Locale could be selected:");
				if (this.m_AvailableLocales.Locales.Count == 0)
				{
					stringBuilder.AppendLine("No Locales were available. Did you build the Addressables?");
				}
				else
				{
					stringBuilder.AppendLine(string.Format("The following ({0}) Locales were considered:", this.m_AvailableLocales.Locales.Count));
					foreach (Locale arg in this.m_AvailableLocales.Locales)
					{
						stringBuilder.AppendLine(string.Format("\t{0}", arg));
					}
				}
				stringBuilder.AppendLine(string.Format("The following ({0}) IStartupLocaleSelectors were used:", this.m_StartupSelectors.Count));
				foreach (IStartupLocaleSelector arg2 in this.m_StartupSelectors)
				{
					stringBuilder.AppendLine(string.Format("\t{0}", arg2));
				}
				Debug.LogError(stringBuilder.ToString(), this);
			}
			return null;
		}

		public void SetSelectedLocale(Locale locale)
		{
			if (this.m_SelectedLocaleAsync.IsValid())
			{
				Locale result = this.m_SelectedLocaleAsync.Result;
				if (result == locale)
				{
					return;
				}
				if (result != null && locale != null && result.name == locale.name && result.Identifier.Code == locale.Identifier.Code)
				{
					return;
				}
			}
			this.GetInitializationOperation();
			if (locale == null && this.IsPlayingOrWillChangePlaymode)
			{
				return;
			}
			if (!this.m_SelectedLocaleAsync.IsValid() || this.m_SelectedLocaleAsync.Result != locale)
			{
				if (this.m_SelectedLocaleAsync.IsValid())
				{
					AddressablesInterface.Release(this.m_SelectedLocaleAsync);
				}
				this.m_SelectedLocaleAsync = AddressablesInterface.ResourceManager.CreateCompletedOperation<Locale>(locale, null);
				this.SendLocaleChangedEvents(locale);
			}
		}

		public virtual AsyncOperationHandle<Locale> GetSelectedLocaleAsync()
		{
			if (!this.m_SelectedLocaleAsync.IsValid())
			{
				IPreloadRequired preloadRequired = this.GetAvailableLocales() as IPreloadRequired;
				if (preloadRequired != null && !preloadRequired.PreloadOperation.IsDone)
				{
					this.m_SelectedLocaleAsync = AddressablesInterface.ResourceManager.CreateChainOperation<Locale>(preloadRequired.PreloadOperation, (AsyncOperationHandle op) => AddressablesInterface.ResourceManager.CreateCompletedOperation<Locale>(this.SelectActiveLocale(), null));
				}
				else
				{
					this.m_SelectedLocaleAsync = AddressablesInterface.ResourceManager.CreateCompletedOperation<Locale>(this.SelectActiveLocale(), null);
				}
			}
			return this.m_SelectedLocaleAsync;
		}

		public virtual Locale GetSelectedLocale()
		{
			AsyncOperationHandle<Locale> selectedLocaleAsync = this.GetSelectedLocaleAsync();
			if (selectedLocaleAsync.IsDone)
			{
				return selectedLocaleAsync.Result;
			}
			return selectedLocaleAsync.WaitForCompletion();
		}

		public virtual void OnLocaleRemoved(Locale locale)
		{
			if (this.m_SelectedLocaleAsync.IsValid() && this.m_SelectedLocaleAsync.Result == locale)
			{
				AddressablesInterface.Release(this.m_SelectedLocaleAsync);
				this.m_SelectedLocaleAsync = default(AsyncOperationHandle<Locale>);
			}
		}

		public void ResetState()
		{
			this.m_SelectedLocaleAsync = default(AsyncOperationHandle<Locale>);
			this.m_InitializingOperationHandle = default(AsyncOperationHandle<LocalizationSettings>);
			IReset reset = this.m_AvailableLocales as IReset;
			if (reset != null)
			{
				reset.ResetState();
			}
			LocalizedAssetDatabase assetDatabase = this.m_AssetDatabase;
			if (assetDatabase != null)
			{
				((IReset)assetDatabase).ResetState();
			}
			LocalizedStringDatabase stringDatabase = this.m_StringDatabase;
			if (stringDatabase == null)
			{
				return;
			}
			((IReset)stringDatabase).ResetState();
		}

		void IDisposable.Dispose()
		{
			if (this.m_InitializingOperationHandle.IsValid())
			{
				if (!this.m_InitializingOperationHandle.IsDone)
				{
					this.m_InitializingOperationHandle.WaitForCompletion();
				}
				AddressablesInterface.Release(this.m_InitializingOperationHandle);
			}
			if (this.m_SelectedLocaleAsync.IsValid())
			{
				AddressablesInterface.Release(this.m_SelectedLocaleAsync);
			}
			this.m_InitializingOperationHandle = default(AsyncOperationHandle<LocalizationSettings>);
			this.m_SelectedLocaleAsync = default(AsyncOperationHandle<Locale>);
			IDisposable disposable = this.m_AvailableLocales as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			LocalizedAssetDatabase assetDatabase = this.m_AssetDatabase;
			if (assetDatabase != null)
			{
				((IDisposable)assetDatabase).Dispose();
			}
			LocalizedStringDatabase stringDatabase = this.m_StringDatabase;
			if (stringDatabase != null)
			{
				((IDisposable)stringDatabase).Dispose();
			}
			GC.SuppressFinalize(this);
		}

		public static LocalizationSettings GetInstanceDontCreateDefault()
		{
			if (LocalizationSettings.s_Instance != null)
			{
				return LocalizationSettings.s_Instance;
			}
			return Object.FindFirstObjectByType<LocalizationSettings>();
		}

		private static LocalizationSettings GetOrCreateSettings()
		{
			LocalizationSettings localizationSettings = LocalizationSettings.GetInstanceDontCreateDefault();
			if (localizationSettings == null)
			{
				Debug.LogWarning("Could not find localization settings. Default will be used.");
				localizationSettings = ScriptableObject.CreateInstance<LocalizationSettings>();
				localizationSettings.name = "Default Localization Settings";
			}
			return localizationSettings;
		}

		internal const string ConfigName = "com.unity.localization.settings";

		internal const string ConfigEditorLocale = "com.unity.localization-edit-locale";

		internal const string IgnoreSettings = "IgnoreSettings";

		internal const string LocaleLabel = "Locale";

		internal const string PreloadLabel = "Preload";

		[SerializeReference]
		private List<IStartupLocaleSelector> m_StartupSelectors = new List<IStartupLocaleSelector>
		{
			new CommandLineLocaleSelector(),
			new SystemLocaleSelector(),
			new SpecificLocaleSelector()
		};

		[SerializeReference]
		private ILocalesProvider m_AvailableLocales = new LocalesProvider();

		[SerializeReference]
		private LocalizedAssetDatabase m_AssetDatabase = new LocalizedAssetDatabase();

		[SerializeReference]
		private LocalizedStringDatabase m_StringDatabase = new LocalizedStringDatabase();

		[MetadataType(MetadataType.LocalizationSettings)]
		[SerializeField]
		private MetadataCollection m_Metadata = new MetadataCollection();

		[SerializeField]
		internal LocaleIdentifier m_ProjectLocaleIdentifier = "en";

		[SerializeField]
		private PreloadBehavior m_PreloadBehavior = PreloadBehavior.PreloadSelectedLocale;

		[SerializeField]
		private bool m_InitializeSynchronously;

		internal AsyncOperationHandle<LocalizationSettings> m_InitializingOperationHandle;

		private AsyncOperationHandle<Locale> m_SelectedLocaleAsync;

		private Locale m_ProjectLocale;

		private CallbackArray<Action<Locale>> m_SelectedLocaleChanged;

		internal static LocalizationSettings s_Instance;
	}
}
