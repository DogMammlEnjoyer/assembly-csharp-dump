using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Xml.Serialization;

namespace System.Diagnostics
{
	/// <summary>Provides an abstract base class to create new debugging and tracing switches.</summary>
	public abstract class Switch
	{
		private object IntializedLock
		{
			get
			{
				if (this.m_intializedLock == null)
				{
					object value = new object();
					Interlocked.CompareExchange<object>(ref this.m_intializedLock, value, null);
				}
				return this.m_intializedLock;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Switch" /> class.</summary>
		/// <param name="displayName">The name of the switch.</param>
		/// <param name="description">The description for the switch.</param>
		protected Switch(string displayName, string description) : this(displayName, description, "0")
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Switch" /> class, specifying the display name, description, and default value for the switch.</summary>
		/// <param name="displayName">The name of the switch.</param>
		/// <param name="description">The description of the switch.</param>
		/// <param name="defaultSwitchValue">The default value for the switch.</param>
		protected Switch(string displayName, string description, string defaultSwitchValue)
		{
			if (displayName == null)
			{
				displayName = string.Empty;
			}
			this.displayName = displayName;
			this.description = description;
			List<WeakReference> obj = Switch.switches;
			lock (obj)
			{
				Switch._pruneCachedSwitches();
				Switch.switches.Add(new WeakReference(this));
			}
			this.defaultValue = defaultSwitchValue;
		}

		private static void _pruneCachedSwitches()
		{
			List<WeakReference> obj = Switch.switches;
			lock (obj)
			{
				if (Switch.s_LastCollectionCount != GC.CollectionCount(2))
				{
					List<WeakReference> list = new List<WeakReference>(Switch.switches.Count);
					for (int i = 0; i < Switch.switches.Count; i++)
					{
						if ((Switch)Switch.switches[i].Target != null)
						{
							list.Add(Switch.switches[i]);
						}
					}
					if (list.Count < Switch.switches.Count)
					{
						Switch.switches.Clear();
						Switch.switches.AddRange(list);
						Switch.switches.TrimExcess();
					}
					Switch.s_LastCollectionCount = GC.CollectionCount(2);
				}
			}
		}

		/// <summary>Gets the custom switch attributes defined in the application configuration file.</summary>
		/// <returns>A <see cref="T:System.Collections.Specialized.StringDictionary" /> containing the case-insensitive custom attributes for the trace switch.</returns>
		[XmlIgnore]
		public StringDictionary Attributes
		{
			get
			{
				this.Initialize();
				if (this.attributes == null)
				{
					this.attributes = new StringDictionary();
				}
				return this.attributes;
			}
		}

		/// <summary>Gets a name used to identify the switch.</summary>
		/// <returns>The name used to identify the switch. The default value is an empty string ("").</returns>
		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		/// <summary>Gets a description of the switch.</summary>
		/// <returns>The description of the switch. The default value is an empty string ("").</returns>
		public string Description
		{
			get
			{
				if (this.description != null)
				{
					return this.description;
				}
				return string.Empty;
			}
		}

		/// <summary>Gets or sets the current setting for this switch.</summary>
		/// <returns>The current setting for this switch. The default is zero.</returns>
		protected int SwitchSetting
		{
			get
			{
				if (!this.initialized && this.InitializeWithStatus())
				{
					this.OnSwitchSettingChanged();
				}
				return this.switchSetting;
			}
			set
			{
				bool flag = false;
				object intializedLock = this.IntializedLock;
				lock (intializedLock)
				{
					this.initialized = true;
					if (this.switchSetting != value)
					{
						this.switchSetting = value;
						flag = true;
					}
				}
				if (flag)
				{
					this.OnSwitchSettingChanged();
				}
			}
		}

		/// <summary>Gets or sets the value of the switch.</summary>
		/// <returns>A string representing the value of the switch.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The value is <see langword="null" />.  
		///  -or-  
		///  The value does not consist solely of an optional negative sign followed by a sequence of digits ranging from 0 to 9.  
		///  -or-  
		///  The value represents a number less than <see cref="F:System.Int32.MinValue" /> or greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		protected string Value
		{
			get
			{
				this.Initialize();
				return this.switchValueString;
			}
			set
			{
				this.Initialize();
				this.switchValueString = value;
				try
				{
					this.OnValueChanged();
				}
				catch (ArgumentException inner)
				{
					throw new ConfigurationErrorsException(SR.GetString("The config value for Switch '{0}' was invalid.", new object[]
					{
						this.DisplayName
					}), inner);
				}
				catch (FormatException inner2)
				{
					throw new ConfigurationErrorsException(SR.GetString("The config value for Switch '{0}' was invalid.", new object[]
					{
						this.DisplayName
					}), inner2);
				}
				catch (OverflowException inner3)
				{
					throw new ConfigurationErrorsException(SR.GetString("The config value for Switch '{0}' was invalid.", new object[]
					{
						this.DisplayName
					}), inner3);
				}
			}
		}

		private void Initialize()
		{
			this.InitializeWithStatus();
		}

		private bool InitializeWithStatus()
		{
			if (!this.initialized)
			{
				object intializedLock = this.IntializedLock;
				lock (intializedLock)
				{
					if (this.initialized || this.initializing)
					{
						return false;
					}
					this.initializing = true;
					if (this.switchSettings == null && !this.InitializeConfigSettings())
					{
						this.initialized = true;
						this.initializing = false;
						return false;
					}
					if (this.switchSettings != null)
					{
						SwitchElement switchElement = this.switchSettings[this.displayName];
						if (switchElement != null)
						{
							string value = switchElement.Value;
							if (value != null)
							{
								this.Value = value;
							}
							else
							{
								this.Value = this.defaultValue;
							}
							try
							{
								TraceUtils.VerifyAttributes(switchElement.Attributes, this.GetSupportedAttributes(), this);
							}
							catch (ConfigurationException)
							{
								this.initialized = false;
								this.initializing = false;
								throw;
							}
							this.attributes = new StringDictionary();
							this.attributes.ReplaceHashtable(switchElement.Attributes);
						}
						else
						{
							this.switchValueString = this.defaultValue;
							this.OnValueChanged();
						}
					}
					else
					{
						this.switchValueString = this.defaultValue;
						this.OnValueChanged();
					}
					this.initialized = true;
					this.initializing = false;
				}
				return true;
			}
			return true;
		}

		private bool InitializeConfigSettings()
		{
			if (this.switchSettings != null)
			{
				return true;
			}
			if (!DiagnosticsConfiguration.CanInitialize())
			{
				return false;
			}
			this.switchSettings = DiagnosticsConfiguration.SwitchSettings;
			return true;
		}

		/// <summary>Gets the custom attributes supported by the switch.</summary>
		/// <returns>A string array that contains the names of the custom attributes supported by the switch, or <see langword="null" /> if there no custom attributes are supported.</returns>
		protected internal virtual string[] GetSupportedAttributes()
		{
			return null;
		}

		/// <summary>Invoked when the <see cref="P:System.Diagnostics.Switch.SwitchSetting" /> property is changed.</summary>
		protected virtual void OnSwitchSettingChanged()
		{
		}

		/// <summary>Invoked when the <see cref="P:System.Diagnostics.Switch.Value" /> property is changed.</summary>
		protected virtual void OnValueChanged()
		{
			this.SwitchSetting = int.Parse(this.Value, CultureInfo.InvariantCulture);
		}

		internal static void RefreshAll()
		{
			List<WeakReference> obj = Switch.switches;
			lock (obj)
			{
				Switch._pruneCachedSwitches();
				for (int i = 0; i < Switch.switches.Count; i++)
				{
					Switch @switch = (Switch)Switch.switches[i].Target;
					if (@switch != null)
					{
						@switch.Refresh();
					}
				}
			}
		}

		internal void Refresh()
		{
			object intializedLock = this.IntializedLock;
			lock (intializedLock)
			{
				this.initialized = false;
				this.switchSettings = null;
				this.Initialize();
			}
		}

		private SwitchElementsCollection switchSettings;

		private readonly string description;

		private readonly string displayName;

		private int switchSetting;

		private volatile bool initialized;

		private bool initializing;

		private volatile string switchValueString = string.Empty;

		private StringDictionary attributes;

		private string defaultValue;

		private object m_intializedLock;

		private static List<WeakReference> switches = new List<WeakReference>();

		private static int s_LastCollectionCount;
	}
}
